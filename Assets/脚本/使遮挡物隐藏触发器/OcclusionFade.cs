using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(SpriteRenderer))]
public class OcclusionFade : MonoBehaviour
{
    [Header("Fade Settings")]
    [Tooltip("渐隐的目标透明度")]
    [Range(0f, 1f)] public float fadeAlpha = 0.3f;
    [Tooltip("渐隐速度")]
    public float fadeSpeed = 5f;
    [Tooltip("检测半径")]
    public float checkRadius = 1f;
    [Tooltip("检测层级")]
    public LayerMask occlusionLayer;

    private SpriteRenderer spriteRenderer;
    private float originalAlpha;
    private List<Transform> overlappingObjects = new List<Transform>();
    private Dictionary<Transform, SpriteRenderer> overlappingRenderers = new Dictionary<Transform, SpriteRenderer>();

    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        originalAlpha = spriteRenderer.color.a;
    }

    private void Update()
    {
        CheckForOcclusion();
        UpdateFade();
    }

    private void CheckForOcclusion()
    {
        // 清除上一帧的检测结果
        overlappingObjects.Clear();

        // 检测所有可能遮挡此物体的碰撞体
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, checkRadius, occlusionLayer);
        
        foreach (Collider2D hit in hits)
        {
            if (hit.transform != transform && !overlappingObjects.Contains(hit.transform))
            {
                overlappingObjects.Add(hit.transform);
                
                // 缓存SpriteRenderer
                if (!overlappingRenderers.ContainsKey(hit.transform))
                {
                    SpriteRenderer renderer = hit.GetComponent<SpriteRenderer>();
                    if (renderer != null)
                    {
                        overlappingRenderers[hit.transform] = renderer;
                    }
                }
            }
        }
    }

    private void UpdateFade()
    {
        // 更新所有被遮挡物体的透明度
        foreach (var pair in overlappingRenderers)
        {
            if (pair.Value != null)
            {
                Color newColor = pair.Value.color;
                
                // 如果物体在当前帧被检测为遮挡物
                if (overlappingObjects.Contains(pair.Key))
                {
                    newColor.a = Mathf.Lerp(newColor.a, fadeAlpha, fadeSpeed * Time.deltaTime);
                }
                else
                {
                    newColor.a = Mathf.Lerp(newColor.a, 1f, fadeSpeed * Time.deltaTime);
                }
                
                pair.Value.color = newColor;
            }
        }

        // 清理字典中已销毁的对象
        List<Transform> toRemove = new List<Transform>();
        foreach (var pair in overlappingRenderers)
        {
            if (pair.Value == null)
            {
                toRemove.Add(pair.Key);
            }
        }
        
        foreach (var key in toRemove)
        {
            overlappingRenderers.Remove(key);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}