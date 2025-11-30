using UnityEngine;

[ExecuteAlways]
public class EarthCurvatureController : MonoBehaviour
{
    [Tooltip("主角 Transform，会在每帧将位置写入全局着色器参数 `_PlayerPos`。")]
    public Transform player;

    [Tooltip("全局着色器属性名，默认 `_PlayerPos`。如果你的 shader 使用其他名字可修改。")]
    public string globalPropertyName = "_PlayerPos";

    // 缓存属性 ID，避免每次调用时进行字符串查找
    int globalPropertyID = -1;

    void Awake()
    {
        // 初始化缓存
        globalPropertyID = Shader.PropertyToID(globalPropertyName);
    }
    
    // 使用 LateUpdate 以确保在渲染前更新（例如角色由物理或动画系统移动）
    void LateUpdate()
    {
        if (player != null)
        {
            Vector3 p = player.position;
            // 将位置作为 Vector4 传入（w 分量设为 1），使用缓存的 property ID 写入全局变量
            if (globalPropertyID == -1)
            {
                globalPropertyID = Shader.PropertyToID(globalPropertyName);
            }
            Shader.SetGlobalVector(globalPropertyID, new Vector4(p.x, p.y, p.z, 1f));
        }
    }

    void OnDisable()
    {
        // 组件被禁用或销毁时清除全局变量（可选），使用缓存 ID，如果未初始化则先获取
        if (globalPropertyID == -1)
        {
            globalPropertyID = Shader.PropertyToID(globalPropertyName);
        }
        Shader.SetGlobalVector(globalPropertyID, Vector4.zero);
    }

}