// LightTrigger2D.cs - 修正Freeform Light 2D控制
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class LightTrigger2D : MonoBehaviour
{
    [Header("光源设置")]
    public Light2D targetLight; // 需要控制的光源
    public LightType lightType = LightType.Freeform; // 新增：光源类型
    
    [Header("触发时光照设置")]
    [Range(0, 7)]
    public float targetIntensity = 7f; // 触发时的光照强度
    public Color targetColor = Color.white; // 触发时的光照颜色
    
    [Header("Freeform光参数")]
    [Range(0.1f, 5f)]
    public float targetFalloffIntensity = 1f; // Freeform光的衰减强度
    public Vector2 targetShapeScale = Vector2.one; // Freeform光形状缩放
    
    [Header("点光参数")]
    [Range(0.1f, 20f)]
    public float targetPointLightRadius = 5f; // 点光源半径
    
    [Header("过渡设置")]
    public float transitionDuration = 1f; // 过渡持续时间
    public AnimationCurve transitionCurve = AnimationCurve.EaseInOut(0, 0, 1, 1); // 过渡曲线
    
    [Header("初始值记录")]
    [SerializeField] private float originalIntensity; // 原始光照强度
    [SerializeField] private Color originalColor; // 原始光照颜色
    [SerializeField] private float originalFalloffIntensity; // Freeform光原始衰减强度
    [SerializeField] private Vector2 originalShapeScale; // Freeform光原始形状缩放
    [SerializeField] private float originalPointLightRadius; // 点光源原始半径
    
    private bool isPlayerInTrigger = false;
    private Coroutine transitionCoroutine;
    
    // 光源类型枚举
    public enum LightType
    {
        Freeform,
        Point,
        Global,
        Sprite
    }
    
    void Start()
    {
        // 验证目标光源
        if (targetLight == null)
        {
            Debug.LogError("请指定需要控制的光源！");
            return;
        }
        
        // 自动检测光源类型
        DetectLightType();
        
        // 记录原始值
        originalIntensity = targetLight.intensity;
        originalColor = targetLight.color;
        RecordOriginalFalloffValues();
        
        Debug.Log($"光源初始化 - 类型: {lightType}, 原始强度: {originalIntensity}, 原始颜色: {originalColor}");
    }
    
    // 自动检测光源类型
    private void DetectLightType()
    {
        switch (targetLight.lightType)
        {
            case UnityEngine.Rendering.Universal.Light2D.LightType.Freeform:
                lightType = LightType.Freeform;
                break;
            case UnityEngine.Rendering.Universal.Light2D.LightType.Point:
                lightType = LightType.Point;
                break;
            case UnityEngine.Rendering.Universal.Light2D.LightType.Global:
                lightType = LightType.Global;
                break;
            case UnityEngine.Rendering.Universal.Light2D.LightType.Sprite:
                lightType = LightType.Sprite;
                break;
        }
        Debug.Log($"检测到光源类型: {lightType}");
    }
    
    // 记录原始衰减值
    private void RecordOriginalFalloffValues()
    {
        switch (lightType)
        {
            case LightType.Freeform:
                // 对于Freeform光，记录falloff强度
                originalFalloffIntensity = targetLight.falloffIntensity;
                originalShapeScale = Vector2.one; // 默认缩放
                break;
            case LightType.Point:
                originalPointLightRadius = targetLight.pointLightOuterRadius;
                break;
            case LightType.Global:
            case LightType.Sprite:
                // 全局光和精灵光可能没有直接的衰减参数
                originalFalloffIntensity = 1f;
                break;
        }
    }
    
    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetLight != null)
        {
            isPlayerInTrigger = true;
            
            // 停止当前的过渡协程
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            
            // 开始过渡到目标值
            transitionCoroutine = StartCoroutine(TransitionLight(true));
            
            Debug.Log("玩家进入光触发器，开始增强光照");
        }
    }
    
    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && targetLight != null)
        {
            isPlayerInTrigger = false;
            
            // 停止当前的过渡协程
            if (transitionCoroutine != null)
            {
                StopCoroutine(transitionCoroutine);
            }
            
            // 开始过渡回原始值
            transitionCoroutine = StartCoroutine(TransitionLight(false));
            
            Debug.Log("玩家离开光触发器，恢复原始光照");
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isPlayerInTrigger)
        {
            isPlayerInTrigger = true;
        }
    }
    
    // 统一的光照过渡协程
    private System.Collections.IEnumerator TransitionLight(bool toTarget)
    {
        float startIntensity = targetLight.intensity;
        Color startColor = targetLight.color;
        float startFalloffIntensity = GetCurrentFalloffIntensity();
        Vector2 startShapeScale = GetCurrentShapeScale();
        
        float targetIntensityValue = toTarget ? targetIntensity : originalIntensity;
        Color targetColorValue = toTarget ? targetColor : originalColor;
        float targetFalloffIntensityValue = toTarget ? targetFalloffIntensity : originalFalloffIntensity;
        Vector2 targetShapeScaleValue = toTarget ? targetShapeScale : originalShapeScale;
        float targetPointRadiusValue = toTarget ? targetPointLightRadius : originalPointLightRadius;
        
        float elapsedTime = 0f;
        
        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = elapsedTime / transitionDuration;
            float curveProgress = transitionCurve.Evaluate(progress);
            
            // 平滑过渡强度
            targetLight.intensity = Mathf.Lerp(startIntensity, targetIntensityValue, curveProgress);
            
            // 平滑过渡颜色
            targetLight.color = Color.Lerp(startColor, targetColorValue, curveProgress);
            
            // 平滑过渡Freeform光衰减强度
            if (lightType == LightType.Freeform)
            {
                targetLight.falloffIntensity = Mathf.Lerp(startFalloffIntensity, targetFalloffIntensityValue, curveProgress);
                
                // 注意：形状缩放需要通过变换控制，见下面的方法
                ApplyShapeScale(Vector2.Lerp(startShapeScale, targetShapeScaleValue, curveProgress));
            }
            
            // 平滑过渡点光源半径
            if (lightType == LightType.Point)
            {
                targetLight.pointLightOuterRadius = Mathf.Lerp(GetCurrentPointLightRadius(), targetPointRadiusValue, curveProgress);
            }
            
            yield return null;
        }
        
        // 确保最终达到目标值
        targetLight.intensity = targetIntensityValue;
        targetLight.color = targetColorValue;
        
        if (lightType == LightType.Freeform)
        {
            targetLight.falloffIntensity = targetFalloffIntensityValue;
            ApplyShapeScale(targetShapeScaleValue);
        }
        else if (lightType == LightType.Point)
        {
            targetLight.pointLightOuterRadius = targetPointRadiusValue;
        }
        
        Debug.Log($"光照过渡完成 - 强度: {targetLight.intensity}, 颜色: {targetLight.color}");
    }
    
    // 获取当前Freeform光衰减强度
    private float GetCurrentFalloffIntensity()
    {
        return targetLight.falloffIntensity;
    }
    
    // 获取当前形状缩放（通过变换组件）
    private Vector2 GetCurrentShapeScale()
    {
        return targetLight.transform.localScale;
    }
    
    // 应用形状缩放
    private void ApplyShapeScale(Vector2 scale)
    {
        Vector3 newScale = targetLight.transform.localScale;
        newScale.x = scale.x;
        newScale.y = scale.y;
        targetLight.transform.localScale = newScale;
    }
    
    // 获取当前点光源半径
    private float GetCurrentPointLightRadius()
    {
        return targetLight.pointLightOuterRadius;
    }
    
    // 公共方法：设置Freeform光的目标参数
    public void SetFreeformTargetValues(float falloffIntensity, Vector2 shapeScale)
    {
        if (lightType == LightType.Freeform)
        {
            targetFalloffIntensity = Mathf.Clamp01(falloffIntensity);
            targetShapeScale = new Vector2(
                Mathf.Max(0.1f, shapeScale.x),
                Mathf.Max(0.1f, shapeScale.y)
            );
        }
    }
    
    // 公共方法：设置点光的目标半径
    public void SetPointLightTargetRadius(float radius)
    {
        if (lightType == LightType.Point)
        {
            targetPointLightRadius = Mathf.Max(0.1f, radius);
        }
    }
    
    // 公共方法：立即切换到目标光照
    public void SnapToTargetLight()
    {
        if (targetLight != null)
        {
            targetLight.intensity = targetIntensity;
            targetLight.color = targetColor;
            
            if (lightType == LightType.Freeform)
            {
                targetLight.falloffIntensity = targetFalloffIntensity;
                ApplyShapeScale(targetShapeScale);
            }
            else if (lightType == LightType.Point)
            {
                targetLight.pointLightOuterRadius = targetPointLightRadius;
            }
        }
    }
    
    // 公共方法：立即恢复原始光照
    public void SnapToOriginalLight()
    {
        if (targetLight != null)
        {
            targetLight.intensity = originalIntensity;
            targetLight.color = originalColor;
            
            if (lightType == LightType.Freeform)
            {
                targetLight.falloffIntensity = originalFalloffIntensity;
                ApplyShapeScale(originalShapeScale);
            }
            else if (lightType == LightType.Point)
            {
                targetLight.pointLightOuterRadius = originalPointLightRadius;
            }
        }
    }
    
    // 公共方法：获取当前光照状态
    public string GetLightStatus()
    {
        if (targetLight == null) return "无目标光源";
        
        string status = isPlayerInTrigger ? "触发状态" : "正常状态";
        string falloffInfo = "";
        
        switch (lightType)
        {
            case LightType.Freeform:
                falloffInfo = $"衰减强度: {targetLight.falloffIntensity:F2}, 缩放: {targetLight.transform.localScale}";
                break;
            case LightType.Point:
                falloffInfo = $"半径: {targetLight.pointLightOuterRadius:F2}";
                break;
            default:
                falloffInfo = "无衰减控制";
                break;
        }
        
        return $"{status} - 类型: {lightType}, 强度: {targetLight.intensity:F2}, 颜色: {targetLight.color}, {falloffInfo}";
    }
    
    private void OnDrawGizmos()
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            Gizmos.color = isPlayerInTrigger ? new Color(1, 0.8f, 0, 0.3f) : new Color(0, 1, 1, 0.3f);
            Gizmos.DrawCube(transform.position, collider.bounds.size);
            
            if (targetLight != null)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(transform.position, targetLight.transform.position);
            }
        }
    }
    
    #if UNITY_EDITOR
    [ContextMenu("自动检测光源类型")]
    private void AutoDetectLightType()
    {
        if (targetLight != null)
        {
            DetectLightType();
            UnityEditor.EditorUtility.SetDirty(this);
        }
    }
    
    [ContextMenu("记录当前值为原始值")]
    private void RecordCurrentAsOriginal()
    {
        if (targetLight != null)
        {
            originalIntensity = targetLight.intensity;
            originalColor = targetLight.color;
            
            if (lightType == LightType.Freeform)
            {
                originalFalloffIntensity = targetLight.falloffIntensity;
                originalShapeScale = targetLight.transform.localScale;
            }
            else if (lightType == LightType.Point)
            {
                originalPointLightRadius = targetLight.pointLightOuterRadius;
            }
            
            Debug.Log("已记录当前光照值为原始值");
        }
    }
    
    [ContextMenu("应用目标值测试")]
    private void ApplyTargetValuesTest()
    {
        SnapToTargetLight();
    }
    
    [ContextMenu("恢复原始值测试")]
    private void RestoreOriginalValuesTest()
    {
        SnapToOriginalLight();
    }
    #endif
}