using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("Player Settings")]
    [SerializeField] private bool isPlayer2 = false;

    [SerializeField] private float moveSpeed = 6f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float doubleJumpForce = 7f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;

    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.8f;

    private bool isGrounded = true;
    private int jumpCount = 0;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;

    private KeyCode moveLeft, moveRight, jumpKey;
    private KeyCode attack1Key, attack2Key, comboKey, blockKey;

    private int comboPressCount = 0;
    private float lastComboClickTime = 0f;
    public float comboWindow = 0.6f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        // CẤU HÌNH PHÍM CHO 2 PLAYER
        if (!isPlayer2)
        {
            // --- PLAYER 1 (Phím Chữ) ---
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
            jumpKey = KeyCode.Space; // Có thể đổi thành KeyCode.W nếu thích

            attack1Key = KeyCode.J;
            attack2Key = KeyCode.K;
            comboKey = KeyCode.L;
            blockKey = KeyCode.U;
        }
        else
        {
            // --- PLAYER 2 (Phím Số Numpad) ---
            moveLeft = KeyCode.LeftArrow;
            moveRight = KeyCode.RightArrow;
            jumpKey = KeyCode.UpArrow;

            // Dùng Keypad (bàn phím số bên phải). Nếu dùng laptop ko có Keypad, đổi chữ "Keypad" thành "Alpha" (vd: KeyCode.Alpha1)
            attack1Key = KeyCode.Keypad1;
            attack2Key = KeyCode.Keypad2;
            comboKey = KeyCode.Keypad3;
            blockKey = KeyCode.Keypad5;
        }
    }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded && rb.velocity.y <= 0) jumpCount = 0;

        HandleMovement();
        HandleJump();
        HandleCombat();
        HandleBlock();
    }

    private void HandleJump()
    {
        animator.SetBool("IsGrounded", isGrounded);

        if (Input.GetKeyDown(jumpKey))
        {
            if (isGrounded) // Nhảy lần 1
            {
                jumpCount = 1;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                animator.SetTrigger("Jump");
            }
            else if (jumpCount == 1) // Nhảy lần 2 (Double Jump)
            {
                jumpCount = 2;
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce); // Nhảy cao hơn
                animator.SetTrigger("Jump");
            }
        }
    }

    private void HandleMovement()
    {
        float moveDirection = 0;

        if (Input.GetKey(moveLeft)) moveDirection = -1;
        else if (Input.GetKey(moveRight)) moveDirection = 1;

        rb.velocity = new Vector2(moveDirection * moveSpeed, rb.velocity.y);

        if (moveDirection != 0)
        {
            bool flip = moveDirection < 0;

            if (isPlayer2)
            {
                spriteRenderer.flipX = !flip;
            }
            else
            {
                spriteRenderer.flipX = flip;
            }
        }
        animator.SetBool("IsRunning", moveDirection != 0);
    }

    private void HandleCombat()
    {
        // Reset combo nếu quá thời gian window
        if (Time.time - lastComboClickTime > comboWindow)
        {
            comboPressCount = 0;
        }

        if (Input.GetKeyDown(attack1Key))
        {
            animator.SetTrigger("Attack");
            comboPressCount = 0;
        }

        if (Input.GetKeyDown(attack2Key))
        {
            animator.SetTrigger("Attack2");
            comboPressCount = 0;
        }

        // Xử lý cơ chế bấm nút Combo nhiều lần
        if (Input.GetKeyDown(comboKey))
        {
            comboPressCount++;
            lastComboClickTime = Time.time;

            if (comboPressCount == 3)
            {
                animator.SetTrigger("Combo");
                comboPressCount = 0;
            }
        }
    }
    public void DealDamage(float damage) // hàm gắn vào frame attack nhận biết đòn đánh nào
    {
        if (attackPoint == null) return;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius);

        foreach (Collider2D hitTarget in hitTargets)
        {
            HpAndMpPlayer targetEnergy = hitTarget.GetComponent<HpAndMpPlayer>();

            if (targetEnergy != null)
            {
                targetEnergy.TakeDamageCombo(damage, true);
                targetEnergy.GainEnergy(damage);

                //if (myEnergy != null)
                //{
                //    myEnergy.GainEnergy(energyGains[attackIndex]);
                //}
            }
        }
    }

    private void HandleBlock()
    {
        bool blocking = Input.GetKey(blockKey);
        animator.SetBool("Block", blocking);
    }
}