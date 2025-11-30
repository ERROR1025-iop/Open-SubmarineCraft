using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 挂在主角身上，负责树木批量瞬间生成、自动设置Layer和距离隐藏
/// </summary>
public class TreeSpawner : MonoBehaviour
{
    [Header("核心设置")]
    [Tooltip("地形Layer（必须设置为10）")]
    public LayerMask terrainLayer = 1 << 10;
    
    [Tooltip("树木目标Layer（固定为12）")]
    public int treeTargetLayer = 12;
    
    [Tooltip("基础生成范围（Z=0时的默认范围）")]
    public float baseSpawnRange = 30f;       
    
    [Tooltip("单帧最大生成数量（防止瞬间生成过多卡顿）")]
    [Range(10, 100)]
    public int maxTreesPerFrame = 50;

    [Header("距离隐藏设置")]
    [Tooltip("Z>0（水面）树木可视距离")]
    public float waterTreeVisibleDistance = 1000f;
    
    [Tooltip("Z<0（地面）树木可视距离")]
    public float groundTreeVisibleDistance = 100f;
    
    [Tooltip("距离检测更新间隔（秒），越大性能越好")]
    public float distanceCheckInterval = 0.5f;

    [Header("调试设置")]
    [Tooltip("是否在编辑器中显示射线调试可视化")]
    public bool showRayDebugVisualization = true;

    [Header("引用设置")]
    [Tooltip("所有树木生成区域（挂有TreeSpawnArea的对象）")]
    [HideInInspector]
    public List<TreeSpawnArea> allSpawnAreas;

    // 已生成的所有树木（用于距离检测和隐藏）
    private List<TreeData> _spawnedTrees = new List<TreeData>();
    
    // 射线检测缓存（减少GC）
    private RaycastHit[] _raycastHits = new RaycastHit[100];

    // 距离检测计时器
    private float _distanceCheckTimer;

    private Transform runtime_gen;

    /// <summary>
    /// 存储树木数据（用于距离检测）
    /// </summary>
    private class TreeData
    {
        public GameObject treeObj; // 树木对象
        public float visibleDistance; // 该树木的可视距离
        public Renderer[] renderers; // 树木的所有渲染器（用于快速隐藏/显示）

        public TreeData(GameObject obj, float distance)
        {
            treeObj = obj;
            visibleDistance = distance;
            renderers = obj.GetComponentsInChildren<Renderer>();
        }
    }

    private void Awake()
    {
        // 验证目标Layer是否存在
        if (treeTargetLayer < 0 || treeTargetLayer >= 32)
        {
            Debug.LogError("树木目标Layer超出范围（0-31），自动重置为12！");
            treeTargetLayer = 12;
        }
        
        // 自动查找场景中所有的TreeSpawnArea组件
        allSpawnAreas = new List<TreeSpawnArea>(FindObjectsOfType<TreeSpawnArea>());
        
        runtime_gen = GameObject.Find("runtime_gen").transform;
    }

    private void Update()
    {
        // 遍历所有区域，检查是否需要批量生成
        foreach (var area in allSpawnAreas)
        {
            if (area.isSpawned)
                continue;

            // 计算动态生成范围
            Vector3 tempRayOrigin = GetRandomRayOrigin(area);
            float dynamicSpawnRange = area.GetDynamicSpawnRange(tempRayOrigin, baseSpawnRange);
            
            // 检查区域是否在生成范围内
            float distanceToArea = Vector3.Distance(transform.position, area.transform.position);
            if (distanceToArea <= dynamicSpawnRange)
            {
                StartCoroutine(BatchSpawnTreesCoroutine(area, dynamicSpawnRange));
                area.isSpawned = true;
            }
        }

        // 定期检测树木距离，隐藏/显示超出范围的树木
        _distanceCheckTimer += Time.deltaTime;
        if (_distanceCheckTimer >= distanceCheckInterval)
        {
            _distanceCheckTimer = 0;
            UpdateTreeVisibility();
        }
    }

    /// <summary>
    /// 协程：批量生成树木（分帧处理）
    /// </summary>
    private IEnumerator BatchSpawnTreesCoroutine(TreeSpawnArea targetArea, float dynamicSpawnRange)
    {
        if (targetArea == null || !targetArea.CanSpawnMore())
            yield break;

        int treesToSpawn = targetArea.targetTreeCount - targetArea.currentTreeCount;
        int spawnedThisFrame = 0;

        while (targetArea.CanSpawnMore() && treesToSpawn > 0)
        {
            int rayCount = Mathf.Min(100, treesToSpawn);
            int hitCount = 0;

            for (int i = 0; i < rayCount; i++)
            {
                Vector3 rayOrigin = GetRandomRayOrigin(targetArea);
                
                // 检查射线起点是否在生成范围内
                if (Vector3.Distance(rayOrigin, transform.position) > dynamicSpawnRange)
                    continue;

                // 射线检测地形 - 使用无限距离，但只取第一个击中点
                int hits = Physics.RaycastNonAlloc(rayOrigin, Vector3.down, _raycastHits, Mathf.Infinity, terrainLayer);
                if (hits > 0)
                {
                    // 从所有击中点中找到第一个（y坐标最大的，也就是最上面的）击中点
                    RaycastHit firstHit = _raycastHits[0];
                    for (int j = 1; j < hits; j++)
                    {
                        if (_raycastHits[j].point.y > firstHit.point.y)
                        {
                            firstHit = _raycastHits[j];
                        }
                    }
                    
                    // 检查击中点是否在区域内（特别是Y坐标是否高于区域下边界）
                    Vector3 areaCenter = targetArea.transform.position + Vector3.up * targetArea.centerYOffset;
                    float areaBottom = areaCenter.y - targetArea.height / 2;
                    
                    if (firstHit.point.y < areaBottom)
                    {
                        // 击中点低于区域下边界，跳过生成
                        // 清空缓存
                        for (int j = 0; j < hits; j++)
                        {
                            _raycastHits[j] = new RaycastHit();
                        }       
                        
                        continue;
                    }
                    
                    // 清空缓存
                    for (int j = 0; j < hits; j++)
                    {
                        _raycastHits[j] = new RaycastHit();
                    }

                    // 在编辑器中可视化射线
                    if (showRayDebugVisualization && Application.isEditor)
                    {
                        Debug.DrawLine(rayOrigin, firstHit.point, Color.green, 100.0f);
                    }

                    // 生成树木
                    GameObject treePrefab = targetArea.GetRandomTreePrefab();
                    if (treePrefab == null)
                        continue;

                    GameObject tree = Instantiate(treePrefab, firstHit.point, Quaternion.Euler(0, Random.Range(0, 360f), 0));

                    // Apply area-defined scale to instantiated tree (multiply with prefab's original scale)
                    tree.transform.localScale = Vector3.Scale(tree.transform.localScale, targetArea.spawnScale);

                    tree.AddComponent<EarthCurvatureObjectController>();
                    tree.transform.SetParent(runtime_gen);
                    if (tree.name.Contains("Rock"))
                    {
                        tree.tag = "terrain";
                    }
                    
                    // 1. 设置树木Layer=12（递归设置所有子对象）
                    SetObjectLayerRecursively(tree, treeTargetLayer);
                    
                    // 2. 计算该树木的可视距离（根据Z轴位置）
                    float visibleDistance = firstHit.point.z > 0 ? waterTreeVisibleDistance : groundTreeVisibleDistance;
                    
                    // 3. 存储树木数据，用于后续距离检测
                    _spawnedTrees.Add(new TreeData(tree, visibleDistance));
                    
                    // 4. 初始显示树木
                    SetTreeVisible(tree, true);

                    targetArea.IncreaseTreeCount();
                    spawnedThisFrame++;
                    treesToSpawn--;
                    hitCount++;

                    // 控制单帧生成数量，避免卡顿
                    if (spawnedThisFrame >= maxTreesPerFrame)
                    {
                        spawnedThisFrame = 0;
                        yield return null;
                    }
                }
#if UNITY_EDITOR
                else
                {
                    // 可视化未击中的射线
                    if (showRayDebugVisualization)
                    {
                        Debug.DrawRay(rayOrigin, Vector3.down * 100f, Color.red, 1.0f);
                    }
                }
#endif
            }

            // 没有命中地形时，短暂等待避免死循环
            if (hitCount == 0)
                yield return new WaitForEndOfFrame();
        }

        Debug.Log($"区域 {targetArea.gameObject.name} 树木生成完成！共生成 {targetArea.currentTreeCount} 棵，Layer={treeTargetLayer}");
    }

    /// <summary>
    /// 递归设置对象及其子对象的Layer
    /// </summary>
    private void SetObjectLayerRecursively(GameObject obj, int layer)
    {
        if (obj == null)
            return;
        
        obj.layer = layer;
        foreach (Transform child in obj.transform)
        {
            SetObjectLayerRecursively(child.gameObject, layer);
        }
    }

    /// <summary>
    /// 更新所有树木的可见性（根据距离主角的距离）
    /// </summary>
    private void UpdateTreeVisibility()
    {
        for (int i = _spawnedTrees.Count - 1; i >= 0; i--)
        {
            TreeData treeData = _spawnedTrees[i];
            
            // 树木已被销毁，移除数据
            if (treeData.treeObj == null)
            {
                _spawnedTrees.RemoveAt(i);
                continue;
            }

            // 计算树木到主角的距离（忽略Y轴，只算水平距离）
            Vector3 treePos = treeData.treeObj.transform.position;
            Vector3 playerPos = transform.position;
            float horizontalDistance = Vector3.Distance(
                new Vector3(treePos.x, 0, treePos.z), 
                new Vector3(playerPos.x, 0, playerPos.z)
            );

            // 距离超出可视范围 → 隐藏；否则 → 显示
            bool shouldVisible = horizontalDistance <= treeData.visibleDistance;
            SetTreeVisible(treeData.treeObj, shouldVisible);
        }
    }

    /// <summary>
    /// 设置树木的可见性（激活/禁用所有渲染器）
    /// </summary>
    private void SetTreeVisible(GameObject tree, bool visible)
    {
        Renderer[] renderers = tree.GetComponentsInChildren<Renderer>();
        foreach (var renderer in renderers)
        {
            renderer.enabled = visible;
        }
    }

    /// <summary>
    /// 生成随机射线起点（在目标区域水平范围内）
    /// </summary>
    private Vector3 GetRandomRayOrigin(TreeSpawnArea targetArea)
    {
        Vector3 areaCenter = targetArea.transform.position + Vector3.up * targetArea.centerYOffset;
        
        float randomAngle = Random.Range(0, Mathf.PI * 2);
        float randomRadius = Random.Range(0, targetArea.radius);
        float x = areaCenter.x + Mathf.Cos(randomAngle) * randomRadius;
        float z = areaCenter.z + Mathf.Sin(randomAngle) * randomRadius;
        float y = transform.position.y + targetArea.raycastHeight;
        
        return new Vector3(x, y, z);
    }

    /// <summary>
    /// 手动重置所有区域的生成状态（调试用）
    /// </summary>
    public void ResetAllSpawnAreas()
    {
        // 销毁所有已生成树木
        foreach (var treeData in _spawnedTrees)
        {
            if (treeData.treeObj != null)
                Destroy(treeData.treeObj);
        }
        _spawnedTrees.Clear();

        // 重置所有区域的生成状态
        foreach (var area in allSpawnAreas)
        {
            area.ResetSpawnState();
        }

        Debug.Log("所有区域树木已重置，可重新生成");
    }
}

/// <summary>
/// 树木区域标记（保留，用于后续扩展）
/// </summary>
public class TreeAreaMarker : MonoBehaviour
{
    public TreeSpawnArea associatedArea;
}