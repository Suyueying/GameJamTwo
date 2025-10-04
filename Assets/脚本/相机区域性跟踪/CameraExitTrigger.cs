using UnityEngine;

public class CameraAreaTrigger : MonoBehaviour
{
    [Header("Trigger Settings")]
    public int priority = 0; // 优先级，数值越高优先级越高
    public bool useCustomZoom = false;
    public float customZoomLevel = 5f;

    // 触发器的碰撞器
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

    // 获取触发器的中心位置
    public Vector3 GetCenterPosition()
    {
        if (triggerCollider != null)
        {
            return triggerCollider.bounds.center;
        }

        return transform.position;
    }

    // 在Scene视图中可视化触发器区域
    void OnDrawGizmosSelected()
    {
        // 获取碰撞器或使用默认大小
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

            // 绘制触发器区域
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(GetCenterPosition(), size);

            // 绘制中心点
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(GetCenterPosition(), 0.2f);
        }
        else
        {
            // 如果没有碰撞器，绘制默认大小的区域
            Gizmos.color = new Color(0, 1, 0, 0.3f);
            Gizmos.DrawCube(transform.position, Vector3.one);

            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, 0.2f);
        }

        // 绘制优先级标签
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