using UnityEngine;
using UnityEngine.Rendering.Universal; // ���º�������ռ�
using System.Collections;

public class Light2DIntensityTrigger : MonoBehaviour
{
    [Header("2D�ƹ�����")]
    public Light2D targetLight2D;           // 2D�ƹ����
    public float targetIntensity = 1f;      // Ŀ��ǿ��ֵ
    public float transitionDuration = 2f;   // ����ʱ��

    [Header("��ɫ���ã���ѡ��")]
    public Color targetColor = Color.white;
    public bool changeColor = false;

    [Header("��������")]
    public bool triggerOnce = true;
    public bool requirePlayerTag = true;
    public bool showDebugLogs = true;

    [Header("����2D�ƹ�����")]
    public float targetOuterRadius = 5f;    // Ŀ����뾶����⣩
    public float targetInnerRadius = 2f;    // Ŀ���ڰ뾶����⣩
    public bool changeRadius = false;

    private bool hasBeenTriggered = false;
    private Coroutine transitionCoroutine;
    private float originalIntensity;
    private Color originalColor;
    private float originalOuterRadius;
    private float originalInnerRadius;

    void Start()
    {
        // ��鲢��ȡLight2D���
        if (targetLight2D == null)
        {
            if (showDebugLogs)
                Debug.LogWarning("Ŀ��Light2Dδ���ã��뽫Light2D������ק��Target Light2D�ֶ�");
            return;
        }

        // ����ԭʼֵ
        originalIntensity = targetLight2D.intensity;
        originalColor = targetLight2D.color;

        // ֻ�е����а뾶����
        if (targetLight2D.lightType == Light2D.LightType.Point)
        {
            originalOuterRadius = targetLight2D.pointLightOuterRadius;
            originalInnerRadius = targetLight2D.pointLightInnerRadius;
        }

        if (showDebugLogs)
        {
            Debug.Log($"2D�ƹⴥ������ʼ�� - Ŀ��ƹ�: {targetLight2D.name}");
            Debug.Log($"�ƹ�����: {targetLight2D.lightType}");
            Debug.Log($"��ʼǿ��: {originalIntensity}");
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
                Debug.LogWarning("Ŀ��Light2Dδ���ã�");
            return;
        }

        if (showDebugLogs)
            Debug.Log($"2D�ƹⴥ����������");

        hasBeenTriggered = true;

        // ֹ֮ͣǰ�Ĺ���
        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        // ��ʼ����
        transitionCoroutine = StartCoroutine(TransitionLight2D());
    }

    private IEnumerator TransitionLight2D()
    {
        float elapsedTime = 0f;

        // ������ʼֵ
        float startIntensity = targetLight2D.intensity;
        Color startColor = targetLight2D.color;
        float startOuterRadius = 0f;
        float startInnerRadius = 0f;

        // ֻ�е��ű���뾶
        if (targetLight2D.lightType == Light2D.LightType.Point)
        {
            startOuterRadius = targetLight2D.pointLightOuterRadius;
            startInnerRadius = targetLight2D.pointLightInnerRadius;
        }

        if (showDebugLogs)
        {
            Debug.Log($"��ʼ2D�ƹ����:");
            Debug.Log($"ǿ��: {startIntensity} -> {targetIntensity}");
            if (changeColor)
                Debug.Log($"��ɫ: {startColor} -> {targetColor}");
            if (changeRadius && targetLight2D.lightType == Light2D.LightType.Point)
                Debug.Log($"��뾶: {startOuterRadius} -> {targetOuterRadius}");
        }

        while (elapsedTime < transitionDuration)
        {
            elapsedTime += Time.deltaTime;
            float progress = Mathf.Clamp01(elapsedTime / transitionDuration);

            // ����ǿ��
            targetLight2D.intensity = Mathf.Lerp(startIntensity, targetIntensity, progress);

            // ������ɫ
            if (changeColor)
            {
                targetLight2D.color = Color.Lerp(startColor, targetColor, progress);
            }

            // ���°뾶��ֻ�е����а뾶���ԣ�
            if (changeRadius && targetLight2D.lightType == Light2D.LightType.Point)
            {
                targetLight2D.pointLightOuterRadius = Mathf.Lerp(startOuterRadius, targetOuterRadius, progress);
                targetLight2D.pointLightInnerRadius = Mathf.Lerp(startInnerRadius, targetInnerRadius, progress);
            }

            yield return null;
        }

        // ȷ������ֵ׼ȷ
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
            Debug.Log("2D�ƹ�������");
    }

    // �ֶ���������
    public void StartLightTransition()
    {
        if (targetLight2D == null) return;

        if (transitionCoroutine != null)
        {
            StopCoroutine(transitionCoroutine);
        }

        transitionCoroutine = StartCoroutine(TransitionLight2D());
    }

    // ���õ�ԭʼֵ
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

        // ȷ������ֵ׼ȷ
        targetLight2D.intensity = originalIntensity;
        targetLight2D.color = originalColor;

        if (targetLight2D.lightType == Light2D.LightType.Point)
        {
            targetLight2D.pointLightOuterRadius = originalOuterRadius;
            targetLight2D.pointLightInnerRadius = originalInnerRadius;
        }
    }

    // ���ƹ������Ƿ�֧�ְ뾶�仯
    public bool SupportsRadiusChanges()
    {
        return targetLight2D != null && targetLight2D.lightType == Light2D.LightType.Point;
    }

    // ���ӻ�����
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

        // ���Ƶ��ƹ������
        if (targetLight2D != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetLight2D.transform.position);
        }
    }
}