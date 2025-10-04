using UnityEngine;
using System.Collections.Generic;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }

    [Header("Camera Settings")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float zoomSpeed = 2f;

    // 存储默认位置和缩放
    private Vector3 defaultPosition;
    private float defaultOrthoSize;

    // 当前激活的触发器列表
    private List<CameraAreaTrigger> activeTriggers = new List<CameraAreaTrigger>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // 记录初始位置和缩放作为默认值
        defaultPosition = transform.position;
        defaultOrthoSize = Camera.main.orthographicSize;
    }

    void LateUpdate()
    {
        // 如果有激活的触发器，移动到最近或优先级最高的触发器中心
        if (activeTriggers.Count > 0)
        {
            // 找到优先级最高的触发器
            CameraAreaTrigger highestPriorityTrigger = GetHighestPriorityTrigger();

            if (highestPriorityTrigger != null)
            {
                // 获取触发器中心位置
                Vector3 targetPosition = highestPriorityTrigger.GetCenterPosition();

                // 保持摄像机原有的Z坐标
                Vector3 targetPosWithZ = new Vector3(
                    targetPosition.x,
                    targetPosition.y,
                    transform.position.z
                );

                // 平滑移动到目标位置
                transform.position = Vector3.Lerp(
                    transform.position,
                    targetPosWithZ,
                    moveSpeed * Time.deltaTime
                );

                // 处理缩放
                if (highestPriorityTrigger.useCustomZoom)
                {
                    float targetZoom = highestPriorityTrigger.customZoomLevel;
                    Camera.main.orthographicSize = Mathf.Lerp(
                        Camera.main.orthographicSize,
                        targetZoom,
                        zoomSpeed * Time.deltaTime
                    );
                }
            }
        }
        else
        {
            // 没有激活的触发器，返回默认位置和缩放
            Vector3 defaultPosWithZ = new Vector3(
                defaultPosition.x,
                defaultPosition.y,
                transform.position.z
            );

            transform.position = Vector3.Lerp(
                transform.position,
                defaultPosWithZ,
                moveSpeed * Time.deltaTime
            );

            Camera.main.orthographicSize = Mathf.Lerp(
                Camera.main.orthographicSize,
                defaultOrthoSize,
                zoomSpeed * Time.deltaTime
            );
        }
    }

    // 添加激活的触发器
    public void AddActiveTrigger(CameraAreaTrigger trigger)
    {
        if (!activeTriggers.Contains(trigger))
        {
            activeTriggers.Add(trigger);
        }
    }

    // 移除激活的触发器
    public void RemoveActiveTrigger(CameraAreaTrigger trigger)
    {
        if (activeTriggers.Contains(trigger))
        {
            activeTriggers.Remove(trigger);
        }
    }

    // 获取优先级最高的触发器
    private CameraAreaTrigger GetHighestPriorityTrigger()
    {
        if (activeTriggers.Count == 0) return null;

        CameraAreaTrigger highestPriority = activeTriggers[0];

        for (int i = 1; i < activeTriggers.Count; i++)
        {
            if (activeTriggers[i].priority > highestPriority.priority)
            {
                highestPriority = activeTriggers[i];
            }
        }

        return highestPriority;
    }
}