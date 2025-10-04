using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour
{
    [Header("打字机效果设置")]
    public float charactersPerSecond = 20f;
    public float delayBeforeStart = 0.5f;
    public bool startOnEnable = true;

    [Header("高级设置")]
    public AudioClip typeSound;          // 打字音效
    public float punctuationDelay = 0.5f; // 标点符号后的额外延迟

    [Header("事件")]
    public UnityEvent onTypingStarted;   // 打字开始事件
    public UnityEvent onTypingCompleted; // 打字完成事件

    private Text textComponent;
    private string fullText;
    private bool isTyping = false;
    private AudioSource audioSource;

    // 添加 IsTyping 属性
    public bool IsTyping
    {
        get { return isTyping; }
    }

    void Awake()
    {
        textComponent = GetComponent<Text>();
        audioSource = GetComponent<AudioSource>();

        if (textComponent != null)
        {
            fullText = textComponent.text;
            textComponent.text = "";
        }

        // 如果没有AudioSource，添加一个
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
            audioSource.playOnAwake = false;
        }
    }

    void OnEnable()
    {
        if (startOnEnable && textComponent != null)
        {
            StartTypewriter();
        }
    }

    public void StartTypewriter()
    {
        if (!isTyping)
        {
            StartCoroutine(TypeText());
        }
    }

    public void CompleteText()
    {
        if (isTyping)
        {
            StopAllCoroutines();
            textComponent.text = fullText;
            isTyping = false;
            onTypingCompleted.Invoke();
        }
    }

    private IEnumerator TypeText()
    {
        isTyping = true;
        textComponent.text = "";

        onTypingStarted.Invoke();

        yield return new WaitForSeconds(delayBeforeStart);

        float delay = 1f / charactersPerSecond;

        for (int i = 0; i <= fullText.Length; i++)
        {
            textComponent.text = fullText.Substring(0, i);

            // 播放打字音效
            if (typeSound != null && i < fullText.Length && !char.IsWhiteSpace(fullText[i]))
            {
                audioSource.PlayOneShot(typeSound);
            }

            // 检查当前字符是否为标点符号，如果是则增加延迟
            if (i < fullText.Length && IsPunctuation(fullText[i]))
            {
                yield return new WaitForSeconds(punctuationDelay);
            }
            else
            {
                yield return new WaitForSeconds(delay);
            }
        }

        isTyping = false;
        onTypingCompleted.Invoke();
    }

    private bool IsPunctuation(char c)
    {
        return c == '.' || c == '!' || c == '?' || c == ',' || c == ';' || c == ':';
    }

    public void ResetText()
    {
        if (textComponent != null)
        {
            textComponent.text = "";
            isTyping = false;
        }
    }

    public void SetText(string newText)
    {
        fullText = newText;
        ResetText();
        StartTypewriter();
    }
}