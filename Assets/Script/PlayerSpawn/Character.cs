using UnityEngine;

public class Character
{
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

    // combo
    protected int lPressCount = 0;
    protected float lastLClickTime = 0f;
    protected float comboWindow = 0.6f;


    // Trong class Character
    [Header("Combat Settings")]
    public float attackRange = 1f; // bán kính tấn công
    public float attackDamage = 20f; // sát thương cơ bản
    public LayerMask enemyLayer; // Layer của Enemy để quét
    public Transform attackPoint; // điểm phát ra attack (vị trí tay hoặc thân)

    public Character(GameObject obj, Transform groundCheck, LayerMask groundLayer)
    {
        rb = obj.GetComponent<Rigidbody2D>();
        animator = obj.GetComponent<Animator>();
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
        transform = obj.transform;

        this.groundCheck = groundCheck;
        this.groundLayer = groundLayer;
    }

    public void Update()
    {
        CheckGround();
        HandleMovement();
        HandleJump();
        HandleCombat();
        HandleBlock();
    }

    // ================= GROUND =================
    private void CheckGround()
    {
        if (groundCheck == null) return;

        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpCount = 0;
        }

        if (animator != null)
            animator.SetBool("IsGrounded", isGrounded);
    }

    // ================= MOVE =================
    protected void HandleMovement()
    {
        float moveX = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        if (moveX < 0) spriteRenderer.flipX = true;
        else if (moveX > 0) spriteRenderer.flipX = false;

        animator.SetBool("IsRunning", moveX != 0);
    }

    // ================= JUMP =================
    protected void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (isGrounded)
            {
                jumpCount = 1;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetTrigger("Jump");
            }
            else if (jumpCount == 1)
            {
                jumpCount = 2;
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                animator.SetTrigger("Jump");
            }
        }
    }

    // ================= COMBAT =================
    protected void HandleCombat()
    {
        if (Time.time - lastLClickTime > comboWindow)
        {
            lPressCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.J))
        {
            animator.SetTrigger("Attack");
            DealDamage(attackDamage); // trừ HP khi attack
            lPressCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator.SetTrigger("Attack2");
            DealDamage(attackDamage); // trừ HP khi attack 2
            lPressCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.L))
        {
            lPressCount++;
            lastLClickTime = Time.time;

            if (lPressCount == 3)
            {
                animator.SetTrigger("Combo");
                DealDamage(attackDamage * 1.5f); // combo mạnh hơn
                lPressCount = 0;
            }
        }
    }

    // ================= BLOCK =================
    protected void HandleBlock()
    {
        bool blocking = Input.GetKey(KeyCode.I);
        animator?.SetBool("Block", blocking);
    }

    private void DealDamage(float damage)
    {
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, enemyLayer);

        foreach (Collider2D enemy in hitEnemies)
        {
            HpAndMpEnemy enemyScript = enemy.GetComponent<HpAndMpEnemy>();
            if (enemyScript != null)
            {
                enemyScript.TakeDamage(damage);
            }
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }
}