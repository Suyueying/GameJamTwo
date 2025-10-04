using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class InteractiveTextDisplay : MonoBehaviour
{
    [Header("UI����")]
    public GameObject textPanel; // ��ʾ�ı���UI���
    public Text displayText;     // ��ʾ�ı���UI�ı����

    [Header("���ֻ�Ч������")]
    public bool useTypewriterEffect = true;
    public float charactersPerSecond = 20f;
    public bool canSkipText = true; // �Ƿ����ͨ��������������Ч��

    [Header("��������")]
    public KeyCode interactionKey = KeyCode.E;
    public string textToDisplay = "��������ʾ���ı�����";

    [Header("��������")]
    public Collider2D interactionArea;

    private bool isPlayerInRange = false;
    private TypewriterEffect typewriterEffect;
    private bool isTextShowing = false;

    void Start()
    {
        // ��ʼ��ʱ�����ı�
        if (textPanel != null)
        {
            textPanel.SetActive(false);
        }

        // ��ȡ�����TypewriterEffect���
        if (displayText != null && useTypewriterEffect)
        {
            typewriterEffect = displayText.GetComponent<TypewriterEffect>();
            if (typewriterEffect == null)
            {
                typewriterEffect = displayText.AddComponent<TypewriterEffect>();
            }

            // ���ô��ֻ�Ч��
            typewriterEffect.charactersPerSecond = charactersPerSecond;
            typewriterEffect.startOnEnable = false; // �����ֶ����ƿ�ʼ
        }
    }

    void Update()
    {
        // �������Ƿ��������ڲ������˽�����
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            if (isTextShowing)
            {
                // ����ı�������ʾ������Ƿ����������ر�
                if (useTypewriterEffect && canSkipText && typewriterEffect != null && typewriterEffect.IsTyping)
                {
                    typewriterEffect.CompleteText();
                }
                else
                {
                    HideText();
                }
            }
            else
            {
                ShowText();
            }
        }
    }

    // ��ʾ�ı�
    void ShowText()
    {
        if (textPanel != null && displayText != null)
        {
            textPanel.SetActive(true);
            isTextShowing = true;

            if (useTypewriterEffect && typewriterEffect != null)
            {
                typewriterEffect.SetText(textToDisplay);
            }
            else
            {
                displayText.text = textToDisplay;
            }
        }
    }

    // �����ı�
    void HideText()
    {
        if (textPanel != null)
        {
            textPanel.SetActive(false);
            isTextShowing = false;

            // ���ô��ֻ�Ч��
            if (useTypewriterEffect && typewriterEffect != null)
            {
                typewriterEffect.ResetText();
            }
        }
    }

    // ����ҽ��봥������
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("��ҽ��뽻������");

            // ��ѡ����ʾ��ʾUI����"��E�鿴"
        }
    }

    // ������뿪��������
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("����뿪��������");

            // ȷ������뿪����ʱ�ı�����
            if (isTextShowing)
            {
                HideText();
            }
        }
    }
}