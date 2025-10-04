using UnityEngine;
using TMPro;
using System.Collections;

public class TMProFadeTrigger : MonoBehaviour
{
    [Header("Text Mesh Pro 设置")]
    public TextMeshProUGUI targetText;      // Text Mesh Pro 文本组件
    public string displayText = "这里是显示的文字内容"; // 要显示的文本

    [Header("渐隐渐显设置")]
    public float fadeInDuration = 1f;       // 渐显时间
    public float fadeOutDuration = 1f;      // 渐隐时间
    public bool useTypewriterEffect = false; // 是否使用打字机效果
    public float typewriterSpeed = 20f;     // 打字速度（字符/秒）

    [Header("触发设置")]
    public bool requirePlayerTag = true;    // 是否需要玩家标签
    public bool showDebugLogs = true;       // 显示调试信息

    private Coroutine fadeCoroutine;
    private Coroutine typewriterCoroutine;
    private bool isPlayerInTrigger = false;
    private bool isTextVisible = false;

    void Start()
    {
        // 初始化文本状态
        InitializeText();
    }

    void Update()
    {
        // 调试用：按T键测试显示，按Y键测试隐藏
        if (Input.GetKeyDown(KeyCode.T))
        {
            ShowText();
        }
        if (Input.GetKeyDown(KeyCode.Y))
        {
            HideText();
        }
    }

    private void InitializeText()
    {
        if (targetText != null)
        {
            targetText.alpha = 0f; // 开始时完全透明
            targetText.text = "";  // 清空文本
        }

        if (showDebugLogs)
        {
            Debug.Log($"文本渐隐触发器初始化 - 目标文本: {(targetText != null ? targetText.name : "未设置")}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ShouldTrigger(other)) return;

        isPlayerInTrigger = true;

        if (showDebugLogs)
            Debug.Log("玩家进入触发器，开始渐显文本");

        ShowText();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!ShouldTrigger(other)) return;

        // 确保玩家在触发器内时文本保持显示状态
        if (!isTextVisible && isPlayerInTrigger)
        {
            if (showDebugLogs)
                Debug.Log("玩家仍在触发器内，确保文本显示");

            ShowText();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!ShouldTrigger(other)) return;

        isPlayerInTrigger = false;

        if (showDebugLogs)
            Debug.Log("玩家离开触发器，开始渐隐文本");

        HideText();
    }

    private bool ShouldTrigger(Collider2D other)
    {
        if (requirePlayerTag && !other.CompareTag("Player"))
            return false;

        if (targetText == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("目标Text Mesh Pro文本未设置！");
            return false;
        }

        return true;
    }

    // 显示文本（渐显）
    public void ShowText()
    {
        if (targetText == null) return;

        // 如果文本已经可见，不再重复显示
        if (isTextVisible && targetText.alpha >= 0.99f)
            return;

        // 停止之前的渐隐协程
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // 设置文本内容
        targetText.text = displayText;
        targetText.alpha = Mathf.Max(targetText.alpha, 0.01f); // 确保至少有一点透明度

        // 开始渐显
        fadeCoroutine = StartCoroutine(FadeText(targetText.alpha, 1f, fadeInDuration, true));
    }

    // 隐藏文本（渐隐）
    public void HideText()
    {
        if (targetText == null || targetText.alpha <= 0f || !isTextVisible)
            return;

        // 如果玩家还在触发器内，不隐藏文本
        if (isPlayerInTrigger)
        {
            if (showDebugLogs)
                Debug.Log("玩家仍在触发器内，取消隐藏");
            return;
        }

        // 停止之前的协程
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        // 开始渐隐
        fadeCoroutine = StartCoroutine(FadeText(targetText.alpha, 0f, fadeOutDuration, false));
    }

    // 文本渐隐渐显协程
    private IEnumerator FadeText(float startAlpha, float targetAlpha, float duration, bool isFadeIn)
    {
        isTextVisible = isFadeIn;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // 如果在渐隐过程中玩家又进入了触发器，停止渐隐
            if (!isFadeIn && isPlayerInTrigger)
            {
                if (showDebugLogs)
                    Debug.Log("渐隐过程中玩家进入触发器，停止渐隐");
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            targetText.alpha = newAlpha;

            yield return null;
        }

        // 确保最终alpha值准确
        targetText.alpha = targetAlpha;

        // 如果是渐显完成且需要使用打字机效果
        if (isFadeIn && useTypewriterEffect && targetAlpha > 0)
        {
            typewriterCoroutine = StartCoroutine(TypewriterEffect());
        }

        if (showDebugLogs)
        {
            if (isFadeIn)
                Debug.Log("文本渐显完成");
            else
                Debug.Log("文本渐隐完成");
        }
    }

    // 打字机效果协程
    private IEnumerator TypewriterEffect()
    {
        string fullText = targetText.text;
        targetText.text = fullText;
        targetText.maxVisibleCharacters = 0;

        int totalCharacters = fullText.Length;
        float delayPerCharacter = 1f / typewriterSpeed;

        for (int i = 0; i <= totalCharacters; i++)
        {
            if (!isTextVisible || !isPlayerInTrigger) break; // 如果文本被隐藏或玩家离开，停止打字效果

            targetText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delayPerCharacter);
        }

        // 确保完全显示
        if (isTextVisible && isPlayerInTrigger)
        {
            targetText.maxVisibleCharacters = totalCharacters;
        }
    }

    // 立即显示文本（无渐隐效果）
    public void ShowTextImmediate()
    {
        if (targetText == null) return;

        // 停止所有协程
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);

        targetText.text = displayText;
        targetText.alpha = 1f;
        isTextVisible = true;

        if (useTypewriterEffect)
        {
            typewriterCoroutine = StartCoroutine(TypewriterEffect());
        }
    }

    // 立即隐藏文本（无渐隐效果）
    public void HideTextImmediate()
    {
        if (targetText == null) return;

        // 停止所有协程
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);

        targetText.alpha = 0f;
        targetText.text = "";
        isTextVisible = false;
    }

    // 可视化调试
    void OnDrawGizmos()
    {
        Gizmos.color = isPlayerInTrigger ? new Color(0f, 1f, 0f, 0.3f) : new Color(1f, 0f, 0f, 0.3f);

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
    }
}