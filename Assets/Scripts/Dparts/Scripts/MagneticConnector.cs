using System.Collections;
using UnityEngine;

namespace Scraft
{
    
public class MagneticConnector : MonoBehaviour
{
    [Header("设置")]
    [Tooltip("磁吸生效的半径")]
    public float attractionRadius = 5f;
    
    [Tooltip("吸附力的大小")]
    public float magneticForce = 50f;

    [Tooltip("触发“锁定”连接的最小距离")]
    public float snapDistance = 0.5f;

    [Tooltip("磁吸目标的Layer（建议单独设置一个Magnet层）")]
    public LayerMask targetLayer;

    [Header("状态 (只读)")]
    public bool isConnected = false;
    public MagneticConnector connectedTarget;
    // 是否允许连接（用于断开后的冷却期）
    public bool canConnect = true;

    [Header("断开设置")]
    [Tooltip("断开时施加的推力大小（冲量）")]
    public float detachPushForce = 200f;

    [Tooltip("断开后多长时间内不会再次连接（秒）")]
    public float disconnectCooldown = 5f;

    private Rigidbody parentRb;
    private Coroutine cooldownCoroutine;


    void Start()
    {
        if (World.GameMode == World.GameMode_Freedom && enabled){
            // 获取自身最高层级或直接父级的Rigidbody
            parentRb = GetComponentInParent<Rigidbody>();

            if (parentRb == null)
            {
                Debug.LogError("错误：磁吸盘的父级链中找不到Rigidbody！");
                this.enabled = false;
            }
        }
        
    }

    void FixedUpdate()
    {
        if (World.GameMode == World.GameMode_Freedom && enabled && parentRb != null){
            // 如果已经连接，就不再进行磁吸计算
            if (isConnected) return;

            DetectAndAttract();
        }
    }

    void DetectAndAttract()
    {
        // 关键修改：如果处于冷却期，不进行任何吸引或吸附
        if (!canConnect) return;

        // 1. 检测周围的磁吸盘
        Collider[] colliders = Physics.OverlapSphere(transform.position, attractionRadius, targetLayer);

        foreach (var col in colliders)
        {
            // 跳过自己
            if (col.gameObject == this.gameObject) continue;

            // 尝试获取对方的脚本
            MagneticConnector otherMagnet = col.GetComponent<MagneticConnector>();

            // 确保对方也是磁吸盘，且对方也没有连接，并且对方也允许连接
            if (otherMagnet != null && !otherMagnet.isConnected && otherMagnet.canConnect)
            {
                // 2. 计算方向和距离
                Vector3 directionToTarget = otherMagnet.transform.position - transform.position;
                float distance = directionToTarget.magnitude;

                // 3. 判断是吸附还是吸引
                if (distance <= snapDistance)
                {
                    SnapTo(otherMagnet);
                }
                else
                {
                    // 归一化方向向量 * 力度
                    Vector3 force = directionToTarget.normalized * magneticForce;
                    
                    // 关键点：力是加在父物体的Rigidbody上的
                    parentRb.AddForce(force, ForceMode.Force);
                }

                // 简化逻辑：这里我们只处理找到的第一个有效目标
                return; 
            }
        }
    }

    // 执行物理吸附
    void SnapTo(MagneticConnector other)
    {
        // 再次检查确保双方都允许连接
        if (isConnected || !canConnect || !other.canConnect) return;

        // 1. 移动父物体，使两个磁吸盘位置重合
        // 计算父物体需要移动的偏移量
        Vector3 offset = other.transform.position - transform.position;
        parentRb.position += offset;

        // 2. (可选) 旋转对齐逻辑... (保持不变)

        // 3. 给被吸附物体添加SimulateParentFollow脚本
        // 获取对方的Rigidbody
        Rigidbody otherRb = other.parentRb;

        if (parentRb.gameObject.name == "3D MainSubmarine" && otherRb != null)
        {
            SimulateParentFollow followScript = otherRb.gameObject.AddComponent<SimulateParentFollow>();
            followScript.targetParent = transform;
        }
        
        // 设置双方状态
        this.isConnected = true;
        this.connectedTarget = other;
        other.isConnected = true; // 同时也标记对方已连接，防止对方继续吸附
        other.connectedTarget = this;

        Debug.Log("磁吸连接成功！");
    }

    // 也就是断开连接的方法（供外部调用）
    public void Detach()
    {
        if (!isConnected || connectedTarget == null) return;
        
        // --- 1. 获取推力所需的参数 ---
        // 记录对方的引用，以便在清除状态前操作
        MagneticConnector otherMagnet = connectedTarget;
        Rigidbody otherRb = otherMagnet.parentRb;
        
        // 计算推开的方向（从我方磁吸盘指向对方磁吸盘）
        Vector3 pushDirection = otherMagnet.transform.position - transform.position;

        // --- 2. 清理SimulateParentFollow脚本 ---
        if (otherRb != null)
        {
            Destroy(otherRb.gameObject.GetComponent<SimulateParentFollow>());
        }

        // --- 3. 施加反方向推力 ---
        // 使用 ForceMode.Impulse 施加瞬间的推力
        // 推力方向为 (对方 - 我方) 的单位向量
        if (otherRb != null)
        {
            otherRb.AddForce(pushDirection.normalized * detachPushForce, ForceMode.Impulse);
            Debug.Log($"对 {otherRb.gameObject.name} 施加了推力：{pushDirection.normalized * detachPushForce}");
        }
        
        // --- 4. 启动冷却计时器 ---
        // 双方都进入冷却期
        StartCoroutine(CooldownRoutine());
        otherMagnet.StartCoroutine(otherMagnet.CooldownRoutine());
        
        // --- 5. 重置双方状态 ---
        otherMagnet.isConnected = false;
        otherMagnet.connectedTarget = null;
        
        isConnected = false;
        connectedTarget = null;
    }
    
    /// <summary>
    /// 冷却协程：设置 canConnect 为 false，等待一段时间后恢复。
    /// </summary>
    IEnumerator CooldownRoutine()
    {
        canConnect = false;
        // 如果之前有冷却协程在运行，新的会覆盖旧的，但这里为了确保逻辑简单，直接运行即可
        
        Debug.Log(parentRb.gameObject.name + " 进入冷却期：" + disconnectCooldown + " 秒");
        yield return new WaitForSeconds(disconnectCooldown);
        
        canConnect = true;
        Debug.Log(parentRb.gameObject.name + " 冷却结束，允许重新连接");
    }

    // 辅助线：在Scene窗口显示范围
    void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, attractionRadius);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, snapDistance);
    }
}
}