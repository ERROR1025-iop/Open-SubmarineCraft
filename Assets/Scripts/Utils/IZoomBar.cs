using UnityEngine;
using UnityEngine.UI;

namespace Scraft
{
    /// <summary>
    /// 【修复版】支持2D/3D摄像机的缩放控制条
    /// 使用“绝对位移控制” + “持久化缩放状态”
    /// 松开鼠标后缩放不会回弹
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class IZoomBar : MonoBehaviour
    {
        public static IZoomBar instance;
        public bool IsDragging => isDragging;

        // 可选：再加一个只读属性
        public float CurrentOrthoSize => camera2D != null ? camera2D.orthographicSize : defaultOrthoSize;

        public Transform camera3DTrans;
        public Camera camera2D;
        public Camera camera3D;

        [Header("缩放设置")]
        public float zoomSpeed2D = 0.005f;
        public float zoomSpeed3D = 5f;
        public float zoomSpeedFOV = 50f;

        [Header("限制范围")]
        public float minOrthoSize = 0.5f;
        public float maxOrthoSize = 10f;
        public float minFOV = 2f;
        public float maxFOV = 60f;
        public float minCameraZ = -20f;
        public float maxCameraZ = -2f;

        [Header("调试选项")]
        public bool enableDebugLogs = false;
        public bool enableGizmos = false;
        public Color gizmoColor = new Color(0f, 1f, 0f, 0.3f);

        // 内部状态
        private Rect rect;
        private Vector3 dragStartMousePos; // 拖动起点（关键）
        private bool isDragging = false;

        // 持久化缩放状态（关键）
        private float currentOrthoOffset = 0f;
        private float currentFOVOffset = 0f;
        private float currentPosOffset = 0f;

        private float defaultOrthoSize = 5.0f;
        private float defaultFOV = 60f;
        private float defaultCameraZ = -10f;

        private float dragStartOrthoSize = 2.0f;
        private float lastSize3D = 2.0f;
        private Vector3  lastPos;
        private bool isMouseOver = false;
        private bool isDragCaptured = false; // 是否已捕获拖动（即使鼠标移出区域）

        #region 生命周期

        public static float canvasW;

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
                if (enableDebugLogs) Debug.Log($"[IZoomBar] 初始化单例");
            }
            else if (instance != this)
            {
                Debug.LogWarning($"[IZoomBar] 发现重复实例，销毁 GameObject: {gameObject.name}");
                Destroy(gameObject);
            }
        }

        private void Start()
        {
            RectTransform rt = GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogError($"[IZoomBar] 缺少 RectTransform 组件！");
                enabled = false;
                return;
            }

            var canvasScaler = GameObject.Find("Canvas").GetComponent<CanvasScaler>();
            canvasW = canvasScaler.referenceResolution.x;

            Vector2 anchoredPos = rt.anchoredPosition;
            Vector2 size = rt.sizeDelta;
            rect = new Rect(anchoredPos.x, anchoredPos.y, size.x, size.y);

            // 初始化默认值
            if (camera2D != null)
            {
                defaultOrthoSize = camera2D.orthographicSize;
            }
            if (camera3D != null)
            {
                defaultFOV = camera3D.fieldOfView;
            }
            if (camera3DTrans != null)
            {
                defaultCameraZ = camera3DTrans.localPosition.z;
            }
        }

        private void Update()
        {
            Vector3 rawMouse = Input.mousePosition;
            Vector3 processedMouse = IUtils.reviseMousePos(rawMouse, canvasW);
            isMouseOver = rect.Contains(processedMouse);

            // ✅ 关键：只要捕获了拖动，或鼠标在区域内按下，就处理
            if (Input.GetMouseButtonDown(0))
            {
                if (isMouseOver)
                {
                    isDragCaptured = true; // 捕获拖动
                    HandleDragInput(processedMouse); // 立即开始
                }
            }

            if (Input.GetMouseButton(0))
            {
                if (isDragCaptured)
                {
                    HandleDragInput(processedMouse); // 即使鼠标移出也继续
                }
            }

            if (Input.GetMouseButtonUp(0))
            {
                isDragCaptured = false; // 释放捕获
                isDragging = false;
            }

            // 模式切换逻辑（可选保留）
            if (isMouseOver)
            {
                if (World.GameMode != World.GameMode_Builder)
                {
                    World.changeMode(World.GameMode_Builder);
                }

                if (World.GameMode == World.GameMode_Builder && Builder.isLoadParts)
                {
                    World.changeMode(5);
                }
            }
        }
        #endregion

        #region 拖动处理（绝对位移控制）

        private void HandleDragInput(Vector3 currentMousePos)
        {
            if (!isDragging)
            {
                dragStartMousePos = currentMousePos;
                isDragging = true;

                // ✅ 关键：在拖动开始时，读取当前缩放作为“基准”
                if (camera2D != null)
                {
                    dragStartOrthoSize = camera2D.orthographicSize;
                }

                if (enableDebugLogs)
                {
                    Debug.Log($"[IZoomBar] 开始拖动: 起点={dragStartMousePos}, 初始缩放={dragStartOrthoSize:F3}");
                }
            }
            else
            {
                float totalDeltaY = currentMousePos.y - dragStartMousePos.y;
                float dy = totalDeltaY * zoomSpeed2D;

                HandleZoom(dy);
            }
        }

        #endregion

        #region 缩放逻辑（持久化状态）

        private void HandleZoom(float dy)
        {
            bool isAssembler = World.GameMode == World.GameMode_Assembler;
            bool isHighCamera = camera3DTrans != null && World.activeCamera >= 2;
            bool isNotBuilder = !(World.GameMode == World.GameMode_Builder);
            bool isFreedomAim = World.GameMode == World.GameMode_Freedom && SubCamera.isAimMode;
            bool canScale3D = World.GameMode != World.GameMode_Freedom || SubCamera.canScale;

            if (isAssembler || (isHighCamera && isNotBuilder))
            {
                camera3DMethod(dy);
            }
            else if (camera2D != null)
            {
                camera2DMethod(dy);
            }
            else
            {
                if (enableDebugLogs) Debug.LogWarning("[IZoomBar] 无可用摄像机目标");
            }
        }

        private void camera2DMethod(float dy)
        {
            if (camera2D == null) return;

            // ✅ 基于“拖动开始时”的缩放值 + 总位移
            float targetSize = dragStartOrthoSize - dy; // 向上拖 = 放大
            float clampedSize = Mathf.Clamp(targetSize, minOrthoSize, maxOrthoSize);

            camera2D.orthographicSize = clampedSize;
            if(World.GameMode == World.GameMode_Builder)
                Builder.changeMode(1);

            if (enableDebugLogs)
            {
                Debug.Log($"[IZoomBar] 2D 缩放: 基准={dragStartOrthoSize:F3}, dy={dy:F3}, 目标={targetSize:F3}, 实际={clampedSize:F3}");
            }
        }

        void camera3DMethod(float dy)
        {
            if (World.GameMode == World.GameMode_Freedom && SubCamera.isAimMode)
            {
                dy = -dy;
                dy *= 1;
                float size = camera3D.fieldOfView;
                if ((size > 2 && dy < 0) || size < 60 && dy > 0)
                {
                    camera3D.fieldOfView = Mathf.Clamp(size + dy, 2, 60);
                }
            }
            else if (World.GameMode != World.GameMode_Freedom || SubCamera.canScale)
            {
                float size = camera3DTrans.localPosition.z;
                camera3DTrans.Translate(0, 0, dy*0.1f);
                lastSize3D = camera3DTrans.localPosition.z;
            }
            lastPos = IUtils.reviseMousePos(Input.mousePosition, canvasW);
        }
    
    

        #endregion

        #region 可视化调试

        private void OnDrawGizmos()
        {
            if (!enableGizmos || !Application.isPlaying) return;

            if (rect.width > 0 && rect.height > 0)
            {
                Vector3 center = new Vector3(rect.center.x, rect.center.y, 0);
                Vector3 size = new Vector3(rect.width, rect.height, 1);
                Gizmos.color = gizmoColor;
                Gizmos.DrawCube(center, size);

                Gizmos.color = Color.green;
                Gizmos.DrawLine(new Vector3(rect.xMin, rect.yMin), new Vector3(rect.xMax, rect.yMin));
                Gizmos.DrawLine(new Vector3(rect.xMax, rect.yMin), new Vector3(rect.xMax, rect.yMax));
                Gizmos.DrawLine(new Vector3(rect.xMax, rect.yMax), new Vector3(rect.xMin, rect.yMax));
                Gizmos.DrawLine(new Vector3(rect.xMin, rect.yMax), new Vector3(rect.xMin, rect.yMin));
            }
        }

        private void OnGUI()
        {
            if (!enableGizmos || !Application.isPlaying) return;
            GUILayout.BeginArea(new Rect(10, 10, 300, 200));
            GUILayout.Box("IZoomBar 状态", GUILayout.ExpandWidth(false));
            GUILayout.Label($"鼠标在区域内: {isMouseOver}");
            GUILayout.Label($"拖动中: {isDragging}");
            GUILayout.Label($"当前模式: {World.GameMode}");
            if (camera2D != null)
                GUILayout.Label($"正交大小: {camera2D.orthographicSize:F3}");
            if (camera3D != null)
                GUILayout.Label($"FOV: {camera3D.fieldOfView:F1}");
            if (camera3DTrans != null)
                GUILayout.Label($"相机Z: {camera3DTrans.localPosition.z:F3}");
            GUILayout.EndArea();
        }

        #endregion
    }
}