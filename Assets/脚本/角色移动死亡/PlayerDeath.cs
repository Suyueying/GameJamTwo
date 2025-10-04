// PlayerDeath.cs - 依靠触发器触发死亡的版本
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeath : MonoBehaviour
{
    private bool isDead = false;
    public Rigidbody2D rb;
    public SpriteRenderer spriteRenderer;
    public Transform initialPositionPoint;

    // 声明死亡事件
    public static System.Action OnPlayerDied;

    // 声明重生事件
    public static System.Action OnPlayerRespawned;

    void Update()
    {
        if (isDead)
        {
            Respawn();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // 检查是否进入死亡区域
        if (other.CompareTag("DeathZone"))
        {
            Die();
        }
    }

    private void Die()
    {
        if (!isDead)
        {
            isDead = true;
            rb.velocity = Vector2.zero; // 停止速度
            rb.angularVelocity = 0;    // 停止旋转
            spriteRenderer.enabled = false; // 隐藏角色

            // 触发死亡事件
            OnPlayerDied?.Invoke(); // 使用空合并运算符简化调用
        }
    }

    public void Respawn()
    {
        if (isDead)
        {
            isDead = false;
            spriteRenderer.enabled = true;
            transform.position = initialPositionPoint.position; // 返回初始位置

            // 触发重生事件
            OnPlayerRespawned?.Invoke(); // 使用空合并运算符简化调用
        }
    }
}