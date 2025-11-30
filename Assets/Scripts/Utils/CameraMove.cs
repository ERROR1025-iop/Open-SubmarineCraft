using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Scraft
{
    public class CameraMove : MonoBehaviour
    {
        [Header("移动速度")]
        public float moveSpeed = 5f;
        public float scrollSpeed = 1f;

        [Header("缩放限制")]
        public float minOrthoSize = 0.5f;
        public float maxOrthoSize = 15.8f;

        [Header("调试")]
        public bool enableDebug = false;

        private Camera mainCamera;
        private float currentSize;

        void Start()
        {
            mainCamera = Camera.main;
            if (mainCamera == null)
            {
                Debug.LogError("[CameraMove] 找不到主摄像机！");
                enabled = false;
                return;
            }

            // ✅ 从当前摄像机读取 orthographicSize，而不是硬编码
            currentSize = mainCamera.orthographicSize;
            Debug.Log($"[CameraMove] 初始化缩放值: {currentSize:F2}");            
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
                HandleMovement();
                return;
            }

            // 仅当鼠标不在UI上时处理滚轮
            if (!EventSystem.current.IsPointerOverGameObject())
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
                if (scroll != 0f)
                {
                    currentSize -= scroll; // 滚轮向下 = 放大（负值），所以减
                    currentSize = Mathf.Clamp(currentSize, minOrthoSize, maxOrthoSize);
                    mainCamera.orthographicSize = currentSize;

                    if (enableDebug)
                    {
                        Debug.Log($"[CameraMove] 滚轮缩放: {mainCamera.orthographicSize:F2}");
                    }
                }
            }

            // 处理平移
            HandleMovement();
        }

        private void HandleMovement()
        {
            float horizontal = Input.GetAxis("Horizontal");
            float vertical = Input.GetAxis("Vertical");
            if (horizontal != 0f || vertical != 0f)
            {
                Vector3 movement = new Vector3(horizontal, vertical, 0f) * moveSpeed * Time.deltaTime;
                transform.Translate(movement);
            }
        }

        // 提供一个接口，让其他脚本知道是否正在处理输入
        public bool IsScrolling => Input.GetAxis("Mouse ScrollWheel") != 0f;
    }
}