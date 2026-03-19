using UnityEngine;

public class Character
{
    // === Thông số chuyển động =  ==
    public float moveSpeed = 6f;
    public float jumpForce = 5f;
    public float doubleJumpForce = 8f;

    protected Rigidbody2D rb;
    protected Animator animator;
    protected SpriteRenderer spriteRenderer;
    protected Transform transform;

    protected LayerMask groundLayer;
    protected Transform groundCheck;
    protected float groundCheckRadius = 0.2f;

    protected bool isGrounded = true;
    protected int jumpCount = 0;

    // === Hệ thống Combo ===
    protected int lPressCount = 0;
    protected float lastLClickTime = 0f;
    protected float comboWindow = 0.6f;

    // === Hệ thống Chiến đấu (Dùng Tag "Enemy") ===
    public float attackRange = 1f;
    public float attackDamage = 5f;
    public Transform attackPoint;

    private HpAndMpPlayer myEnergy;
    public bool isBlocking =false;


    public Character(GameObject obj, Transform groundCheck, LayerMask groundLayer)
    {
        rb = obj.GetComponent<Rigidbody2D>();
        animator = obj.GetComponent<Animator>();
        spriteRenderer = obj.GetComponent<SpriteRenderer>();
        myEnergy = obj.GetComponent<HpAndMpPlayer>();
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

    private float stayOnGroundTime = 0f; // Biến đếm thời gian đứng yên

    private void CheckGround()
    {
        // Kiểm tra nếu vận tốc Y gần bằng 0 (nhân vật đang đứng yên trên sàn)
        // Dùng Abs để tính cả trường hợp đứng yên tuyệt đối
        if (Mathf.Abs(rb.velocity.y) < 0.05f)
        {
            stayOnGroundTime += Time.deltaTime;

            // Nếu đứng yên đủ lâu (khoảng 0.05s đến 0.1s) thì reset số lần nhảy
            if (stayOnGroundTime > 0.05f)
            {
                isGrounded = true;
                jumpCount = 0;
            }
        }
        else
        {
            // Nếu vận tốc Y khác 0 (đang bay hoặc đang rơi)
            stayOnGroundTime = 0f;
            isGrounded = false;
        }

        animator?.SetBool("IsGrounded", isGrounded);
    }

    protected void HandleJump()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Cho phép nhảy nếu đang ở trạng thái "ước lượng là chạm đất"
            // Hoặc đang ở trên không nhưng mới nhảy 1 lần
            if (isGrounded || (jumpCount > 0 && jumpCount < 2))
            {
                jumpCount++;

                // Reset vận tốc Y về 0 trước khi nhảy để lực nhảy lần 2 luôn mạnh
                rb.velocity = new Vector2(rb.velocity.x, 0);

                float force = (jumpCount == 1) ? jumpForce : doubleJumpForce;
                rb.AddForce(Vector2.up * force, ForceMode2D.Impulse);

                animator?.SetTrigger("Jump");

                // Sau khi bấm nhảy, lập tức hủy trạng thái chạm đất
                isGrounded = false;
                stayOnGroundTime = 0f;
            }
        }
    }

    protected void HandleMovement()
    {
        float moveX = Input.GetAxisRaw("Horizontal"); // GetAxisRaw nhạy hơn cho 2D
        rb.velocity = new Vector2(moveX * moveSpeed, rb.velocity.y);

        if (moveX != 0) spriteRenderer.flipX = moveX < 0;
        animator?.SetBool("IsRunning", moveX != 0);
    }


    protected void HandleCombat()
    {
        // Reset combo nếu bấm quá chậm
        if (Time.time - lastLClickTime > comboWindow) lPressCount = 0;

        if (Input.GetKeyDown(KeyCode.J))
        {
            animator?.SetTrigger("Attack");
            //DealDamage(attackDamage, false);
            lPressCount = 0;
        }

        if (Input.GetKeyDown(KeyCode.K))
        {
            animator?.SetTrigger("Attack2");
            //DealDamage(attackDamage, false);
            lPressCount = 0;
        }

        //if (Input.GetKeyDown(KeyCode.L))
        //{
        //    lPressCount++;
        //    lastLClickTime = Time.time;
        //    animator?.SetTrigger("Attack"); // H   iển thị anim đánh thường trước

        //    if (lPressCount == 1)
        //    {
        //        animator?.SetTrigger("Combo");
        //        //DealDamage(attackDamage * 1.5f, true);
        //        lPressCount = 0;
        //    }
        //}

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (myEnergy == null || myEnergy.currentEnergy < myEnergy.maxEnergy)
            {
                Debug.Log("Chưa đủ năng lượng để dùng combo!");
                return;
            }
            animator?.SetTrigger("Attack"); // anim thường trước

            animator?.SetTrigger("Combo");
            myEnergy.ResetEnergy();

        }

    }
    private void HandleBlock()
    {
        // I = block
        isBlocking = Input.GetKey(KeyCode.I);
        // Nếu có animator trong Character, có thể set trigger hoặc bool
        if (animator != null)
        {
            animator.SetBool("Block", isBlocking);
        }
    }

    // HÀM QUAN TRỌNG: Quét theo Tag thay vì Layer
    public void DealDamage(float damage, bool isCombo = false)
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
                    enemyScript.GainEnergy(damage);
                }

                if (myEnergy != null)
                {
                    myEnergy.GainEnergy(damage*2);
                }
            }
        }
    }
}