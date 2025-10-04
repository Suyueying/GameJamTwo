using UnityEngine;
using System.Collections.Generic;

public class StableSoftTail : MonoBehaviour
{
    [Header("Tail Structure")]
    [Range(3, 20)] public int segmentCount = 10;
    [Range(0f, 0.5f)] public float segmentLength = 0.2f;
    [Range(0.5f, 0.95f)] public float massFalloff = 0.8f;

    [Header("Physics Settings")]
    [Range(10f, 200f)] public float stiffness = 80f;
    [Range(0f, 0.5f)] public float damping = 0.2f;
    [Range(0f, 10f)] public float gravityScale = 1f;
    [Range(0.5f, 5f)] public float angularDrag = 2f;

    [Header("Visuals")]
    public GameObject tailSegmentPrefab;
    public GameObject tailTipPrefab;
    public bool useLineRenderer = true;

    [Header("Color Gradient")] 
    public bool useColorGradient = true;
    public Color rootColor = Color.red;
    public Color tipColor = Color.blue;
    [Range(0.1f, 5f)] public float colorTransitionSharpness = 1f;

    [Header("Size Gradient")] // 新增大小渐变参数
    public bool useSizeGradient = true;
    [Range(0.1f, 2f)] public float rootSize = 1f;    // 根部大小
    [Range(0.1f, 2f)] public float midSize = 0.6f;   // 中部大小
    [Range(0f, 2f)] public float tipSize = 0.3f;   // 尖端大小
    [Range(0.1f, 5f)] public float sizeTransitionSharpness = 1f; // 大小过渡锐度

    private List<Transform> segments = new List<Transform>();
    private List<Rigidbody2D> segmentRBs = new List<Rigidbody2D>();
    private LineRenderer lineRenderer;
    private Vector2 basePosition;

    void Start()
    {
        basePosition = transform.position;
        InitializeTail();
        
        if (useLineRenderer)
        {
            SetupLineRenderer();
        }
    }

    void Update()
    {
        basePosition = transform.position;
        
        if (useLineRenderer && lineRenderer != null)
        {
            UpdateLineRenderer();
        }
    }

    void FixedUpdate()
    {
        if (segments.Count == 0) return;

        // 固定根部位置
        segments[0].position = basePosition;
        segmentRBs[0].velocity = Vector2.zero;
        segmentRBs[0].angularVelocity = 0f;

        // 应用物理约束
        for (int i = 1; i < segments.Count; i++)
        {
            Rigidbody2D currentRB = segmentRBs[i];
            Rigidbody2D prevRB = segmentRBs[i - 1];
            
            Vector2 targetPos = prevRB.position;
            Vector2 currentPos = currentRB.position;
            Vector2 direction = (targetPos - currentPos).normalized;
            
            float currentDistance = Vector2.Distance(targetPos, currentPos);
            float positionError = currentDistance - segmentLength;
            Vector2 velocityError = prevRB.velocity - currentRB.velocity;
            
            Vector2 springForce = direction * (positionError * stiffness * Time.fixedDeltaTime * 50f);
            Vector2 dampingForce = velocityError * (damping * Time.fixedDeltaTime * 50f);
            
            currentRB.AddForce(springForce + dampingForce, ForceMode2D.Force);
            prevRB.AddForce(-(springForce + dampingForce) * 0.5f, ForceMode2D.Force);
            
            if (currentDistance > segmentLength * 1.2f)
            {
                currentRB.MovePosition(targetPos - direction * segmentLength);
                currentRB.velocity = prevRB.velocity * 0.9f;
            }
        }
    }

    void InitializeTail()
    {
        // 清除现有尾巴
        foreach (var segment in segments)
        {
            if (segment != null) Destroy(segment.gameObject);
        }
        segments.Clear();
        segmentRBs.Clear();

        // 创建新尾巴
        for (int i = 0; i < segmentCount; i++)
        {
            GameObject prefabToUse = (i == segmentCount - 1 && tailTipPrefab != null) ? 
                tailTipPrefab : tailSegmentPrefab;
            
            Vector2 spawnPos = basePosition - (Vector2)transform.right * i * segmentLength * 0.9f;
            spawnPos += Vector2.down * i * 0.05f;
            
            GameObject segment = Instantiate(prefabToUse, spawnPos, Quaternion.identity, transform);
            segment.name = "TailSegment_" + i;

            // 配置物理组件
            Rigidbody2D rb = segment.GetComponent<Rigidbody2D>();
            if (rb == null) rb = segment.AddComponent<Rigidbody2D>();
            
            float massFactor = Mathf.Pow(massFalloff, i);
            rb.mass = Mathf.Lerp(0.3f, 1f, massFactor);
            rb.gravityScale = gravityScale * massFactor;
            rb.drag = 0.5f;
            rb.angularDrag = angularDrag;
            rb.interpolation = RigidbodyInterpolation2D.Interpolate;

            // 禁用碰撞
            Collider2D col = segment.GetComponent<Collider2D>();
            if (col != null) col.enabled = false;

            // 颜色渐变
            if (useColorGradient)
            {
                SpriteRenderer renderer = segment.GetComponent<SpriteRenderer>();
                if (renderer != null)
                {
                    float t = Mathf.Pow((float)i / (segmentCount - 1), colorTransitionSharpness);
                    renderer.color = Color.Lerp(rootColor, tipColor, t);
                }
            }

            // 大小渐变（核心新增逻辑）
            if (useSizeGradient)
            {
                float t = Mathf.Pow((float)i / (segmentCount - 1), sizeTransitionSharpness);
                float size;
                if (t < 0.5f)
                {
                    // 根部到中部：rootSize -> midSize
                    size = Mathf.Lerp(rootSize, midSize, t * 2f);
                }
                else
                {
                    // 中部到尖端：midSize -> tipSize
                    size = Mathf.Lerp(midSize, tipSize, (t - 0.5f) * 2f);
                }
                segment.transform.localScale = new Vector3(size, size, 1f);
            }

            segments.Add(segment.transform);
            segmentRBs.Add(rb);
        }
    }

    void SetupLineRenderer()
    {
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.positionCount = segmentCount;
        lineRenderer.startWidth = rootSize * 0.1f;  // 线宽与根部大小关联
        lineRenderer.endWidth = tipSize * 0.03f;    // 线宽与尖端大小关联
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
        lineRenderer.textureMode = LineTextureMode.Tile;

        if (useColorGradient)
        {
            lineRenderer.colorGradient = CreateGradient(rootColor, tipColor);
        }
    }

    void UpdateLineRenderer()
    {
        for (int i = 0; i < segments.Count; i++)
        {
            lineRenderer.SetPosition(i, segments[i].position);
        }
    }

    private Gradient CreateGradient(Color start, Color end)
    {
        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] { new GradientColorKey(start, 0), new GradientColorKey(end, 1) },
            new GradientAlphaKey[] { new GradientAlphaKey(1, 0), new GradientAlphaKey(1, 1) }
        );
        return gradient;
    }
}