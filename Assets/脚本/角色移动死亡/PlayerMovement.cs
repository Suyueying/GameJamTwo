using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // �ƶ��ٶ�
    public float moveSpeed = 5f;

    // ��Ծ����
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f; // ��������������С��һ����

    // ��������ر���
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask whatIsGround;

    // ��������ر���
    public int maxJumps = 2; // �����Ծ����
    private int jumpsRemaining; // ʣ����Ծ����
    private bool isDoubleJumping = false; // ����Ƿ����ڽ��ж�����

    // ����Ч����ر���
    public ParticleSystem doubleJumpParticles; // ����������Ч��
    public Transform particleSpawnPoint; // ����Ч�����ɵ㣨��������ã�Ĭ��Ϊ��ɫλ�ã�

    // �������
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // ״̬����
    private bool isGrounded;
    private float moveInput;
    private bool facingRight = true;

    void Start()
    {
        // ��ȡ�������
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // ��ʼ����Ծ����
        jumpsRemaining = maxJumps;

        // ���û��ָ���������ɵ㣬ʹ�ý�ɫλ��
        if (particleSpawnPoint == null)
        {
            particleSpawnPoint = transform;
        }
    }

    void Update()
    {
        // ����Ƿ��ڵ�����
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // �������أ�������Ծ��������Ծ״̬
        if (!wasGrounded && isGrounded)
        {
            jumpsRemaining = maxJumps;
            isDoubleJumping = false;
            // ������ض����¼�
            if (animator != null)
            {
                animator.SetTrigger("Landed");
            }
        }

        // ��ȡˮƽ����
        moveInput = Input.GetAxisRaw("Horizontal");

        // �ƶ����
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // ������Ծ
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // �ڵ�������Ծ - ʹ��������Ծ����
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpsRemaining--;
                isDoubleJumping = false;
            }
            else if (jumpsRemaining > 0)
            {
                // �ڿ��н��ж����� - ʹ�ý�С����Ծ����
                // ����ˮƽ���������ô�ֱ�ٶȣ�ʹ���������ɿ�
                float currentXVelocity = rb.velocity.x;
                rb.velocity = new Vector2(currentXVelocity, doubleJumpForce);
                jumpsRemaining--;
                isDoubleJumping = true;

                // ���������������¼�
                if (animator != null)
                {
                    animator.SetTrigger("DoubleJump");
                }

                // ���Ŷ���������Ч��
                PlayDoubleJumpParticles();
            }
        }

        // �����ɫ����
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        // ���ö�������
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.velocity.y);
            animator.SetInteger("JumpsRemaining", jumpsRemaining);
            animator.SetBool("IsDoubleJumping", isDoubleJumping);
        }
    }

    // ���Ŷ���������Ч��
    void PlayDoubleJumpParticles()
    {
        if (doubleJumpParticles != null)
        {
            // ���ݽ�ɫ����������ӷ���
            Vector3 particlePosition = particleSpawnPoint.position;

            // ʵ��������Ч��
            ParticleSystem particles = Instantiate(
                doubleJumpParticles,
                particlePosition,
                Quaternion.identity
            );

            // ���ݽ�ɫ����������ӷ���
            ParticleSystem.ShapeModule shape = particles.shape;
            if (!facingRight)
            {
                // �����ɫ���󣬷�ת����Ч��
                shape.rotation = new Vector3(0, 180, 0);
            }

            // ��������Ч��
            particles.Play();

            // ���Ӳ�����ɺ��Զ�����
            Destroy(particles.gameObject, particles.main.duration);
        }
        else
        {
            Debug.LogWarning("����������Ч��δ����!");
        }
    }

    // ��ת��ɫ����
    void Flip()
    {
        facingRight = !facingRight;

        // ʹ��spriteRenderer.flipXʵ�ַ�ת
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        else
        {
            // ���û�л�ȡ��spriteRenderer�����Ի�ȡһ��
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
            else
            {
                // �������û��spriteRenderer�����˵�scale����
                Debug.LogWarning("SpriteRendererδ�ҵ���ʹ��scale������ת");
                Vector3 scaler = transform.localScale;
                scaler.x *= -1;
                transform.localScale = scaler;
            }
        }
    }

    // ���ӻ������ⷶΧ
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }

    // ����������������Ծ�����������ڼ��ܻ���ߣ�
    public void ResetJumps()
    {
        jumpsRemaining = maxJumps;
        isDoubleJumping = false;
    }

    // �������������Ӷ�����Ծ�����������ڼ��ܻ���ߣ�
    public void AddExtraJump()
    {
        jumpsRemaining++;
    }

    // �������������������Ծ�����������ڼ��ܻ���ߣ�
    public void SetMaxJumps(int newMax)
    {
        maxJumps = newMax;
        if (jumpsRemaining > maxJumps)
        {
            jumpsRemaining = maxJumps;
        }
    }
}
