using UnityEngine;
using UnityEngine.Rendering.Universal; // 更新后的命名空间
using System.Collections;

public class Light2DIntensityTrigger : MonoBehaviour
{
    [Header("2D灯光设置")]
    public Light2D targetLight2D;           // 2D灯光组件
    public float targetIntensity = 1f;      // 目标强度值
    public float transitionDuration = 2f;   // 过渡时间

    [Header("颜色设置（可选）")]
    public Color targetColor = Color.white;
    public bool changeColor = false;

    [Header("触发设置")]
    public bool triggerOnce = true;
    public bool requirePlayerTag = true;
    public bool showDebugLogs = true;

    [Header("其他2D灯光属性")]
    public float targetOuterRadius = 5f;    // 目标外半径（点光）
    public float targetInnerRadius = 2f;    // 目标内半径（点光）
    public bool changeRadius = false;

    private bool hasBeenTriggered = false;
    private Coroutine transitionCoroutine;
    private float originalIntensity;
    private Color originalColor;
    private float originalOuterRadius;
    private float originalInnerRadius;

    void Start()
    {
        // 检查并获取Light2D组件
        if (targetLight2D == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("目标Light2D未设置！请将Light2D对象拖拽到Target Light2D字段");
            return;
        }

        // 保存原始值
        originalIntensity = targetLight2D.intensity;
        originalColor = targetLight2D.color;

        // 只有点光才有半径属性
        if (targetLight2D.lightType == Light2D.LightType.Point)
        {
            originalOuterRadius = targetLight2D.pointLightOuterRadius;
            originalInnerRadius = targetLight2D.pointLightInnerRadius;
        }

        if (showDebugLogs)
        {
            Debug.Log($"2D灯光触发器初始化 - 目标灯光: {targetLight2D.name}");
            Debug.Log($"灯光类型: {targetLight2D.lightType}");
            Debug.Log($"初始强度: {originalIntensity}");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (requirePlayerTag && !other.CompareTag("Player"))
            return;

        if (triggerOnce && hasBeenTriggered)
            return;

        if (targetLight2D == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("目标Light2D未设置！");
            return;
        }

        if (showDebugLogs)
            Debug.Log($"2D灯光触发器被触发");

        hasBeenTriggered = true;

        // 停止之前的过渡
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        // 开始过渡
        transitionCoroutine = StartCoroutine(TransitionLight2D());
    }

    private IEnumerator TransitionLight2D()
    {
        float elapsedTime = 0f;

        // 保存起始值
        float startIntensity = targetLight2D.intensity;
        Color startColor = targetLight2D.color;
        float startOuterRadius = 0f;
        float startInnerRadius = 0f;

        // 只有点光才保存半径
        if (targetLight2D.lightType == Light2D.LightType.Point)
        {
            startOuterRadius = targetLight2D.pointLightOuterRadius;
            startInnerRadius = targetLight2D.pointLightInnerRadius;
        }

        if (showDebugLogs)
        {
            Debug.Log($"开始2D灯光过渡:");
            Debug.Log($"强度: {startIntensity} -> {targetIntensity}");
            if (changeColor)
                Debug.Log($"颜色: {startColor} -> {targetColor}");
            if (changeRadius && targetLight2D.lightType == Light2D.LightType.Point)
                Debug.Log($"外半径: {startOuterRadius} -> {targetOuterRadius}");
        }

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / transitionDuration);

            // 更新强度
            targetLight2D.intensity = Mathf.Lerp(startIntensity, targetIntensity, progress);

            // 更新颜色
            if (changeColor)
            {
                targetLight2D.color = Color.Lerp(startColor, targetColor, progress);
            }

            // 更新半径（只有点光才有半径属性）
            if (changeRadius && targetLight2D.lightType == Light2D.LightType.Point)
            {
                targetLight2D.pointLightOuterRadius = Mathf.Lerp(startOuterRadius, targetOuterRadius, progress);
                targetLight2D.pointLightInnerRadius = Mathf.Lerp(startInnerRadius, targetInnerRadius, progress);
            }

            yield return null;
        }

        // 确保最终值准确
        targetLight2D.intensity = targetIntensity;
        if (changeColor)
        {
            targetLight2D.color = targetColor;
        }
        if (changeRadius && targetLight2D.lightType == Light2D.LightType.Point)
        {
            targetLight2D.pointLightOuterRadius = targetOuterRadius;
            targetLight2D.pointLightInnerRadius = targetInnerRadius;
        }

        if (showDebugLogs)
            Debug.Log("2D灯光过渡完成");
    }

    // 手动触发方法
    public void StartLightTransition()
    {
        if (targetLight2D == null) return;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionLight2D());
    }

    // 重置到原始值
    public void ResetToOriginal()
    {
        if (targetLight2D == null) return;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(ResetLight2D());
    }

    private IEnumerator ResetLight2D()
    {
        float elapsedTime = 0f;
        float currentIntensity = targetLight2D.intensity;
        Color currentColor = targetLight2D.color;
        float currentOuterRadius = 0f;
        float currentInnerRadius = 0f;

        if (targetLight2D.lightType == Light2D.LightType.Point)
        {
            currentOuterRadius = targetLight2D.pointLightOuterRadius;
            currentInnerRadius = targetLight2D.pointLightInnerRadius;
        }

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / transitionDuration);

            targetLight2D.intensity = Mathf.Lerp(currentIntensity, originalIntensity, progress);
            targetLight2D.color = Color.Lerp(currentColor, originalColor, progress);

            if (changeRadius && targetLight2D.lightType == Light2D.LightType.Point)
            {
                targetLight2D.pointLightOuterRadius = Mathf.Lerp(currentOuterRadius, originalOuterRadius, progress);
                targetLight2D.pointLightInnerRadius = Mathf.Lerp(currentInnerRadius, originalInnerRadius, progress);
            }

            yield return null;
        }

        // 确保最终值准确
        targetLight2D.intensity = originalIntensity;
        targetLight2D.color = originalColor;

        if (targetLight2D.lightType == Light2D.LightType.Point)
        {
            targetLight2D.pointLightOuterRadius = originalOuterRadius;
            targetLight2D.pointLightInnerRadius = originalInnerRadius;
        }
    }

    // 检查灯光类型是否支持半径变化
    public bool SupportsRadiusChanges()
    {
        return targetLight2D != null && targetLight2D.lightType == Light2D.LightType.Point;
    }

    // 可视化调试
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.8f, 0.2f, 1f, 0.3f);

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

        // 绘制到灯光的连线
        if (targetLight2D != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetLight2D.transform.position);
        }
    }
}