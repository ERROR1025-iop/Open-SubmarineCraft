using UnityEngine;

namespace Scraft
{
    /// <summary>
    /// 区域检测类型枚举
    /// </summary>
    public enum AreaType
    {
        Cylinder,  // 圆柱体区域
        Cube       // 立方体区域
    }

    /// <summary>
    /// 区域检测器：支持圆柱体/立方体区域，提供点检测和随机点生成功能
    /// </summary>
    [DisallowMultipleComponent]
    public class AreaDetector : MonoBehaviour
    {
        [Header("基础设置")]
        [Tooltip("区域类型")]
        public AreaType areaType = AreaType.Cylinder;

        [Tooltip("是否限制高度（不限制则仅检测XZ平面）")]
        public bool limitHeight = true;

        [Tooltip("是否在编辑器中显示区域可视化（Gizmo）")]
        public bool showGizmo = true; // 新增：编辑器显示开关

        // 圆柱体参数
        [Tooltip("圆柱体半径（XZ平面）")]
        [SerializeField] private float cylinderRadius = 5f;
        [Tooltip("圆柱体高度（Y轴方向）")]
        [SerializeField] private float cylinderHeight = 3f;

        // 立方体参数
        [Tooltip("立方体长度（X轴方向）")]
        [SerializeField] private float cubeLength = 10f;
        [Tooltip("立方体宽度（Z轴方向）")]
        [SerializeField] private float cubeWidth = 10f;
        [Tooltip("立方体高度（Y轴方向）")]
        [SerializeField] private float cubeHeight = 3f;

        #region 公共API
        /// <summary>
        /// 判断世界坐标点是否在区域内
        /// </summary>
        /// <param name="worldPoint">待检测的世界坐标</param>
        /// <returns>true=在区域内，false=不在</returns>
        public bool IsPointInArea(Vector3 worldPoint)
        {
            // 转换为本地坐标（适配物体的位置、旋转、缩放）
            Vector3 localPoint = transform.InverseTransformPoint(worldPoint);

            return areaType switch
            {
                AreaType.Cylinder => CheckCylinderArea(localPoint),
                AreaType.Cube => CheckCubeArea(localPoint),
                _ => false
            };
        }

        /// <summary>
        /// 在区域内随机生成一个世界坐标点
        /// </summary>
        /// <returns>区域内的随机世界坐标</returns>
        public Vector3 GetRandomPointInArea()
        {
            Vector3 localRandomPoint = areaType switch
            {
                AreaType.Cylinder => GenerateRandomCylinderPoint(),
                AreaType.Cube => GenerateRandomCubePoint(),
                _ => Vector3.zero
            };

            // 转换为世界坐标并返回
            return transform.TransformPoint(localRandomPoint);
        }
        #endregion

        #region 内部检测逻辑
        /// <summary>
        /// 检测本地坐标是否在圆柱体区域内
        /// </summary>
        private bool CheckCylinderArea(Vector3 localPoint)
        {
            // 1. 检测XZ平面距离（圆柱体水平范围）
            float xzDistance = Mathf.Sqrt(localPoint.x * localPoint.x + localPoint.z * localPoint.z);
            if (xzDistance > cylinderRadius) return false;

            // 2. 检测高度（如果限制高度）
            if (limitHeight)
            {
                float halfHeight = cylinderHeight / 2f;
                if (localPoint.y < -halfHeight || localPoint.y > halfHeight)
                    return false;
            }

            return true;
        }

        /// <summary>
        /// 检测本地坐标是否在立方体区域内
        /// </summary>
        private bool CheckCubeArea(Vector3 localPoint)
        {
            float halfLength = cubeLength / 2f;
            float halfWidth = cubeWidth / 2f;

            // 1. 检测XZ平面范围
            if (localPoint.x < -halfLength || localPoint.x > halfLength) return false;
            if (localPoint.z < -halfWidth || localPoint.z > halfWidth) return false;

            // 2. 检测高度（如果限制高度）
            if (limitHeight)
            {
                float halfHeight = cubeHeight / 2f;
                if (localPoint.y < -halfHeight || localPoint.y > halfHeight)
                    return false;
            }

            return true;
        }
        #endregion

        #region 随机点生成逻辑
        /// <summary>
        /// 生成圆柱体区域内的本地随机点（均匀分布）
        /// </summary>
        private Vector3 GenerateRandomCylinderPoint()
        {
            // 极坐标随机（确保XZ平面内均匀分布）
            float angle = Random.Range(0f, Mathf.PI * 2f);
            float radius = Random.Range(0f, cylinderRadius);

            float x = radius * Mathf.Cos(angle);
            float z = radius * Mathf.Sin(angle);
            float y = limitHeight 
                ? Random.Range(-cylinderHeight / 2f, cylinderHeight / 2f) 
                : 0f; // 不限制高度时，Y轴固定在物体中心平面

            return new Vector3(x, y, z);
        }

        /// <summary>
        /// 生成立方体区域内的本地随机点
        /// </summary>
        private Vector3 GenerateRandomCubePoint()
        {
            float x = Random.Range(-cubeLength / 2f, cubeLength / 2f);
            float z = Random.Range(-cubeWidth / 2f, cubeWidth / 2f);
            float y = limitHeight 
                ? Random.Range(-cubeHeight / 2f, cubeHeight / 2f) 
                : 0f; // 不限制高度时，Y轴固定在物体中心平面

            return new Vector3(x, y, z);
        }
        #endregion

        #region 编辑器可视化（新增显示开关控制）
        /// <summary>
        /// 选中物体时绘制Gizmo（受showGizmo控制）
        /// </summary>
        private void OnDrawGizmosSelected()
        {
            if (!showGizmo) return; // 新增：如果关闭显示，则不绘制Gizmo

            Gizmos.color = new Color(0, 1, 1, 0.5f); // 青色半透明
            Gizmos.matrix = transform.localToWorldMatrix; // 适配物体的变换（位置、旋转、缩放）

            switch (areaType)
            {
                case AreaType.Cylinder:
                    DrawCylinderGizmo();
                    break;
                case AreaType.Cube:
                    DrawCubeGizmo();
                    break;
            }
        }

        /// <summary>
        /// 绘制圆柱体Gizmo
        /// </summary>
        private void DrawCylinderGizmo()
        {
            int segments = 32; // 圆柱分段数（越多越平滑）
            float halfHeight = cylinderHeight / 2f;

            if (limitHeight)
            {
                // 绘制完整圆柱体（顶面+底面+侧面）
                Vector3[] topCircle = new Vector3[segments];
                Vector3[] bottomCircle = new Vector3[segments];

                // 生成圆周点
                for (int i = 0; i < segments; i++)
                {
                    float angle = (float)i / segments * Mathf.PI * 2f;
                    topCircle[i] = new Vector3(Mathf.Cos(angle) * cylinderRadius, halfHeight, Mathf.Sin(angle) * cylinderRadius);
                    bottomCircle[i] = new Vector3(Mathf.Cos(angle) * cylinderRadius, -halfHeight, Mathf.Sin(angle) * cylinderRadius);
                }

                // 绘制顶面和底面
                for (int i = 0; i < segments; i++)
                {
                    int nextIdx = (i + 1) % segments;
                    Gizmos.DrawLine(topCircle[i], topCircle[nextIdx]);
                    Gizmos.DrawLine(bottomCircle[i], bottomCircle[nextIdx]);
                    Gizmos.DrawLine(topCircle[i], bottomCircle[i]); // 侧面竖线
                }
            }
            else
            {
                // 不限制高度：绘制XZ平面的圆（Y=0）
                Vector3 prevPoint = new Vector3(cylinderRadius, 0f, 0f);
                for (int i = 1; i <= segments; i++)
                {
                    float angle = (float)i / segments * Mathf.PI * 2f;
                    Vector3 currPoint = new Vector3(Mathf.Cos(angle) * cylinderRadius, 0f, Mathf.Sin(angle) * cylinderRadius);
                    Gizmos.DrawLine(prevPoint, currPoint);
                    prevPoint = currPoint;
                }

                // 绘制十字辅助线
                Gizmos.DrawLine(new Vector3(-cylinderRadius, 0f, 0f), new Vector3(cylinderRadius, 0f, 0f));
                Gizmos.DrawLine(new Vector3(0f, 0f, -cylinderRadius), new Vector3(0f, 0f, cylinderRadius));
            }
        }

        /// <summary>
        /// 绘制立方体Gizmo
        /// </summary>
        private void DrawCubeGizmo()
        {
            float halfLength = cubeLength / 2f;
            float halfWidth = cubeWidth / 2f;
            float halfHeight = cubeHeight / 2f;

            if (limitHeight)
            {
                // 绘制完整立方体（线框）
                Gizmos.DrawWireCube(Vector3.zero, new Vector3(cubeLength, cubeHeight, cubeWidth));
            }
            else
            {
                // 不限制高度：绘制XZ平面的矩形（Y=0）
                Vector3 p1 = new Vector3(-halfLength, 0f, -halfWidth);
                Vector3 p2 = new Vector3(halfLength, 0f, -halfWidth);
                Vector3 p3 = new Vector3(halfLength, 0f, halfWidth);
                Vector3 p4 = new Vector3(-halfLength, 0f, halfWidth);

                Gizmos.DrawLine(p1, p2);
                Gizmos.DrawLine(p2, p3);
                Gizmos.DrawLine(p3, p4);
                Gizmos.DrawLine(p4, p1);

                // 绘制十字辅助线
                Gizmos.DrawLine(new Vector3(-halfLength, 0f, 0f), new Vector3(halfLength, 0f, 0f));
                Gizmos.DrawLine(new Vector3(0f, 0f, -halfWidth), new Vector3(0f, 0f, halfWidth));
            }
        }
        #endregion

        #region 编辑器参数验证（防止非法值）
        private void OnValidate()
        {
            // 确保所有尺寸参数为正数
            cylinderRadius = Mathf.Max(0.1f, cylinderRadius);
            cylinderHeight = Mathf.Max(0.1f, cylinderHeight);
            cubeLength = Mathf.Max(0.1f, cubeLength);
            cubeWidth = Mathf.Max(0.1f, cubeWidth);
            cubeHeight = Mathf.Max(0.1f, cubeHeight);
        }
        #endregion
    }
}