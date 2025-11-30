using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(Transform))]
public class EarthCurvatureWaterController : MonoBehaviour
{
    // 目标水面Shader名称（必须与你修改后的水面Shader名称完全一致）
    [Header("配置")]
    public string targetWaterShaderName = "Lux Water/WaterSurface Tessellation";

    // 缓存所有找到的水面材质（避免每帧重复查找，优化性能）
    private List<Material> waterMaterials = new List<Material>();
    // Shader中_PlayerPos的属性ID（提前缓存，减少性能开销）
    private int playerPosPropertyID;

    void Start()
    {
        // 1. 初始化_PlayerPos的属性ID（与Shader中变量名严格一致，大小写敏感）
        playerPosPropertyID = Shader.PropertyToID("_PlayerPos");

        // 2. 首次查找所有水面材质并缓存
        FindAllWaterMaterials();

        // 3. 校验结果
        if (waterMaterials.Count == 0)
            Debug.LogWarning($"未找到使用Shader「{targetWaterShaderName}」的水面材质，请检查Shader名称是否正确！");
        else
            Debug.Log($"成功找到 {waterMaterials.Count} 个水面材质，已开始同步主角位置。");
    }

    void Update()
    {
        // 实时获取主角的世界坐标（w=1，匹配Shader中float4类型）
        Vector4 playerWorldPos = new Vector4(
            transform.position.x,
            transform.position.y,
            transform.position.z,
            1.0f // 必须设为1，确保Shader中空间变换正确
        );

        // 同步坐标到所有缓存的水面材质
        SyncPlayerPosToMaterials(playerWorldPos);
    }

    // 查找场景中所有使用目标Shader的水面材质并缓存
    private void FindAllWaterMaterials()
    {
        waterMaterials.Clear(); // 清空原有缓存

        // 查找场景中所有启用的渲染器（包含所有带材质的对象）
        Renderer[] allRenderers = FindObjectsOfType<Renderer>(includeInactive: false);

        foreach (var renderer in allRenderers)
        {
            // 跳过没有材质的渲染器
            if (renderer.materials == null || renderer.materials.Length == 0)
                continue;

            foreach (var mat in renderer.materials)
            {
                // 匹配目标水面Shader（名称完全一致才生效）
                if (mat.shader != null && mat.shader.name == targetWaterShaderName)
                {
                    // 避免重复添加同一材质（比如多个水面共用一个材质实例）
                    if (!waterMaterials.Contains(mat))
                    {
                        waterMaterials.Add(mat);
                    }
                }
            }
        }
    }

    // 将主角位置同步到所有缓存的水面材质
    private void SyncPlayerPosToMaterials(Vector4 playerPos)
    {
        foreach (var mat in waterMaterials)
        {
            // 安全校验：材质未被销毁且Shader匹配
            if (mat != null && mat.shader != null && mat.shader.name == targetWaterShaderName)
            {
                mat.SetVector(playerPosPropertyID, playerPos);
            }
        }
    }

    // 可选：手动刷新材质列表（比如场景动态生成水面后调用）
    [ContextMenu("刷新水面材质列表")]
    public void RefreshWaterMaterialList()
    {
        FindAllWaterMaterials();
        Debug.Log($"已刷新水面材质列表，当前共 {waterMaterials.Count} 个水面材质。");
    }

    // 可选：在编辑器模式下预览材质列表（方便调试）
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, 1.0f); // 绘制主角位置标记
        Gizmos.color = Color.white;
    }
}