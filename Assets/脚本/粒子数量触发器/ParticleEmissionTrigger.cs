using UnityEngine;

public class ParticleEmissionTrigger : MonoBehaviour
{
    [Header("����ϵͳ����")]
    public ParticleSystem targetParticleSystem;  // Ŀ������ϵͳ
    public float newEmissionRate = 50f;          // �µķ�������

    [Header("��������")]
    public string triggerTag = "Player";         // �������ı�ǩ
    public bool resetOnExit = false;             // �뿪ʱ�Ƿ�����
    public float resetDelay = 0f;                // �����ӳ�ʱ��

    [Header("ԭʼֵ����")]
    public float originalEmissionRate;           // ԭʼ��������

    private bool hasBeenTriggered = false;

    void Start()
    {
        // ����ԭʼ��������
        if (targetParticleSystem != null)
        {
            var emission = targetParticleSystem.emission;
            originalEmissionRate = emission.rateOverTime.constant;

            Debug.Log($"���Ӵ�������ʼ�� - ԭʼ��������: {originalEmissionRate}");
        }
        else
        {
            Debug.LogWarning("Ŀ������ϵͳδ���ã�");
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // ����ǩ
        if (!other.CompareTag(triggerTag)) return;

        // �������ϵͳ
        if (targetParticleSystem == null) return;

        if (!hasBeenTriggered)
        {
            ChangeParticleEmission(newEmissionRate);
            hasBeenTriggered = true;

            Debug.Log($"�������ӷ������ʸı�: {newEmissionRate}");
        }
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        // ����ǩ
        if (!other.CompareTag(triggerTag)) return;

        // ����Ƿ���Ҫ����
        if (resetOnExit && targetParticleSystem != null)
        {
            if (resetDelay > 0)
            {
                Invoke("ResetParticleEmission", resetDelay);
            }
            else
            {
                ResetParticleEmission();
            }

            hasBeenTriggered = false;
        }
    }

    // �ı����ӷ�������
    public void ChangeParticleEmission(float emissionRate)
    {
        if (targetParticleSystem == null) return;

        var emission = targetParticleSystem.emission;
        emission.rateOverTime = emissionRate;

        Debug.Log($"���ӷ��������Ѹ�Ϊ: {emissionRate}");
    }

    // �������ӷ�������
    public void ResetParticleEmission()
    {
        if (targetParticleSystem == null) return;

        var emission = targetParticleSystem.emission;
        emission.rateOverTime = originalEmissionRate;

        Debug.Log($"���ӷ�������������Ϊ: {originalEmissionRate}");
    }

    // �ֶ������ı䷢������
    public void TriggerEmissionChange()
    {
        ChangeParticleEmission(newEmissionRate);
    }

    // �ֶ���������
    public void TriggerReset()
    {
        ResetParticleEmission();
    }

    // �����µķ�������
    public void SetNewEmissionRate(float rate)
    {
        newEmissionRate = rate;
    }

    // ���ӻ�����
    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0.5f, 0.8f, 1f, 0.3f);

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

        // ���Ƶ�����ϵͳ������
        if (targetParticleSystem != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(transform.position, targetParticleSystem.transform.position);
        }
    }
}