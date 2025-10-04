using UnityEngine;

public class CameraAreaTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public int priority = 0; // ���ȼ�����ֵԽ�����ȼ�Խ��
    public bool useCustomZoom = false;
    public float customZoomLevel = 5f;

    // ����������ײ��
    private Collider2D triggerCollider;

    void Start()
    {
        triggerCollider = GetComponent<Collider2D>();

        if (triggerCollider == null)
        {
            Debug.LogError("CameraAreaTrigger requires a Collider2D component!");
        }
        else if (!triggerCollider.isTrigger)
        {
            Debug.LogWarning("CameraAreaTrigger's Collider2D should be set as a trigger!");
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController.Instance.AddActiveTrigger(this);
        }
    }

    void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            CameraController.Instance.RemoveActiveTrigger(this);
        }
    }

    // ��ȡ������������λ��
    public Vector3 GetCenterPosition()
    {
        if (triggerCollider != null)
        {
            return triggerCollider.bounds.center;
        }

        return transform.position;
    }

    // ��Scene��ͼ�п��ӻ�����������
    void OnDrawGizmosSelected()
    {
        // ��ȡ��ײ����ʹ��Ĭ�ϴ�С
        Collider2D collider = GetComponent<Collider2D>();
        Vector3 size = Vector3.one;

        if (collider != null)
        {
            if (collider is BoxCollider2D)
            {
                BoxCollider2D boxCollider = (BoxCollider2D)collider;
                size = boxCollider.size;
                size.z = 0.1f;
            }
            else
            {
                size = collider.bounds.size;
            }

            // ���ƴ���������
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(GetCenterPosition(), size);

            // �������ĵ�
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetCenterPosition(), 0.2f);
        }
        else
        {
            // ���û����ײ��������Ĭ�ϴ�С������
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position, Vector3.one);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }

        // �������ȼ���ǩ
#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            GetCenterPosition(),
            "Priority: " + priority,
            new GUIStyle()
            {
                normal = new GUIStyleState() { textColor = Color.white },
                fontSize = 10
            }
        );
#endif
    }
}