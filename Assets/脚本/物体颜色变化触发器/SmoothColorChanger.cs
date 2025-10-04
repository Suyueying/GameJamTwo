using UnityEngine;
using System.Collections;

public class SmoothColorChanger : MonoBehaviour
{
    [Header("目标物体设置")]
    public GameObject targetObject;              // 要改变颜色的目标物体
    public bool changeSelf = false;              // 是否改变脚本所在物体（如果targetObject为空则生效）

    [Header("颜色改变设置")]
    public Color targetColor = Color.white;      // 目标颜色
    public float changeDuration = 2.0f;          // 颜色改变持续时间（秒）
    public bool revertOnExit = false;            // 离开时是否恢复原色
    public float revertDuration = 1.0f;          // 恢复原色的持续时间（秒）

    [Header("触发设置")]
    public string playerTag = "Player";          // 玩家标签

    private SpriteRenderer targetSpriteRenderer;
    private Color originalColor;
    private Coroutine colorChangeCoroutine;

    void Start()
    {
        // 确定目标物体
        GameObject finalTarget = targetObject;
        if (finalTarget == null && changeSelf)
        {
            finalTarget = gameObject;
        }

        if (finalTarget == null)
        {
            Debug.LogError("未指定目标物体！请设置TargetObject或勾选ChangeSelf。");
            return;
        }

        // 获取目标物体的SpriteRenderer组件
        targetSpriteRenderer = finalTarget.GetComponent<SpriteRenderer>();
        if (targetSpriteRenderer == null)
        {
            Debug.LogError($"目标物体 '{finalTarget.name}' 上未找到SpriteRenderer组件！");
            return;
        }

        // 保存原始颜色
        originalColor = targetSpriteRenderer.color;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // 检查进入的对象是否是玩家
        if (other.CompareTag(playerTag) && targetSpriteRenderer != null)
        {
            // 停止可能正在进行的颜色改变协程
            if (colorChangeCoroutine != null)
            {
                StopCoroutine(colorChangeCoroutine);
            }

            // 开始改变颜色到目标颜色
            colorChangeCoroutine = StartCoroutine(ChangeColor(targetColor, changeDuration));
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        // 检查离开的对象是否是玩家且需要恢复原色
        if (other.CompareTag(playerTag) && revertOnExit && targetSpriteRenderer != null)
        {
            // 停止可能正在进行的颜色改变协程
            if (colorChangeCoroutine != null)
            {
                StopCoroutine(colorChangeCoroutine);
            }

            // 开始恢复原始颜色
            colorChangeCoroutine = StartCoroutine(ChangeColor(originalColor, revertDuration));
        }
    }

    // 平滑改变颜色的协程
    IEnumerator ChangeColor(Color targetColor, float duration)
    {
        if (targetSpriteRenderer == null) yield break;

        Color startColor = targetSpriteRenderer.color;
        float timer = 0f;

        while (timer < duration)
        {
            timer += Time.deltaTime;
            float progress = timer / duration;
            
            // 使用Lerp平滑过渡颜色
            targetSpriteRenderer.color = Color.Lerp(startColor, targetColor, progress);
            
            yield return null;
        }

        // 确保最终颜色准确
        targetSpriteRenderer.color = targetColor;
        colorChangeCoroutine = null;
    }

    // 公共方法：设置目标物体
    public void SetTargetObject(GameObject newTarget)
    {
        if (newTarget != null)
        {
            targetObject = newTarget;
            SpriteRenderer sr = newTarget.GetComponent<SpriteRenderer>();
            if (sr != null)
            {
                targetSpriteRenderer = sr;
                originalColor = sr.color;
            }
            else
            {
                Debug.LogError($"新目标物体 '{newTarget.name}' 上未找到SpriteRenderer组件！");
            }
        }
    }

    // 公共方法：立即改变颜色（不平滑过渡）
    public void SetColorImmediately(Color newColor)
    {
        if (targetSpriteRenderer != null)
        {
            targetSpriteRenderer.color = newColor;
        }
    }

    // 公共方法：平滑改变颜色（可通过代码调用）
    public void ChangeColorSmoothly(Color newColor, float duration)
    {
        if (targetSpriteRenderer != null)
        {
            // 停止可能正在进行的颜色改变协程
            if (colorChangeCoroutine != null)
            {
                StopCoroutine(colorChangeCoroutine);
            }

            colorChangeCoroutine = StartCoroutine(ChangeColor(newColor, duration));
        }
    }

    // 公共方法：重置为原始颜色
    public void ResetToOriginalColor(float duration = 0f)
    {
        if (targetSpriteRenderer != null)
        {
            if (duration <= 0f)
            {
                // 立即重置
                SetColorImmediately(originalColor);
            }
            else
            {
                // 平滑重置
                ChangeColorSmoothly(originalColor, duration);
            }
        }
    }

    // 公共方法：获取当前目标物体
    public GameObject GetTargetObject()
    {
        return targetObject != null ? targetObject : (changeSelf ? gameObject : null);
    }

    // 在编辑器中可视化触发器范围（可选）
    void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null && collider.isTrigger)
        {
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position, collider.bounds.size);
        }
    }
}