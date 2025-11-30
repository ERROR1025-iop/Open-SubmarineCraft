using UnityEngine;

public class SimulateParentFollow : MonoBehaviour
{
    [Header("模拟的父物体 (物体 A)")]
    public Transform targetParent;

    // 存储 B 相对于 A 的初始状态
    private Vector3 _initialLocalPosition;
    private Quaternion _initialLocalRotation;
    private Vector3 _initialLocalScale;

    void Start()
    {
        if (targetParent == null)
        {
            Debug.LogError("请指定 Target Parent (物体 A)！");
            enabled = false;
            return;
        }

        // 1. 计算位置偏移：将 B 的世界坐标转换为 A 的局部坐标
        // 相当于计算：如果 B 是 A 的子物体，它的 localPosition 应该是多少
        _initialLocalPosition = targetParent.InverseTransformPoint(transform.position);

        // 2. 计算旋转偏移：将 B 的世界旋转转换为 A 的局部旋转
        // 数学公式：LocalRot = Inverse(ParentRot) * ChildRot
        _initialLocalRotation = Quaternion.Inverse(targetParent.rotation) * transform.rotation;
    }

    // 使用 LateUpdate 确保在物体 A 完成所有移动/旋转后再更新 B
    // 否则如果 A 在 Update 中移动，B 可能会产生一帧的延迟抖动
    void LateUpdate()
    {
        if (targetParent == null) return;

        // --- 还原位置 ---
        // 将之前存储的局部坐标，根据 A 当前的世界状态，转换回世界坐标
        // 这会自动处理“绕 A 旋转”和“跟随 A 移动”的逻辑
        transform.position = targetParent.TransformPoint(_initialLocalPosition);

        // --- 还原旋转 ---
        // 数学公式：WorldRot = ParentRot * LocalRot
        transform.rotation = targetParent.rotation * _initialLocalRotation;
    }
}