using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class TypewriterEffect : MonoBehaviour
{
    [Header("���ֻ�Ч������")]
    public float charactersPerSecond = 20f;
    public float delayBeforeStart = 0.5f;
    public bool startOnEnable = true;

    [Header("�߼�����")]
    public AudioClip typeSound;          // ������Ч
    public float punctuationDelay = 0.5f; // �����ź�Ķ����ӳ�

    [Header("�¼�")]
    public UnityEvent onTypingStarted;   // ���ֿ�ʼ�¼�
    public UnityEvent onTypingCompleted; // ��������¼�

    private Text textComponent;
    private string fullText;
    private bool isTyping = false;
    private AudioSource audioSource;

    // ��� IsTyping ����
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

        // ���û��AudioSource�����һ��
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

            // ���Ŵ�����Ч
            if (typeSound != null && i < fullText.Length && !char.IsWhiteSpace(fullText[i]))
            {
                audioSource.PlayOneShot(typeSound);
            }

            // ��鵱ǰ�ַ��Ƿ�Ϊ�����ţ�������������ӳ�
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