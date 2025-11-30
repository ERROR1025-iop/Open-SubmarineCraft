using UnityEngine;
using System.Collections.Generic;

namespace Scraft.DpartSpace
{
    
public class ArcDrawer : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("曲线的粗细")]
    public float lineWidth = 0.1f;

    public static ArcDrawer instance;
    
    [Tooltip("曲线的平滑度（段数），数值越高越圆滑")]
    public int segments = 50;

    // 自定义结构体，用于在面板中显示成对的Vector3
    [System.Serializable]
    public struct VectorPair
    {
        public Vector3 a;
        public Vector3 b;
    }

    [Header("数据列表")]
    public List<VectorPair> coordinates = new List<VectorPair>();

    // 用于存储生成的LineRenderer材质，通常使用 Sprites-Default 以支持顶点颜色
    private Material lineMaterial;

    void Start()
    {
        instance = this;
        // 创建一个简单的材质，使用 Sprites/Default 着色器以支持颜色渐变
        lineMaterial = new Material(Shader.Find("Sprites/Default"));
        DrawAllCurves();
    }

    /// <summary>
    /// 清除旧线条并绘制所有新线条
    /// </summary>
    public void DrawAllCurves()
    {
        // 清除之前的子物体（如果有），防止重复生成
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }

        foreach (var pair in coordinates)
        {
            CreateArc(pair.a, pair.b);
        }
    }

    void CreateArc(Vector3 start, Vector3 end)
    {
        // 如果两点重合，无法绘制半圆，直接返回
        if (Vector3.Distance(start, end) < 0.001f) return;

        // 1. 创建承载线条的子物体
        GameObject lineObj = new GameObject("Arc_Line");
        lineObj.layer = 8;
        lineObj.transform.SetParent(this.transform);
        
        LineRenderer lr = lineObj.AddComponent<LineRenderer>();

        // 2. 设置 LineRenderer 基础属性
        lr.useWorldSpace = true;
        lr.startWidth = lineWidth;
        lr.endWidth = lineWidth;
        lr.material = lineMaterial;
        lr.positionCount = segments + 1;

        // 3. 设置颜色渐变 (A端红色 -> B端灰色)
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.grey, 1.0f) },
            new GradientAlphaKey[] { new GradientAlphaKey(1.0f, 0.0f), new GradientAlphaKey(1.0f, 1.0f) }
        );
        lr.colorGradient = gradient;

        // 4. 计算半圆路径点
        Vector3[] positions = CalculateSemicirclePoints(start, end, segments);
        lr.SetPositions(positions);
    }

    Vector3[] CalculateSemicirclePoints(Vector3 p1, Vector3 p2, int count)
    {
        Vector3[] points = new Vector3[count + 1];

        Vector3 center = (p1 + p2) * 0.5f;
        Vector3 startRelative = p1 - center; // 从圆心指向起点的向量
        
        // 生成一个垂直于 (p1-p2) 的随机向量作为旋转轴
        // 这样半圆的朝向就是随机的
        Vector3 direction = (p2 - p1).normalized;
        Vector3 randomVec = Random.onUnitSphere;
        
        // 确保随机向量不与方向向量平行
        if (Vector3.Dot(randomVec, direction) > 0.99f) randomVec = Vector3.up;
        
        // 计算旋转轴（垂直于连线方向）
        Vector3 rotationAxis = Vector3.Cross(direction, randomVec).normalized;

        for (int i = 0; i <= count; i++)
        {
            float t = (float)i / count;
            // 0 到 180 度 (0 到 PI)
            float angleDeg = t * 180f;

            // 使用四元数围绕旋转轴旋转 startRelative 向量
            Quaternion rot = Quaternion.AngleAxis(angleDeg, rotationAxis);
            Vector3 pointVector = rot * startRelative;

            points[i] = center + pointVector;
        }

        return points;
    }

    // 在编辑器中添加一个按钮，方便调试时重新生成
    [ContextMenu("重新生成曲线")]
    public void Regenerate()
    {
        // 注意：在编辑器模式下直接删除子物体需使用 DestroyImmediate
        var children = new List<GameObject>();
        foreach (Transform child in transform) children.Add(child.gameObject);
        children.ForEach(c => DestroyImmediate(c));
        
        // 如果材质丢失（比如脚本重编译），重新获取
        if(lineMaterial == null) lineMaterial = new Material(Shader.Find("Sprites/Default"));

        foreach (var pair in coordinates)
        {
            CreateArc(pair.a, pair.b);
        }
    }
}
}