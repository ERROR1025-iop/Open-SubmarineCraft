using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scraft
{
    public class PoolerCamera2D : MonoBehaviour
    {
        [Header("移动速度")]
        public float moveSpeed = 5f;
        public float scrollSpeed = 1f;

        [Header("缩放限制")]
        public float minOrthoSize = 0.5f;
        public float maxOrthoSize = 15.8f;

        [Header("调试")]
        public bool enableDebug = false;

        public Camera mainCamera;
        private float currentSize;
        Vector3 lastPonitPos;

        void Start()
        {
            if (mainCamera == null)
            {
                Debug.LogError("[CameraMove] 找不到主摄像机！");
                enabled = false;
                return;
            }

            // ✅ 从当前摄像机读取 orthographicSize，而不是硬编码
            currentSize = mainCamera.orthographicSize;
            //Debug.Log($"[CameraMove] 初始化缩放值: {currentSize:F2}");

            // if (GameSetting.isAndroid)
            // {
            //     enabled = false;
            // }
        }

        void Update()
        {
            // 如果 IZoomBar 正在拖动，跳过滚轮逻辑（避免冲突）
            if (IZoomBar.instance != null && IZoomBar.instance.IsDragging)
            {
                // 允许移动，但暂停滚轮缩放
                HandleMovement();
                return;
            }

            // 如果是高视角相机，不处理
            if (World.activeCamera >= 2)
            {
                return;
            }

            // 仅当鼠标不在UI上时处理滚轮
            bool isPointGUI = IUtils.isPointGUI();
            if (!isPointGUI)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                if (scroll != 0f)
                {
                    currentSize -= scroll; // 滚轮向下 = 放大（负值），所以减
                    currentSize = Mathf.Clamp(currentSize, minOrthoSize, maxOrthoSize);
                    mainCamera.orthographicSize = currentSize;

                    if (enableDebug)
                    {
                        //Debug.Log($"[CameraMove] 滚轮缩放: {mainCamera.orthographicSize:F2}");
                    }
                }
            }

            // 处理平移
            HandleMovement();
        }

        private void HandleMovement()
        {
            bool isPointGUI = IUtils.isPointGUI();
            if (!isPointGUI)
            {
                if ((GameSetting.isAndroid && Input.touchCount == 2) || Input.GetMouseButton(1))
                {
                    if (lastPonitPos.Equals(Vector3.zero))
                    {
                        lastPonitPos = Input.mousePosition;
                    }
                    Vector3 dv = (Input.mousePosition - lastPonitPos) * 0.01f;
                    transform.Translate(-dv);
                    lastPonitPos = Input.mousePosition;
                }
            }
               
            if ((GameSetting.isAndroid && Input.GetMouseButtonUp(0)) || Input.GetMouseButtonUp(1))
            {
                lastPonitPos = Vector3.zero;
            }
        }

        // 提供一个接口，让其他脚本知道是否正在处理输入
        public bool IsScrolling => Input.GetAxis("Mouse ScrollWheel") != 0f;
    }
}