using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// 物体超出相机指定距离后隐藏渲染（保留功能）
/// 支持两种隐藏方式：禁用渲染器 / 切换Layer
/// </summary>
public class HideWhenOutOfCameraRange : MonoBehaviour
{
    [Header("核心设置")]
    [Tooltip("超出该距离后隐藏")]
    public float distanceThreshold = 350f; // 距离阈值X
    [Tooltip("隐藏方式（推荐DisableRenderer，避免Layer冲突）")]
    public HideMethod hideMethod = HideMethod.DisableRenderer;

    [Header("Layer隐藏方式专用")]
    [Tooltip("用于隐藏的Layer（默认15，需确保相机不渲染该Layer）")]
    public int hiddenLayer = 15;

    [Header("距离计算选项")]
    [Tooltip("是否使用物体包围盒中心计算距离（更精准）")]
    public bool useBoundsCenter = true;

    // 隐藏方式枚举
    public enum HideMethod
    {
        DisableRenderer, // 禁用所有渲染器（推荐）
        SwitchLayer      // 切换到隐藏Layer
    }

    // 缓存数据
    private Camera _mainCamera;
    private List<Transform> _allTransforms = new List<Transform>(); // 所有物体（自身+子物体）
    private Dictionary<Transform, int> _originalLayers = new Dictionary<Transform, int>(); // 原始Layer记录
    private Dictionary<Renderer, bool> _originalRendererStates = new Dictionary<Renderer, bool>(); // 原始渲染器状态
    private bool _isCurrentlyHidden = false; // 当前是否隐藏

    private void Start()
    {
        // 初始化相机引用
        InitCameraReference();
        
        // 收集所有物体（自身+所有子物体，包括多级）
        CollectAllTransforms(transform);
        
        // 记录原始状态（根据选择的隐藏方式）
        RecordOriginalStates();
    }

    private void Update()
    {
        // 相机未找到时直接返回
        if (_mainCamera == null) return;

        // 计算物体到相机的距离
        float distanceToCamera = CalculateDistanceToCamera();

        // 判断是否需要切换隐藏状态
        bool needHide = distanceToCamera > distanceThreshold;
        if (needHide != _isCurrentlyHidden)
        {
            if (needHide)
                HideObject();
            else
                ShowObject();
            
            _isCurrentlyHidden = needHide;
        }
    }

    /// <summary>
    /// 初始化相机引用
    /// </summary>
    private void InitCameraReference()
    {
        _mainCamera = Camera.main;
        if (_mainCamera == null)
        {
            Debug.LogWarning($"[{nameof(HideWhenOutOfCameraRange)}] 未找到名为'MainCamera'的相机！脚本已禁用", this);
            enabled = false;
        }
        else if (hideMethod == HideMethod.SwitchLayer)
        {
            // 提示：确保相机不渲染隐藏Layer
            if (_mainCamera.cullingMask == (1 << hiddenLayer) || (_mainCamera.cullingMask & (1 << hiddenLayer)) != 0)
            {
                Debug.LogWarning($"[{nameof(HideWhenOutOfCameraRange)}] 相机当前渲染隐藏Layer({hiddenLayer})！请在相机的Culling Mask中取消勾选该Layer", this);
            }
        }
    }

    /// <summary>
    /// 递归收集所有物体（自身+子物体，包括多级）
    /// </summary>
    private void CollectAllTransforms(Transform root)
    {
        if (root == null) return;
        
        _allTransforms.Add(root);
        
        // 递归遍历子物体
        foreach (Transform child in root)
        {
            CollectAllTransforms(child);
        }
    }

    /// <summary>
    /// 记录物体原始状态（用于后续恢复）
    /// </summary>
    private void RecordOriginalStates()
    {
        switch (hideMethod)
        {
            case HideMethod.SwitchLayer:
                // 记录每个物体的原始Layer
                foreach (var trans in _allTransforms)
                {
                    if (!_originalLayers.ContainsKey(trans))
                    {
                        _originalLayers.Add(trans, trans.gameObject.layer);
                    }
                }
                break;
            
            case HideMethod.DisableRenderer:
                // 记录所有渲染器的原始启用状态
                foreach (var trans in _allTransforms)
                {
                    // 获取物体上所有渲染器（可能有多个，如SkinnedMeshRenderer+MeshRenderer）
                    Renderer[] renderers = trans.GetComponents<Renderer>();
                    foreach (var renderer in renderers)
                    {
                        if (renderer != null && !_originalRendererStates.ContainsKey(renderer))
                        {
                            _originalRendererStates.Add(renderer, renderer.enabled);
                        }
                    }
                }

                // 提示：如果没有找到任何渲染器
                if (_originalRendererStates.Count == 0)
                {
                    Debug.LogWarning($"[{nameof(HideWhenOutOfCameraRange)}] 物体及其子物体没有找到任何渲染器！禁用渲染器方式无效", this);
                }
                break;
        }
    }

    /// <summary>
    /// 计算物体到相机的距离
    /// </summary>
    private float CalculateDistanceToCamera()
    {
        Vector3 targetPosition = transform.position;
        
        // 如果启用包围盒中心计算，且物体有渲染器
        if (useBoundsCenter)
        {
            Renderer rootRenderer = GetComponent<Renderer>();
            if (rootRenderer != null && rootRenderer.enabled)
            {
                targetPosition = rootRenderer.bounds.center;
            }
            // 注：如果需要更精准的多级子物体包围盒，可扩展为计算所有渲染器的联合包围盒
        }

        return Vector3.Distance(targetPosition, _mainCamera.transform.position);
    }

    /// <summary>
    /// 隐藏物体（仅隐藏渲染，保留功能）
    /// </summary>
    private void HideObject()
    {
        switch (hideMethod)
        {
            case HideMethod.SwitchLayer:
                foreach (var trans in _allTransforms)
                {
                    trans.gameObject.layer = hiddenLayer;
                }
                break;
            
            case HideMethod.DisableRenderer:
                foreach (var renderer in _originalRendererStates.Keys)
                {
                    if (renderer != null)
                    {
                        renderer.enabled = false;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// 显示物体（恢复原始渲染状态）
    /// </summary>
    private void ShowObject()
    {
        switch (hideMethod)
        {
            case HideMethod.SwitchLayer:
                foreach (var kvp in _originalLayers)
                {
                    if (kvp.Key != null)
                    {
                        kvp.Key.gameObject.layer = kvp.Value;
                    }
                }
                break;
            
            case HideMethod.DisableRenderer:
                foreach (var kvp in _originalRendererStates)
                {
                    if (kvp.Key != null)
                    {
                        kvp.Key.enabled = kvp.Value;
                    }
                }
                break;
        }
    }

    /// <summary>
    /// Gizmos绘制距离范围（编辑器中可视化）
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, distanceThreshold);
        
        // 如果启用包围盒中心，绘制包围盒中心
        if (useBoundsCenter)
        {
            Renderer rootRenderer = GetComponent<Renderer>();
            if (rootRenderer != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(rootRenderer.bounds.center, 0.1f);
            }
        }
    }
}