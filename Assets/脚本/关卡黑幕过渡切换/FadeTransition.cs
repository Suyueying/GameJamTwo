using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class FadeTransition : MonoBehaviour
{
    [Header("基本设置")]
    public Image fadeImage;

    public enum TransitionDirection
    {
        LeftToRight,
        RightToLeft,
        TopToBottom,
        BottomToTop
    }

    public enum TransitionType
    {
        Normal,
        Tear,
        Rotate
    }

    [Header("配置参数")]
    public TransitionDirection direction = TransitionDirection.LeftToRight;
    public TransitionType transitionType = TransitionType.Normal;
    public float transitionDuration = 1.0f;

    // 新增：淡入和淡出的独立控制
    public bool fadeInEnabled = true;  // 是否启用淡入
    public bool fadeOutEnabled = true; // 是否启用淡出

    private Coroutine currentCoroutine;
    private bool isFadingIn = false;   // 当前是否正在淡入
    private bool hasStartedFadeIn = false; // 是否已开始淡入
    private bool hasStartedFadeOut = false; // 是否已开始淡出


    void Start()
    {
        Debug.Log("FadeTransition Start called");
        if (fadeImage == null)
        {
            Debug.LogError("Fade image not assigned!");
            return;
        }
        Debug.Log("FadeTransition initialized successfully");
    }

    public void StartTransition()
    {
        Debug.Log($"StartTransition called. fadeInEnabled: {fadeInEnabled}, fadeOutEnabled: {fadeOutEnabled}");

        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
        }

        // 如果只启用淡入，则执行淡入
        if (fadeInEnabled && !hasStartedFadeIn)
        {
            isFadingIn = true;
            hasStartedFadeIn = true;
            currentCoroutine = StartCoroutine(PerformTransition());
            return;
        }

        // 如果只启用淡出，则执行淡出
        if (fadeOutEnabled && !hasStartedFadeOut)
        {
            isFadingIn = false;
            hasStartedFadeOut = true;
            currentCoroutine = StartCoroutine(PerformTransition());
            return;
        }

        // 如果两者都启用，则先执行淡入，再执行淡出
        if (fadeInEnabled && fadeOutEnabled)
        {
            isFadingIn = true;
            hasStartedFadeIn = true;
            currentCoroutine = StartCoroutine(PerformTransition());
            return;
        }

        Debug.LogWarning("FadeTransition: 淡入和淡出均未启用");
    }

    private IEnumerator PerformTransition()
    {
        Debug.Log("PerformTransition started");

        float startTime = Time.time;
        float endTime = startTime + transitionDuration;

        Debug.Log($"Transition from {(isFadingIn ? 0 : 1)} to {(isFadingIn ? 1 : 0)}");

        while (Time.time < endTime)
        {
            float normalizedTime = (Time.time - startTime) / transitionDuration;
            float progress = normalizedTime;

            switch (transitionType)
            {
                case TransitionType.Normal:
                    PerformNormalTransition(progress);
                    break;

                case TransitionType.Tear:
                    PerformTearTransition(progress);
                    break;

                case TransitionType.Rotate:
                    PerformRotateTransition(progress);
                    break;
            }

            yield return null;
        }

        SetFinalState();

        if (isFadingIn && fadeOutEnabled && !hasStartedFadeOut)
        {
            Debug.Log("淡入完成，开始淡出");
            isFadingIn = false;
            hasStartedFadeOut = true;
            StartCoroutine(PerformTransition());
        }
        else
        {
            Debug.Log("过渡完成");
        }

        Debug.Log("PerformTransition completed");
    }

    private void PerformNormalTransition(float progress)
    {

        float alpha = Mathf.Lerp(isFadingIn ? 1 : 0, isFadingIn ? 0 : 1, progress);
        fadeImage.color = new Color(0, 0, 0, alpha);
        Debug.Log($"Normal transition. Progress: {progress}, Alpha: {alpha}");
    }

    private void PerformTearTransition(float progress)
    {
        float alpha = Mathf.Lerp(isFadingIn ? 1 : 0, isFadingIn ? 0 : 1, progress);
        if (fadeImage != null)
        {
            fadeImage.color = new Color(0, 0, 0, alpha);
            float scale = 1f + Mathf.Sin(progress * Mathf.PI) * 0.1f;
            fadeImage.rectTransform.localScale = new Vector3(scale, scale, 1f);
        }
        Debug.Log($"Tear transition. Progress: {progress}, Alpha: {alpha}");
    }

    private void PerformRotateTransition(float progress)
    {
        float alpha = Mathf.Lerp(isFadingIn ? 1 : 0, isFadingIn ? 0 : 1, progress);
        fadeImage.color = new Color(0, 0, 0, alpha);
        float rotation = progress * 360f * (isFadingIn ? 1 : -1);
        fadeImage.rectTransform.rotation = Quaternion.Euler(0, 0, rotation);
        Debug.Log($"Rotate transition. Progress: {progress}, Alpha: {alpha}, Rotation: {rotation}");
    }

    private void SetFinalState()
    {
        if (isFadingIn)
        {
            fadeImage.color = new Color(0, 0, 0, 0); // 完全透明
            Debug.Log("Set final state to transparent (fade in)");
        }
        else
        {
            fadeImage.color = new Color(0, 0, 0, 1); // 完全黑色
            Debug.Log("Set final state to black (fade out)");
        }

        // 强制重置 scale 和 rotation，防止残留变形
        fadeImage.rectTransform.localScale = Vector3.one;
        fadeImage.rectTransform.rotation = Quaternion.identity;

        // 可选：强制设置锚点和位置，确保 UI 元素覆盖整个屏幕
        RectTransform rt = fadeImage.rectTransform;
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;
    }

    public void ResetTransition()
    {
        Debug.Log("ResetTransition called");
        if (currentCoroutine != null)
        {
            StopCoroutine(currentCoroutine);
            currentCoroutine = null;
        }

        fadeImage.color = new Color(0, 0, 0, 0);
        fadeImage.rectTransform.localScale = Vector3.one;
        fadeImage.rectTransform.rotation = Quaternion.identity;
    }
}