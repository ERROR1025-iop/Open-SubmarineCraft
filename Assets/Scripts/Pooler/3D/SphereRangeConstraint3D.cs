using UnityEngine;

/// <summary>
/// 3D场景专用：限制子物体在父物体为中心的球形区域内活动（每帧检测）
/// </summary>
[RequireComponent(typeof(Transform))]
public class SphereRangeConstraint3D : MonoBehaviour
{
    [Header("球形约束设置")]
    [Tooltip("约束半径（以父物体为中心）")]
    [Min(0.1f)] public float constraintRadius = 5f; // 最小半径0.1f避免异常

    private Transform _parentTransform;

    private void Awake()
    {
        // 验证父物体存在
        if (transform.parent == null)
        {
            Debug.LogError($"[{gameObject.name}] 没有父物体！脚本无法生效，将自动禁用", this);
            enabled = false;
            return;
        }

        _parentTransform = transform.parent;
    }

    private void Update()
    {
        // 每帧执行约束逻辑
        ConstrainToSphereRange();
    }

    /// <summary>
    /// 核心逻辑：将物体约束在父物体为中心的球形区域内
    /// </summary>
    private void ConstrainToSphereRange()
    {
        // 计算子物体相对于父物体的本地位置
        Vector3 localPos = transform.localPosition;
        // 计算当前距离（3D场景包含X/Y/Z三轴）
        float currentDistance = localPos.magnitude;

        // 超出半径时拉回边界（保持原方向）
        if (currentDistance > constraintRadius)
        {
            transform.localPosition = localPos.normalized * constraintRadius;
        }
    }

    /// <summary>
    /// Scene视图绘制球形边界（调试用）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        if (_parentTransform == null && transform.parent != null)
        {
            _parentTransform = transform.parent;
        }

        if (_parentTransform != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawWireSphere(_parentTransform.position, constraintRadius);
            
            // 绘制父子物体连线
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(transform.position, _parentTransform.position);
        }
    }

    /// <summary>
    /// 动态修改约束半径（安全方法）
    /// </summary>
    public void SetConstraintRadius(float newRadius)
    {
        constraintRadius = Mathf.Max(0.1f, newRadius);
    }
}