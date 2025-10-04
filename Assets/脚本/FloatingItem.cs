using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("浮动参数")]
    public float floatHeight = 0.5f;    // 浮动高度
    public float floatSpeed = 1f;       // 浮动速度
    public float rotationSpeed = 30f;   // 旋转速度（度/秒）
    public bool rotateWhileFloating = true; // 是否同时旋转

    [Header("浮动模式")]
    public FloatMode floatMode = FloatMode.Smooth;

    public enum FloatMode
    {
        Smooth,     // 平滑正弦波动
        Bounce,     // 弹跳效果
        Hover       // 悬浮效果
    }

    private Vector3 startPosition;
    private float randomOffset;
    private float timeCounter;

    void Start()
    {
        // 保存初始位置
        startPosition = transform.position;

        // 添加随机偏移，使多个物品不会同步浮动
        randomOffset = Random.Range(0f, 2f * Mathf.PI);

        // 初始化时间计数器
        timeCounter = randomOffset;
    }

    void Update()
    {
        timeCounter += Time.deltaTime * floatSpeed;

        // 根据选择的模式计算新的Y位置
        float newY = CalculateNewYPosition();

        // 更新位置
        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        // 如果需要旋转
        if (rotateWhileFloating)
        {
            RotateItem();
        }
    }

    float CalculateNewYPosition()
    {
        switch (floatMode)
        {
            case FloatMode.Smooth:
                // 平滑的正弦波浮动
                return startPosition.y + Mathf.Sin(timeCounter) * floatHeight;

            case FloatMode.Bounce:
                // 弹跳效果（使用绝对值正弦波）
                return startPosition.y + Mathf.Abs(Mathf.Sin(timeCounter)) * floatHeight;

            case FloatMode.Hover:
                // 悬浮效果（使用余弦波，更柔和）
                return startPosition.y + (Mathf.Cos(timeCounter) * 0.5f + 0.5f) * floatHeight;

            default:
                return startPosition.y;
        }
    }

    void RotateItem()
    {
        // 绕Y轴旋转
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // 如果想要更复杂的旋转，可以取消下面的注释
        /*
        transform.Rotate(Vector3.right, rotationSpeed * 0.5f * Time.deltaTime);
        transform.Rotate(Vector3.forward, rotationSpeed * 0.3f * Time.deltaTime);
        */
    }

    // 重置到初始位置（如果需要的话）
    public void ResetPosition()
    {
        transform.position = startPosition;
        timeCounter = randomOffset;
    }

    // 可视化浮动范围（在Scene视图中显示）
    void OnDrawGizmosSelected()
    {
        Vector3 currentPos = Application.isPlaying ? startPosition : transform.position;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(currentPos, 0.1f);
        Gizmos.DrawLine(
            currentPos - Vector3.up * floatHeight,
            currentPos + Vector3.up * floatHeight
        );

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(currentPos - Vector3.up * floatHeight, 0.05f);
        Gizmos.DrawWireSphere(currentPos + Vector3.up * floatHeight, 0.05f);
    }
}