using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    [Header("下一个场景")]
    public string targetSceneName;

    [Header("过渡效果设置")]
    public FadeTransition fadeTransition;

    [Header("过渡控制")]
    public bool autoLoadOnStart = false;
    public float delayBeforeLoad = 0f;

    private bool isTriggeredTransition = false;

    void Start()
    {
        if (autoLoadOnStart)
        {
            Debug.Log("autoLoadOnStart 为 true，开始淡入动画");

            if (fadeTransition != null)
            {
                fadeTransition.fadeInEnabled = true;  // 启用淡入
                fadeTransition.fadeOutEnabled = false; // 不启用淡出
                fadeTransition.StartTransition();
            }
            else
            {
                Debug.LogWarning("FadeTransition 组件未设置，无法播放淡入动画");
            }
        }
    }

    public void LoadTargetScene(bool isTriggered = false)
    {
        Debug.Log($"开始加载场景: {targetSceneName}");

        if (!Application.CanStreamedLevelBeLoaded(targetSceneName))
        {
            Debug.LogError($"场景不存在或未添加到Build Settings: {targetSceneName}");
            return;
        }

        isTriggeredTransition = isTriggered;

        if (fadeTransition != null)
        {
            Debug.Log("使用淡入淡出过渡效果");
            fadeTransition.fadeInEnabled = false;  // 不启用淡入
            fadeTransition.fadeOutEnabled = true; // 启用淡出
            StartCoroutine(LoadSceneWithTransition());
        }
        else
        {
            SceneManager.LoadScene(targetSceneName);
        }
    }

    private IEnumerator LoadSceneWithTransition()
    {
        // 淡出效果（逐渐黑幕）
        fadeTransition.StartTransition();

        Debug.Log("开始淡出过渡，屏幕逐渐变黑");

        // 等待淡出完成
        yield return new WaitForSeconds(fadeTransition.transitionDuration);

        Debug.Log("淡出完成，开始加载场景");

        // 立即跳转场景
        SceneManager.LoadScene(targetSceneName);
    }
}