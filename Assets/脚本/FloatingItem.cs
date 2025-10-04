using UnityEngine;

public class FloatingItem : MonoBehaviour
{
    [Header("��������")]
    public float floatHeight = 0.5f;    // �����߶�
    public float floatSpeed = 1f;       // �����ٶ�
    public float rotationSpeed = 30f;   // ��ת�ٶȣ���/�룩
    public bool rotateWhileFloating = true; // �Ƿ�ͬʱ��ת

    [Header("����ģʽ")]
    public FloatMode floatMode = FloatMode.Smooth;

    public enum FloatMode
    {
        Smooth,     // ƽ�����Ҳ���
        Bounce,     // ����Ч��
        Hover       // ����Ч��
    }

    private Vector3 startPosition;
    private float randomOffset;
    private float timeCounter;

    void Start()
    {
        // �����ʼλ��
        startPosition = transform.position;

        // ������ƫ�ƣ�ʹ�����Ʒ����ͬ������
        randomOffset = Random.Range(0f, 2f * Mathf.PI);

        // ��ʼ��ʱ�������
        timeCounter = randomOffset;
    }

    void Update()
    {
        timeCounter += Time.deltaTime * floatSpeed;

        // ����ѡ���ģʽ�����µ�Yλ��
        float newY = CalculateNewYPosition();

        // ����λ��
        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );

        // �����Ҫ��ת
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
                // ƽ�������Ҳ�����
                return startPosition.y + Mathf.Sin(timeCounter) * floatHeight;

            case FloatMode.Bounce:
                // ����Ч����ʹ�þ���ֵ���Ҳ���
                return startPosition.y + Mathf.Abs(Mathf.Sin(timeCounter)) * floatHeight;

            case FloatMode.Hover:
                // ����Ч����ʹ�����Ҳ�������ͣ�
                return startPosition.y + (Mathf.Cos(timeCounter) * 0.5f + 0.5f) * floatHeight;

            default:
                return startPosition.y;
        }
    }

    void RotateItem()
    {
        // ��Y����ת
        transform.Rotate(Vector3.up, rotationSpeed * Time.deltaTime);

        // �����Ҫ�����ӵ���ת������ȡ�������ע��
        /*
        transform.Rotate(Vector3.right, rotationSpeed * 0.5f * Time.deltaTime);
        transform.Rotate(Vector3.forward, rotationSpeed * 0.3f * Time.deltaTime);
        */
    }

    // ���õ���ʼλ�ã������Ҫ�Ļ���
    public void ResetPosition()
    {
        transform.position = startPosition;
        timeCounter = randomOffset;
    }

    // ���ӻ�������Χ����Scene��ͼ����ʾ��
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