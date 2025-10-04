using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Unity.VisualScripting;

public class InteractiveTextDisplay : MonoBehaviour
{
    [Header("UI设置")]
    public GameObject textPanel; // 显示文本的UI面板
    public Text displayText;     // 显示文本的UI文本组件

    [Header("打字机效果设置")]
    public bool useTypewriterEffect = true;
    public float charactersPerSecond = 20f;
    public bool canSkipText = true; // 是否可以通过按键跳过打字效果

    [Header("交互设置")]
    public KeyCode interactionKey = KeyCode.E;
    public string textToDisplay = "这里是显示的文本内容";

    [Header("区域设置")]
    public Collider2D interactionArea;

    private bool isPlayerInRange = false;
    private TypewriterEffect typewriterEffect;
    private bool isTextShowing = false;

    void Start()
    {
        // 初始化时隐藏文本
        if (textPanel != null)
        {
            textPanel.SetActive(false);
        }

        // 获取或添加TypewriterEffect组件
        if (displayText != null && useTypewriterEffect)
        {
            typewriterEffect = displayText.GetComponent<TypewriterEffect>();
            if (typewriterEffect == null)
            {
                typewriterEffect = displayText.AddComponent<TypewriterEffect>();
            }

            // 配置打字机效果
            typewriterEffect.charactersPerSecond = charactersPerSecond;
            typewriterEffect.startOnEnable = false; // 我们手动控制开始
        }
    }

    void Update()
    {
        // 检查玩家是否在区域内并按下了交互键
        if (isPlayerInRange && Input.GetKeyDown(interactionKey))
        {
            if (isTextShowing)
            {
                // 如果文本正在显示，检查是否可以跳过或关闭
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

    // 显示文本
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

    // 隐藏文本
    void HideText()
    {
        if (textPanel != null)
        {
            textPanel.SetActive(false);
            isTextShowing = false;

            // 重置打字机效果
            if (useTypewriterEffect && typewriterEffect != null)
            {
                typewriterEffect.ResetText();
            }
        }
    }

    // 当玩家进入触发区域
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = true;
            Debug.Log("玩家进入交互区域");

            // 可选：显示提示UI，如"按E查看"
        }
    }

    // 当玩家离开触发区域
    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerInRange = false;
            Debug.Log("玩家离开交互区域");

            // 确保玩家离开区域时文本隐藏
            if (isTextShowing)
            {
                HideText();
            }
        }
    }
}