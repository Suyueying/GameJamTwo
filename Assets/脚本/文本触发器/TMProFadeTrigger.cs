using UnityEngine;
using TMPro;
using System.Collections;

public class TMProFadeTrigger : MonoBehaviour
{
    [Header("Text Mesh Pro ����")]
    public TextMeshProUGUI targetText;      // Text Mesh Pro �ı����
    public string displayText = "��������ʾ����������"; // Ҫ��ʾ���ı�

    [Header("������������")]
    public float fadeInDuration = 1f;       // ����ʱ��
    public float fadeOutDuration = 1f;      // ����ʱ��
    public bool useTypewriterEffect = false; // �Ƿ�ʹ�ô��ֻ�Ч��
    public float typewriterSpeed = 20f;     // �����ٶȣ��ַ�/�룩

    [Header("��������")]
    public bool requirePlayerTag = true;    // �Ƿ���Ҫ��ұ�ǩ
    public bool showDebugLogs = true;       // ��ʾ������Ϣ

    private Coroutine fadeCoroutine;
    private Coroutine typewriterCoroutine;
    private bool isPlayerInTrigger = false;
    private bool isTextVisible = false;

    void Start()
    {
        // ��ʼ���ı�״̬
        InitializeText();
    }

    void Update()
    {
        // �����ã���T��������ʾ����Y����������
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
            targetText.alpha = 0f; // ��ʼʱ��ȫ͸��
            targetText.text = "";  // ����ı�
        }

        if (showDebugLogs)
        {
            Debug.Log($"�ı�������������ʼ�� - Ŀ���ı�: {(targetText != null ? targetText.name : "δ����")}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!ShouldTrigger(other)) return;

        isPlayerInTrigger = true;

        if (showDebugLogs)
            Debug.Log("��ҽ��봥��������ʼ�����ı�");

        ShowText();
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!ShouldTrigger(other)) return;

        // ȷ������ڴ�������ʱ�ı�������ʾ״̬
        if (!isTextVisible && isPlayerInTrigger)
        {
            if (showDebugLogs)
                Debug.Log("������ڴ������ڣ�ȷ���ı���ʾ");

            ShowText();
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!ShouldTrigger(other)) return;

        isPlayerInTrigger = false;

        if (showDebugLogs)
            Debug.Log("����뿪����������ʼ�����ı�");

        HideText();
    }

    private bool ShouldTrigger(Collider2D other)
    {
        if (requirePlayerTag && !other.CompareTag("Player"))
            return false;

        if (targetText == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("Ŀ��Text Mesh Pro�ı�δ���ã�");
            return false;
        }

        return true;
    }

    // ��ʾ�ı������ԣ�
    public void ShowText()
    {
        if (targetText == null) return;

        // ����ı��Ѿ��ɼ��������ظ���ʾ
        if (isTextVisible && targetText.alpha >= 0.99f)
            return;

        // ֹ֮ͣǰ�Ľ���Э��
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }

        // �����ı�����
        targetText.text = displayText;
        targetText.alpha = Mathf.Max(targetText.alpha, 0.01f); // ȷ��������һ��͸����

        // ��ʼ����
        fadeCoroutine = StartCoroutine(FadeText(targetText.alpha, 1f, fadeInDuration, true));
    }

    // �����ı���������
    public void HideText()
    {
        if (targetText == null || targetText.alpha <= 0f || !isTextVisible)
            return;

        // �����һ��ڴ������ڣ��������ı�
        if (isPlayerInTrigger)
        {
            if (showDebugLogs)
                Debug.Log("������ڴ������ڣ�ȡ������");
            return;
        }

        // ֹ֮ͣǰ��Э��
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        if (typewriterCoroutine != null)
        {
            StopCoroutine(typewriterCoroutine);
        }

        // ��ʼ����
        fadeCoroutine = StartCoroutine(FadeText(targetText.alpha, 0f, fadeOutDuration, false));
    }

    // �ı���������Э��
    private IEnumerator FadeText(float startAlpha, float targetAlpha, float duration, bool isFadeIn)
    {
        isTextVisible = isFadeIn;

        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            // ����ڽ�������������ֽ����˴�������ֹͣ����
            if (!isFadeIn && isPlayerInTrigger)
            {
                if (showDebugLogs)
                    Debug.Log("������������ҽ��봥������ֹͣ����");
                yield break;
            }

            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / duration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, progress);

            targetText.alpha = newAlpha;

            yield return null;
        }

        // ȷ������alphaֵ׼ȷ
        targetText.alpha = targetAlpha;

        // ����ǽ����������Ҫʹ�ô��ֻ�Ч��
        if (isFadeIn && useTypewriterEffect && targetAlpha > 0)
        {
            typewriterCoroutine = StartCoroutine(TypewriterEffect());
        }

        if (showDebugLogs)
        {
            if (isFadeIn)
                Debug.Log("�ı��������");
            else
                Debug.Log("�ı��������");
        }
    }

    // ���ֻ�Ч��Э��
    private IEnumerator TypewriterEffect()
    {
        string fullText = targetText.text;
        targetText.text = fullText;
        targetText.maxVisibleCharacters = 0;

        int totalCharacters = fullText.Length;
        float delayPerCharacter = 1f / typewriterSpeed;

        for (int i = 0; i <= totalCharacters; i++)
        {
            if (!isTextVisible || !isPlayerInTrigger) break; // ����ı������ػ�����뿪��ֹͣ����Ч��

            targetText.maxVisibleCharacters = i;
            yield return new WaitForSeconds(delayPerCharacter);
        }

        // ȷ����ȫ��ʾ
        if (isTextVisible && isPlayerInTrigger)
        {
            targetText.maxVisibleCharacters = totalCharacters;
        }
    }

    // ������ʾ�ı����޽���Ч����
    public void ShowTextImmediate()
    {
        if (targetText == null) return;

        // ֹͣ����Э��
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

    // ���������ı����޽���Ч����
    public void HideTextImmediate()
    {
        if (targetText == null) return;

        // ֹͣ����Э��
        if (fadeCoroutine != null) StopCoroutine(fadeCoroutine);
        if (typewriterCoroutine != null) StopCoroutine(typewriterCoroutine);

        targetText.alpha = 0f;
        targetText.text = "";
        isTextVisible = false;
    }

    // ���ӻ�����
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