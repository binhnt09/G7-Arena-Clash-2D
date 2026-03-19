using UnityEngine;

public class Character
{
    // === Thông số chuyển động ===
    public float moveSpeed = 6f;
    public float jumpForce = 5f;
    public float doubleJumpForce = 8f;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Transform transform;

    protected LayerMask groundLayer;
    protected Transform groundCheck;

    protected bool isGrounded = true;
    protected int jumpCount = 0;

    // === Hệ thống Combo ===
    protected int lPressCount = 0;
    protected float lastLClickTime = 0f;
    protected float comboWindow = 0.6f;

    // === Hệ thống Chiến đấu (Dùng Tag "Enemy") ===
    public float attackRange = 1.5f;
    public float attackDamage = 20f;
    public Transform attackPoint;

    public Character(GameObject obj, Transform groundCheck, LayerMask groundLayer)
    {
        rb = obj.GetComponent<Rigidbody2D>();
        animator = obj.GetComponent<Animator>();
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
        transform = obj.transform;

        this.groundCheck = groundCheck;
        this.groundLayer = groundLayer;

        // Tự động tạo AttackPoint nếu chưa có
        if (attackPoint == null)
        {
            GameObject ap = new GameObject("AttackPoint");
            ap.transform.parent = transform;
            ap.transform.localPosition = new Vector3(1f, 0f, 0f);
            attackPoint = ap.transform;
        }
    }

    public void Update()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        HandleCombat();
        HandleBlock();
        UpdateAttackPointDirection();
    }

    private void UpdateAttackPointDirection()
    {
        if (attackPoint == null || spriteRenderer == null) return;
        // Xoay AttackPoint sang trái/phải dựa vào flipX của Sprite
        float posX = Mathf.Abs(attackPoint.localPosition.x);
        attackPoint.localPosition = new Vector3(spriteRenderer.flipX ? -posX : posX, 0f, 0f);
    }

    private void CheckGround()
    {
        if (groundCheck == null) return;
        // Kiểm tra chạm đất bằng Physics2D
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);
        if (isGrounded && rb.velocity.y <= 0.1f) jumpCount = 0;
        animator?.SetBool("IsGrounded", isGrounded);
    }

    protected void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // GetAxisRaw nhạy hơn cho 2D
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        if (moveX != 0) spriteRenderer.flipX = moveX < 0;
        animator?.SetBool("IsRunning", moveX != 0);
    }

    protected void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                jumpCount = 1;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator?.SetTrigger("Jump");
            }
            else if (jumpCount == 1)
            {
                jumpCount = 2;
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                animator?.SetTrigger("Jump");
            }
        }
    }

    protected void HandleCombat()
    {
        // Reset combo nếu bấm quá chậm
        if (Time.time - lastLClickTime > comboWindow) lPressCount = 0;

        if (Input.GetKeyDown(KeyCode.J))
        {
            animator?.SetTrigger("Attack");
            DealDamage(attackDamage, false);
            lPressCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator?.SetTrigger("Attack2");
            DealDamage(attackDamage, false);
            lPressCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            lPressCount++;
            lastLClickTime = Time.time;
            animator?.SetTrigger("Attack"); // H   iển thị anim đánh thường trước

            if (lPressCount == 3)
            {
                animator?.SetTrigger("Combo");
                DealDamage(attackDamage * 1.5f, true);
                lPressCount = 0;
            }
        }

    }

    protected void HandleBlock()
    {
        bool blocking = Input.GetKey(KeyCode.I);
        animator?.SetBool("Block", blocking);
    }

    // HÀM QUAN TRỌNG: Quét theo Tag thay vì Layer
    private void DealDamage(float damage, bool isCombo = false)
    {
        if (attackPoint == null) return;

        // Quét tất cả vật thể có Collider2D trong tầm đánh
        Collider2D[] hitObjects = Physics2D.OverlapCircleAll(attackPoint.position, attackRange);

        foreach (Collider2D obj in hitObjects)
        {
            // KIỂM TRA TAG "Enemy"
            if (obj.CompareTag("Enemy"))
            {
                HpAndMpEnemy enemyScript = obj.GetComponent<HpAndMpEnemy>();
                if (enemyScript != null)
                {
                    enemyScript.TakeDamageCombo(damage, isCombo);
                    Debug.Log("<color=cyan>[TAG SUCCESS]</color> Đã đấm: " + obj.name);
                }
            }
        }
    }
}