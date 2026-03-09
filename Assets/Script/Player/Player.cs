using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    private float moveSpeed = 8f;
    private float jumpForce = 15f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    private bool isGrounded = true;

    private SpriteRenderer spriteRenderer;

    private Rigidbody2D rb;
    Animator animator;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }
    void Start() { }

    private void Update()
    {
        //isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        HandleMovement();
        //HandleJump();
        //HandleCombat();
    }

    //private void HandleJump()
    //{
    //    animator.SetBool("IsGrounded", isGrounded);

    //    if (Input.GetButtonDown("Jump") && isGrounded)
    //    {
    //        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
    //        animator.SetTrigger("Jump");
    //    }
    //}

    private void HandleMovement()
    {
        float playerMoveInput = Input.GetAxis("Horizontal");
        rb.velocity = new Vector2(playerMoveInput * moveSpeed, rb.velocity.y);

        if (playerMoveInput < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (playerMoveInput > 0)
        {
            spriteRenderer.flipX = false;
        }
        //if (playerInput != Vector2.zero)
        //{
        //    animator.SetBool("IsRunning", true);
        //}
        //else
        //{
        //    animator.SetBool("IsRunning", false);
        //}

        //animator.SetBool("IsRunning", moveInput != 0);
    }

    //private void HandleCombat()
    //{
    //    if (Input.GetKeyDown(KeyCode.Z))
    //    {
    //        animator.SetTrigger("Attack");
    //    }
    //}
}
