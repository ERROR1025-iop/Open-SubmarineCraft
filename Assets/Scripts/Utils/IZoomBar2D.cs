using System;
using UnityEngine;

namespace Scraft
{
    /// <summary>
    /// 通过在UI区域拖动鼠标实现2D摄像机缩放控制
    /// 拖动方向：向上拉 = 放大（视野缩小），向下拉 = 缩小（视野放大）
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class IZoomBar2D : MonoBehaviour
    {
        public static IZoomBar2D instance;

        [Header("摄像机设置")]
        public Camera camera2D; // 要控制的2D摄像机
        public float minOrthoSize = 0.5f; // 最小缩放（最大放大）
        public float maxOrthoSize = 1000f; // 最大缩放（最小放大）

        [Header("灵敏度")]
        public float speed = 0.1f; // 缩放灵敏度

        // 内部状态
        private Rect rect;                    // 当前UI区域的屏幕矩形
        private Vector3 lastMousePos;         // 上一帧鼠标位置
        private bool hasStartedDragging;      // 是否已经开始拖动

        #region 生命周期

        private void Awake()
        {
            // 单例模式：防止重复实例
            if (instance == null)
            {
                instance = this;
            }
            else if (instance != this)
            {
                Destroy(gameObject);
                return;
            }

            DontDestroyOnLoad(gameObject); // 可选：跨场景保留
        }

        private void Start()
        {
            // 获取UI RectTransform 并计算其在屏幕空间的 Rect
            RectTransform rt = GetComponent<RectTransform>();
            if (rt == null)
            {
                Debug.LogError($"{nameof(IZoomBar2D)}: 缺少 RectTransform 组件！");
                enabled = false;
                return;
            }

            // 注意：RectTransform 的 anchoredPosition 和 sizeDelta 是相对于锚点的
            // 如果锚点不是 (0,0)，需要更复杂的转换，这里假设是左下角锚点
            Vector2 screenPos = rt.anchoredPosition;
            Vector2 screenSize = rt.sizeDelta;

            // 如果 UI 是基于 Canvas 的，可能需要使用 RectTransformUtility.ScreenPointToLocalPointInRectangle
            // 但此处假设是简单情况，直接使用 anchoredPosition 和 sizeDelta
            rect = new Rect(screenPos, screenSize);

            // 初始化状态
            hasStartedDragging = false;
            lastMousePos = Vector3.zero;

            // 检查摄像机是否设置
            if (camera2D == null)
            {
                camera2D = Camera.main; // 尝试使用主摄像机
                if (camera2D == null)
                {
                    Debug.LogError($"{nameof(IZoomBar2D)}: 未指定且未找到主摄像机！");
                    enabled = false;
                }
                else if (!camera2D.orthographic)
                {
                    Debug.LogWarning($"{nameof(IZoomBar2D)}: 摄像机不是正交模式，缩放可能不正常。");
                }
            }
        }

        private void Update()
        {
            if (camera2D == null) return;

            Vector3 currentMousePos = IUtils.reviseMousePos(Input.mousePosition);

            // 检查鼠标是否在UI区域内
            if (rect.Contains(currentMousePos))
            {
                if (Input.GetMouseButton(0)) // 按住左键拖动
                {
                    HandleDrag(currentMousePos);
                }
            }

            // 松开左键，重置拖动状态
            if (Input.GetMouseButtonUp(0))
            {
                hasStartedDragging = false;
            }
        }

        #endregion

        #region 拖动处理

        private void HandleDrag(Vector3 currentMousePos)
        {
            if (!hasStartedDragging)
            {
                // 第一次点击，记录起始位置
                lastMousePos = currentMousePos;
                hasStartedDragging = true;
            }
            else
            {
                // 计算Y方向位移
                float deltaY = currentMousePos.y - lastMousePos.y;
                float zoomDelta = deltaY * speed;

                // 应用缩放
                ApplyZoom(zoomDelta);

                // 更新上一次位置
                lastMousePos = currentMousePos;
            }
        }

        private void ApplyZoom(float zoomDelta)
        {
            float currentSize = camera2D.orthographicSize;

            // 计算新大小，限制在范围内
            float newSize = Mathf.Clamp(currentSize - zoomDelta, minOrthoSize, maxOrthoSize);

            // 避免无意义的赋值（可选优化）
            if (Mathf.Approximately(currentSize, newSize)) return;

            camera2D.orthographicSize = newSize;
        }

        #endregion

        #region 调试与可视化（可选）

        private void OnDrawGizmosSelected()
        {
            // 在编辑器中可视化控制区域（仅选中时）
            if (Application.isPlaying) return;

            RectTransform rt = GetComponent<RectTransform>();
            if (rt != null)
            {
                // 这只是一个示意，实际屏幕位置需在运行时计算
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(rt.anchoredPosition, new Vector3(rt.sizeDelta.x, rt.sizeDelta.y, 0));
            }
        }

        #endregion
    }
}