using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    // 移动速度
    public float moveSpeed = 5f;

    // 跳跃力量
    public float jumpForce = 10f;
    public float doubleJumpForce = 8f; // 二段跳力量，略小于一段跳

    // 地面检测相关变量
    public Transform groundCheck;
    public float checkRadius = 0.2f;
    public LayerMask whatIsGround;

    // 二段跳相关变量
    public int maxJumps = 2; // 最大跳跃次数
    private int jumpsRemaining; // 剩余跳跃次数
    private bool isDoubleJumping = false; // 标记是否正在进行二段跳

    // 粒子效果相关变量
    public ParticleSystem doubleJumpParticles; // 二段跳粒子效果
    public Transform particleSpawnPoint; // 粒子效果生成点（如果不设置，默认为角色位置）

    // 组件引用
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    // 状态变量
    private bool isGrounded;
    private float moveInput;
    private bool facingRight = true;

    void Start()
    {
        // 获取组件引用
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        // 初始化跳跃次数
        jumpsRemaining = maxJumps;

        // 如果没有指定粒子生成点，使用角色位置
        if (particleSpawnPoint == null)
        {
            particleSpawnPoint = transform;
        }
    }

    void Update()
    {
        // 检测是否在地面上
        bool wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);

        // 如果刚落地，重置跳跃次数和跳跃状态
        if (!wasGrounded && isGrounded)
        {
            jumpsRemaining = maxJumps;
            isDoubleJumping = false;
            // 触发落地动画事件
            if (animator != null)
            {
                animator.SetTrigger("Landed");
            }
        }

        // 获取水平输入
        moveInput = Input.GetAxisRaw("Horizontal");

        // 移动玩家
        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);

        // 处理跳跃
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                // 在地面上跳跃 - 使用正常跳跃力量
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                jumpsRemaining--;
                isDoubleJumping = false;
            }
            else if (jumpsRemaining > 0)
            {
                // 在空中进行二段跳 - 使用较小的跳跃力量
                // 保持水平动量但重置垂直速度，使二段跳更可控
                float currentXVelocity = rb.velocity.x;
                rb.velocity = new Vector2(currentXVelocity, doubleJumpForce);
                jumpsRemaining--;
                isDoubleJumping = true;

                // 触发二段跳动画事件
                if (animator != null)
                {
                    animator.SetTrigger("DoubleJump");
                }

                // 播放二段跳粒子效果
                PlayDoubleJumpParticles();
            }
        }

        // 处理角色朝向
        if (moveInput > 0 && !facingRight)
        {
            Flip();
        }
        else if (moveInput < 0 && facingRight)
        {
            Flip();
        }

        // 设置动画参数
        if (animator != null)
        {
            animator.SetFloat("Speed", Mathf.Abs(moveInput));
            animator.SetBool("IsGrounded", isGrounded);
            animator.SetFloat("VerticalVelocity", rb.velocity.y);
            animator.SetInteger("JumpsRemaining", jumpsRemaining);
            animator.SetBool("IsDoubleJumping", isDoubleJumping);
        }
    }

    // 播放二段跳粒子效果
    void PlayDoubleJumpParticles()
    {
        if (doubleJumpParticles != null)
        {
            // 根据角色朝向调整粒子方向
            Vector3 particlePosition = particleSpawnPoint.position;

            // 实例化粒子效果
            ParticleSystem particles = Instantiate(
                doubleJumpParticles,
                particlePosition,
                Quaternion.identity
            );

            // 根据角色朝向调整粒子方向
            ParticleSystem.ShapeModule shape = particles.shape;
            if (!facingRight)
            {
                // 如果角色朝左，翻转粒子效果
                shape.rotation = new Vector3(0, 180, 0);
            }

            // 播放粒子效果
            particles.Play();

            // 粒子播放完成后自动销毁
            Destroy(particles.gameObject, particles.main.duration);
        }
        else
        {
            Debug.LogWarning("二段跳粒子效果未分配!");
        }
    }

    // 翻转角色朝向
    void Flip()
    {
        facingRight = !facingRight;

        // 使用spriteRenderer.flipX实现翻转
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = !spriteRenderer.flipX;
        }
        else
        {
            // 如果没有获取到spriteRenderer，尝试获取一次
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = !spriteRenderer.flipX;
            }
            else
            {
                // 如果还是没有spriteRenderer，回退到scale方法
                Debug.LogWarning("SpriteRenderer未找到，使用scale方法翻转");
                Vector3 scaler = transform.localScale;
                scaler.x *= -1;
                transform.localScale = scaler;
            }
        }
    }

    // 可视化地面检测范围
    void OnDrawGizmosSelected()
    {
        if (groundCheck == null) return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(groundCheck.position, checkRadius);
    }

    // 公共方法：重置跳跃次数（可用于技能或道具）
    public void ResetJumps()
    {
        jumpsRemaining = maxJumps;
        isDoubleJumping = false;
    }

    // 公共方法：增加额外跳跃次数（可用于技能或道具）
    public void AddExtraJump()
    {
        jumpsRemaining++;
    }

    // 公共方法：设置最大跳跃次数（可用于技能或道具）
    public void SetMaxJumps(int newMax)
    {
        maxJumps = newMax;
        if (jumpsRemaining > maxJumps)
        {
            jumpsRemaining = maxJumps;
        }
    }
}
