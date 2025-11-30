using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 每个生成区域的对象池数据
/// </summary>
[System.Serializable]
public class TreePoolData
{
    [Tooltip("对应的生成区域")]
    public TreeSpawnArea spawnArea;
    
    [Tooltip("该区域的对象池（回收的树木）")]
    public List<GameObject> pool = new List<GameObject>();
}