using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FadeObjectTrigger : MonoBehaviour
{
    [Header("渐隐渐显设置")]
    public GameObject targetObject; // 需要渐隐渐显的UI物体
    public float fadeDuration = 1.0f; // 渐隐渐显持续时间
    
    private Image imageComponent; // 改为 Image 组件
    private Coroutine fadeCoroutine;
    private bool isPlayerInTrigger = false;
    
    void Start()
    {
        // 获取目标物体的 Image 组件
        if (targetObject != null)
        {
            imageComponent = targetObject.GetComponent<Image>();
            
            // 初始状态设置为透明
            if (imageComponent != null)
            {
                SetAlpha(0f);
                targetObject.SetActive(true);
            }
            else
            {
                Debug.LogError("目标物体上没有找到 Image 组件！");
            }
        }
        else
        {
            Debug.LogError("请指定需要渐隐渐显的目标物体！");
        }
    }
    
    // 玩家进入触发器
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = true;
            
            // 停止当前的渐隐渐显协程
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            
            // 开始渐显
            fadeCoroutine = StartCoroutine(FadeTo(1f));
        }
    }
    
    // 玩家离开触发器
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInTrigger = false;
            
            // 停止当前的渐隐渐显协程
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            
            // 开始渐隐
            fadeCoroutine = StartCoroutine(FadeTo(0f));
        }
    }
    
    // 玩家在触发器内停留
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPlayerInTrigger)
        {
            isPlayerInTrigger = true;
        }
    }
    
    // 渐隐渐显协程
    private IEnumerator FadeTo(float targetAlpha)
    {
        if (imageComponent == null) yield break;
        
        Color currentColor = imageComponent.color;
        float startAlpha = currentColor.a;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, elapsedTime / fadeDuration);
            SetAlpha(newAlpha);
            yield return null;
        }
        
        // 确保最终达到目标透明度
        SetAlpha(targetAlpha);
    }
    
    // 设置透明度
    private void SetAlpha(float alpha)
    {
        if (imageComponent != null)
        {
            Color newColor = imageComponent.color;
            newColor.a = alpha;
            imageComponent.color = newColor;
        }
    }
    
    // 在编辑器中可视化触发器范围
    private void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position, collider.bounds.size);
        }
    }
}