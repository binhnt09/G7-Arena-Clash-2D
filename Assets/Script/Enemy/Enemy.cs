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

    private bool isGrounded = true;
    private int jumpCount = 0;

    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb;
    private Animator animator;

    private KeyCode moveLeft, moveRight, jumpKey, attackKey;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();

        if (!isPlayer2)
        {
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
            jumpKey = KeyCode.Space;
            attackKey = KeyCode.Z;
        }
        else
        {
            moveLeft = KeyCode.LeftArrow;
            moveRight = KeyCode.RightArrow;
            jumpKey = KeyCode.UpArrow;
            //attackKey = KeyCode.Keypad1;
            attackKey = KeyCode.M;
        }
    }
    void Start() { }

    private void Update()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded && rb.velocity.y <= 0) jumpCount = 0;

        HandleMovement();
        HandleJump();
        HandleCombat();
    }

    private void HandleJump()
    {
        animator.SetBool("IsGrounded", isGrounded);

        //if (Input.GetButtonDown("Jump"))
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
        if (Input.GetKeyDown(attackKey))
        {
            animator.SetTrigger("Attack");
        }
    }
}
