using UnityEngine;

public class ParticleEmissionTrigger : MonoBehaviour
{
    [Header("粒子系统设置")]
    public ParticleSystem targetParticleSystem;  // 目标粒子系统
    public float newEmissionRate = 50f;          // 新的发射速率

    [Header("触发设置")]
    public string triggerTag = "Player";         // 触发器的标签
    public bool resetOnExit = false;             // 离开时是否重置
    public float resetDelay = 0f;                // 重置延迟时间

    [Header("原始值保存")]
    public float originalEmissionRate;           // 原始发射速率

    private bool hasBeenTriggered = false;

    void Start()
    {
        // 保存原始发射速率
        if (targetParticleSystem != null)
        {
            var emission = targetParticleSystem.emission;
            originalEmissionRate = emission.rateOverTime.constant;

            Debug.Log($"粒子触发器初始化 - 原始发射速率: {originalEmissionRate}");
        }
        else
        {
            Debug.LogWarning("目标粒子系统未设置！");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查标签
        if (!other.CompareTag(triggerTag)) return;

        // 检查粒子系统
        if (targetParticleSystem == null) return;

        if (!hasBeenTriggered)
        {
            ChangeParticleEmission(newEmissionRate);
            hasBeenTriggered = true;

            Debug.Log($"触发粒子发射速率改变: {newEmissionRate}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // 检查标签
        if (!other.CompareTag(triggerTag)) return;

        // 检查是否需要重置
        if (resetOnExit && targetParticleSystem != null)
        {
            if (resetDelay > 0)
            {
                Invoke("ResetParticleEmission", resetDelay);
            }
            else
            {
                ResetParticleEmission();
            }

            hasBeenTriggered = false;
        }
    }

    // 改变粒子发射速率
    public void ChangeParticleEmission(float emissionRate)
    {
        if (targetParticleSystem == null) return;

        var emission = targetParticleSystem.emission;
        emission.rateOverTime = emissionRate;

        Debug.Log($"粒子发射速率已改为: {emissionRate}");
    }

    // 重置粒子发射速率
    public void ResetParticleEmission()
    {
        if (targetParticleSystem == null) return;

        var emission = targetParticleSystem.emission;
        emission.rateOverTime = originalEmissionRate;

        Debug.Log($"粒子发射速率已重置为: {originalEmissionRate}");
    }

    // 手动触发改变发射速率
    public void TriggerEmissionChange()
    {
        ChangeParticleEmission(newEmissionRate);
    }

    // 手动触发重置
    public void TriggerReset()
    {
        ResetParticleEmission();
    }

    // 设置新的发射速率
    public void SetNewEmissionRate(float rate)
    {
        newEmissionRate = rate;
    }

    // 可视化调试
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.3f);

        BoxCollider2D boxCollider = GetComponent<BoxCollider2D>();
        CircleCollider2D circleCollider = GetComponent<CircleCollider2D>();

        if (boxCollider != null)
        {
            Gizmos.DrawCube(transform.position + (Vector3)boxCollider.offset, boxCollider.size);
        }
        else if (circleCollider != null)
        {
            Gizmos.DrawSphere(transform.position + (Vector3)circleCollider.offset, circleCollider.radius);
        }

        // 绘制到粒子系统的连线
        if (targetParticleSystem != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetParticleSystem.transform.position);
        }
    }
}