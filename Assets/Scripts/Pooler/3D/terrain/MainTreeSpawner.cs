using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 在主角周围按深度层动态生成对象并使用对象池复用。
/// - 在面板中配置：生成范围、每层深度区间（6层）、每层可选的prefab列表、生成对象缩放、每层目标数量等。
/// - 通过从Y=0向下发射射线检测地形（Layer=10）来确定生成位置。
/// - 使用对象池：离开范围的对象会被回收，生成时优先复用。
/// </summary>
public class MainTreeSpawner : MonoBehaviour
{
    [Header("核心设置")]
    [Tooltip("在主角周围的生成半径（单位：米）")]
    public float spawnRange = 100f;

    [Tooltip("生成最近距离（扇形最近半径），小于该距离的点不会生成（单位：米）")]
    public float minDist = 60f;

    [Tooltip("地形Layer（必须设置为10）")]
    public LayerMask terrainLayer = 1 << 10;

    [Tooltip("每帧最大生成数量（用于防止卡顿）")]
    [Range(1, 500)]
    public int maxPerFrame = 50;

    [Tooltip("每层目标物体数量（数组，长度6，对应层0~5；如果长度不足会使用默认值100）")]
    public int[] targetPerLayer = new int[6] { 100, 100, 100, 100, 100, 100 };

    [Tooltip("生成对象总体缩放（乘到Prefab的localScale上）")]
    public float objectScale = 1f;

    [Tooltip("距离检测/回收检查间隔（秒）")]
    public float distanceCheckInterval = 0.5f;

    [Header("层配置（6层：0~5）")]
    public LayerConfig[] layers = new LayerConfig[6];

    [Header("调试")]
    [Tooltip("在编辑器中可视化射线（短时间）")]
    public bool showRayDebug = true;

    // 池与活动对象管理
    private Dictionary<GameObject, Queue<GameObject>> _pool = new Dictionary<GameObject, Queue<GameObject>>();
    private List<SpawnedData> _active = new List<SpawnedData>();

    // 缓存射线击中数组
    private RaycastHit[] _raycastHits = new RaycastHit[100];

    private float _distanceCheckTimer = 0f;
    private Transform _runtimeGen;

    private Transform _playerTransform;

    private bool _initialSpawnDone = false;

    private List<TreeSpawnArea> allSpawnAreas;

    [System.Serializable]
    public class LayerConfig
    {
        [Tooltip("深度范围：Y轴的区间（minY, maxY），例如 -10 到 -1）")]
        public Vector2 depthRange = new Vector2(-10f, -1f);
        [Tooltip("该层可生成的prefab列表（从中随机选择），每项可单独设置缩放")]
        public List<PrefabEntry> prefabs = new List<PrefabEntry>();
    }

    [System.Serializable]
    public class PrefabEntry
    {
        [Tooltip("要生成的Prefab")]
        public GameObject prefab;

        [Tooltip("此Prefab在生成时额外乘的缩放系数（相对于Prefab原始localScale）")]
        public float scale = 1f;
    }

    private class SpawnedData
    {
        public GameObject obj;
        public int layerIndex;
        public GameObject prefab; // 原始prefab参考，用于回收入对应池
        public float prefabScale; // 存储该实例对应的prefab scale
    }

    private void Awake()
    {
        allSpawnAreas = new List<TreeSpawnArea>(FindObjectsOfType<TreeSpawnArea>());

        // 初始化每层（确保存在6个配置）
        if (layers == null || layers.Length != 6)
        {
            var tmp = new LayerConfig[6];
            for (int i = 0; i < 6; i++) tmp[i] = new LayerConfig();
            layers = tmp;
        }

            // 初始化每层目标数量，确保数组长度为6并填充默认值（保留已设置的值）
            if (targetPerLayer == null || targetPerLayer.Length != 6)
            {
                var tmpTargets = new int[6];
                for (int i = 0; i < 6; i++) tmpTargets[i] = 100;
                if (targetPerLayer != null)
                {
                    int copyLen = Mathf.Min(6, targetPerLayer.Length);
                    for (int i = 0; i < copyLen; i++) tmpTargets[i] = targetPerLayer[i];
                }
                targetPerLayer = tmpTargets;
            }

        // 查找runtime_gen节点（用于组织运行时生成的对象）
        var rg = GameObject.Find("runtime_gen");
        if (rg != null) _runtimeGen = rg.transform;

        // 尝试获取主角Transform：优先查找名为 "MainSubmarine" 的组件（避免编译期类型依赖），若找不到则回退到当前对象
        _playerTransform = transform;
    }

    private void Start()
    {
        StartCoroutine(SpawnLoop());
    }

    private IEnumerator SpawnLoop()
    {
        // 持续进行采样生成
        while (true)
        {
            int spawnedThisFrame = 0;

            for (int layerIdx = 0; layerIdx < layers.Length; layerIdx++)
            {
                // 统计当前该层的数量
                int currentCount = CountActiveInLayer(layerIdx);
                int desired = GetTargetForLayer(layerIdx);
                int need = Mathf.Max(0, desired - currentCount);

                if (currentCount == desired){
                    _initialSpawnDone = true;
                }

                for (int i = 0; i < need; i++)
                {
                    if (spawnedThisFrame >= maxPerFrame)
                    {
                        spawnedThisFrame = 0;
                        yield return null; // 分帧
                    }

                    // 采样随机点
                    float dist = 0f;
                    Vector3 origin;
                    Vector3 playerXZ = new Vector3(_playerTransform.position.x, 0f, _playerTransform.position.z);
                    do{
                        Vector2 rnd = Random.insideUnitCircle * spawnRange;
                        origin = new Vector3(_playerTransform.position.x + rnd.x, 0f, _playerTransform.position.z + rnd.y);                    
                        dist = Vector3.Distance(new Vector3(origin.x, 0f, origin.z), playerXZ);       
                        if (!_initialSpawnDone){
                            break;
                        }
                    }while(dist < minDist);


                    int hits = Physics.RaycastNonAlloc(origin, Vector3.down, _raycastHits, Mathf.Infinity, terrainLayer);
                    if (hits <= 0)
                    {
#if UNITY_EDITOR
                        if (showRayDebug) Debug.DrawRay(origin, Vector3.down * 50f, Color.red, 1f);
#endif
                        continue;
                    }

                    // 从击中点中选择最上面的（y最大）
                    RaycastHit best = _raycastHits[0];
                    for (int h = 1; h < hits; h++)
                    {
                        if (_raycastHits[h].point.y > best.point.y) best = _raycastHits[h];
                    }

                    // 清空缓存项
                    for (int h = 0; h < hits; h++) _raycastHits[h] = new RaycastHit();

                    float y = best.point.y;
                    Vector2 depthRange = layers[layerIdx].depthRange;
                    // depthRange 表示 minY, maxY（例如 -10 到 -1）
                    float minY = Mathf.Min(depthRange.x, depthRange.y);
                    float maxY = Mathf.Max(depthRange.x, depthRange.y);

                    if (y < minY || y > maxY)
                    {
                        // 不在该层深度范围内
                        continue;
                    }

                    if(isInSpawnArea(best.point))
                    {
                        yield return null;
                        continue;
                    }
                    
                    // 从该层的prefab列表中随机选取（支持每项独立缩放）
                    var list = layers[layerIdx].prefabs;
                    if (list == null || list.Count == 0) continue;
                    PrefabEntry entry = list[Random.Range(0, list.Count)];
                    if (entry == null || entry.prefab == null) continue;
                    GameObject prefab = entry.prefab;
                    float prefabScale = entry.scale;

                    // 从池或实例化
                    GameObject go = GetFromPool(prefab);
                    go.transform.position = best.point;
                    go.transform.rotation = Quaternion.Euler(0f, Random.Range(0f, 360f), 0f);
                    // 应用缩放（乘到原始localScale上），支持全局objectScale与每-prefab scale相乘
                    go.transform.localScale = Vector3.Scale(prefab.transform.localScale, Vector3.one * objectScale * prefabScale);
                    go.layer =  go.name.Contains("Rock")? 10: 12; // 岩石放地形层，树木放默认层       
                    if (go.name.Contains("Rock"))
                    {
                        go.tag = "terrain";
                    }             
                    if (_runtimeGen != null)
                        go.transform.SetParent(_runtimeGen, true);

                    Vector3 goXY = new Vector3(go.transform.position.x, 0f, go.transform.position.z);
                    dist = Vector3.Distance(goXY, playerXZ); 
                    // if(dist < minDist){
                    //     Debug.Log("_initial：" + _initialSpawnDone + ", 距离：" + dist);
                    // }
                    

                    // 激活并记录
                    SetRenderersEnabled(go, true);
                    go.SetActive(true);

                    _active.Add(new SpawnedData { obj = go, layerIndex = layerIdx, prefab = prefab, prefabScale = prefabScale });

                    spawnedThisFrame++;
                }
            }

            yield return null;
        }
    }

    private bool isInSpawnArea(Vector3 pos)
    {
        foreach (var area in allSpawnAreas)
        {
            if (area != null && area.IsPointInArea(pos))
            {
                return true;
            }
        }
        return false;
    }

    private void Update()
    {
        _distanceCheckTimer += Time.deltaTime;
        if (_distanceCheckTimer >= distanceCheckInterval)
        {
            _distanceCheckTimer = 0f;
            RecycleFarObjects();
        }
    }

    private void RecycleFarObjects()
    {
        Vector3 playerPos = _playerTransform.position;
        float margin = 10f; // 回收余量

        for (int i = _active.Count - 1; i >= 0; i--)
        {
            var d = _active[i];
            if (d == null || d.obj == null)
            {
                _active.RemoveAt(i);
                continue;
            }

            Vector3 pos = d.obj.transform.position;
            // 使用水平距离判断
            float horizontalDistance = Vector3.Distance(new Vector3(pos.x, 0f, pos.z), new Vector3(playerPos.x, 0f, playerPos.z));

            if (horizontalDistance > spawnRange + margin)
            {
                // 回收
                ReturnToPool(d.prefab, d.obj);
                _active.RemoveAt(i);
            }
        }
    }

    private int CountActiveInLayer(int layerIdx)
    {
        int c = 0;
        for (int i = 0; i < _active.Count; i++) if (_active[i] != null && _active[i].layerIndex == layerIdx && _active[i].obj != null) c++;
        return c;
    }

    private int GetTargetForLayer(int layerIdx)
    {
        if (targetPerLayer != null && layerIdx >= 0 && layerIdx < targetPerLayer.Length)
            return targetPerLayer[layerIdx];
        return 100;
    }

    private GameObject GetFromPool(GameObject prefab)
    {
        if (prefab == null) return null;

        Queue<GameObject> q;
        if (_pool.TryGetValue(prefab, out q) && q.Count > 0)
        {
            var go = q.Dequeue();
            return go;
        }

        // 无可复用，实例化新对象（初始设为inactive）
        var newObj = Instantiate(prefab);
        newObj.SetActive(false);
        return newObj;
    }

    private void ReturnToPool(GameObject prefab, GameObject instance)
    {
        if (instance == null) return;

        SetRenderersEnabled(instance, false);
        instance.SetActive(false);

        if (!_pool.TryGetValue(prefab, out var q))
        {
            q = new Queue<GameObject>();
            _pool[prefab] = q;
        }
        q.Enqueue(instance);
    }

    private void SetRenderersEnabled(GameObject go, bool enabled)
    {
        if (go == null) return;
        var rends = go.GetComponentsInChildren<Renderer>(true);
        for (int i = 0; i < rends.Length; i++) rends[i].enabled = enabled;
    }

    /// <summary>
    /// 手动清空所有生成并回收到池中（调试/重置用）
    /// </summary>
    public void ResetAll()
    {
        for (int i = _active.Count - 1; i >= 0; i--)
        {
            var d = _active[i];
            if (d != null && d.obj != null)
            {
                ReturnToPool(d.prefab, d.obj);
            }
        }
        _active.Clear();
    }
}
