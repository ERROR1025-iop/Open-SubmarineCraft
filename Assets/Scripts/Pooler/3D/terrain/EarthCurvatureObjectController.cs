using UnityEngine;

[RequireComponent(typeof(Transform))]
public class EarthCurvatureObjectController : MonoBehaviour
{
    // 核心下沉参数（直接写死，无需编辑器配置）
    private const float NoDropRange = 300.0f;   // 主角周围不下沉范围
    private const float SinkRadius = 18.0f;   // 下沉效果总范围
    private const float MaxDrop = 2.0f;       // 最大下沉量
    private const float SinkPower = 1.0f;     // 下沉曲线陡峭度

    private Transform _submarineTransform;    // 主角（3D MainSubmarine）引用
    private float _originalLocalY;            // 物体初始局部Y坐标（避免叠加偏移）

    void Start()
    {
        // 1. 查找主角（固定name：3D MainSubmarine）
        GameObject submarine = GameObject.Find("MainCamera");
        if (submarine != null)
        {
            _submarineTransform = submarine.transform;
        }
        else
        {
            Debug.LogError($"物体 {gameObject.name} 未找到主角「3D MainSubmarine」！脚本失效。");
            enabled = false; // 找不到主角则禁用脚本
        }

        // 2. 记录物体初始局部Y坐标（基于初始位置计算下沉，避免偏移叠加）
        _originalLocalY = transform.localPosition.y;
    }

    public void ResetOriginalY()
    {
        _originalLocalY = transform.localPosition.y;
    }

    void Update()
    {
        if (_submarineTransform == null) return;

        // 3. 计算物体到主角的XZ平面距离（忽略Y轴高度差）
        Vector3 objectWorldPos = transform.position;
        Vector3 submarinePos = _submarineTransform.position;
        float dx = objectWorldPos.x - submarinePos.x;
        float dz = objectWorldPos.z - submarinePos.z;
        float distToSubmarine = Mathf.Sqrt(dx * dx + dz * dz);

        // 4. 计算下沉量（与地形/水面逻辑一致）
        float sinkAmount = 0.0f;
        if (distToSubmarine > NoDropRange)
        {
            float effectiveDist = distToSubmarine - NoDropRange;
            effectiveDist = Mathf.Max(effectiveDist, 0.0f); // 避免负数异常
            float sinkT = Mathf.Pow(effectiveDist / SinkRadius, SinkPower);
            sinkAmount = sinkT * MaxDrop;
        }

        // 5. 更新物体位置（基于初始Y坐标，确保每次计算准确）
        Vector3 newLocalPos = transform.localPosition;
        newLocalPos.y = _originalLocalY - sinkAmount;
        transform.localPosition = newLocalPos;
    }
}