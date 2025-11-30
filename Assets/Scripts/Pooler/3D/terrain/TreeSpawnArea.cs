using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 树木生成区域配置（圆柱体区域）
/// </summary>
public class TreeSpawnArea : MonoBehaviour
{
    [Header("圆柱体区域设置")]
    [Tooltip("区域半径")]
    public float radius = 20f;
    
    [Tooltip("区域高度（Y轴方向）")]
    public float height = 10f;
    
    [Tooltip("区域中心点Y偏移（相对于自身Transform）")]
    public float centerYOffset = 0f;

    [Header("生成规则设置")]
    [Tooltip("该区域可用的树木Prefab列表")]
    public List<GameObject> treePrefabs;
    
    [Tooltip("射线发射高度（相对于主角Y轴）")]
    [HideInInspector]
    public float raycastHeight = 50f;
    
    [Tooltip("区域内最大树木数量（一次性生成的总数量）")]
    public int maxTreeCount = 50;
    
    [Tooltip("树木生成密度（0-1，1=最密，影响实际生成数量）")]
    [Range(0.1f, 1f)]
    public float spawnDensity = 0.5f;

    [Header("动态范围设置")]
    [Tooltip("Z>0（水面）时的额外生成范围")]
    public float waterExtraRange = 20f;
    
    [Tooltip("Z<0（地面）时的范围缩减量")]
    public float groundReduceRange = 10f;

    [Header("生成尺寸设置")]
    [Tooltip("生成物体的缩放值（乘于Prefab原始缩放，默认(1,1,1)）")]
    public Vector3 spawnScale = Vector3.one;

    // 目标生成数量（最大数量×密度）- 改为public
    public int targetTreeCount;
    // 已生成数量 - 改为public
    public int currentTreeCount;
    // 是否已生成完成
    public bool isSpawned = false;

    private void Awake()
    {
        targetTreeCount = Mathf.RoundToInt(maxTreeCount * spawnDensity);
        currentTreeCount = 0; // 初始化已生成数量
        raycastHeight = transform.position.y + centerYOffset + height / 2;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = isSpawned ? new Color(1, 0, 0, 0.3f) : new Color(0, 1, 0, 0.3f);
        Vector3 center = transform.position + Vector3.up * centerYOffset;
        
        // 绘制圆柱可视化
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 prevPoint = center + Quaternion.Euler(0, 0, 0) * Vector3.forward * radius;
        prevPoint.y = center.y - height / 2;
        
        for (int i = 1; i <= segments; i++)
        {
            float angle = angleStep * i;
            Vector3 currPoint = center + Quaternion.Euler(0, angle, 0) * Vector3.forward * radius;
            currPoint.y = center.y - height / 2;
            Gizmos.DrawLine(prevPoint, currPoint);
            Gizmos.DrawLine(prevPoint + Vector3.up * height, currPoint + Vector3.up * height);
            Gizmos.DrawLine(prevPoint, prevPoint + Vector3.up * height);
            prevPoint = currPoint;
        }
        
        //Gizmos.DrawWireSphere(center + Vector3.up * height / 2, radius);
        //Gizmos.DrawWireSphere(center - Vector3.up * height / 2, radius);
    }

    /// <summary>
    /// 检查点是否在区域内
    /// </summary>
    public bool IsPointInArea(Vector3 point)
    {
        Vector3 center = transform.position + Vector3.up * centerYOffset;
        if (point.y < center.y - height / 2 || point.y > center.y + height / 2)
            return false;
        
        Vector3 horizontalPoint = new Vector3(point.x, center.y, point.z);
        float horizontalDistance = Vector3.Distance(horizontalPoint, center);
        return horizontalDistance <= radius;
    }

    /// <summary>
    /// 计算动态生成范围
    /// </summary>
    public float GetDynamicSpawnRange(Vector3 spawnPosition, float baseSpawnRange)
    {
        return spawnPosition.z > 0 
            ? baseSpawnRange + waterExtraRange 
            : Mathf.Max(baseSpawnRange - groundReduceRange, 5f);
    }

    /// <summary>
    /// 是否还能生成更多树木
    /// </summary>
    public bool CanSpawnMore()
    {
        return currentTreeCount < targetTreeCount;
    }

    /// <summary>
    /// 增加生成计数
    /// </summary>
    public void IncreaseTreeCount(int count = 1)
    {
        currentTreeCount = Mathf.Min(currentTreeCount + count, targetTreeCount);
    }

    /// <summary>
    /// 重置生成状态
    /// </summary>
    public void ResetSpawnState()
    {
        isSpawned = false;
        currentTreeCount = 0;
    }

    /// <summary>
    /// 获取随机树木Prefab
    /// </summary>
    public GameObject GetRandomTreePrefab()
    {
        if (treePrefabs == null || treePrefabs.Count == 0)
        {
            Debug.LogWarning($"区域 {gameObject.name} 没有配置树木Prefab！");
            return null;
        }
        return treePrefabs[Random.Range(0, treePrefabs.Count)];
    }
}