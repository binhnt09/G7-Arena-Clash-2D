using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Move")]
    [SerializeField] float moveSpeed = 6f;

    [Header("Jump")]
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float doubleJumpForce = 8f;
    [SerializeField] LayerMask groundLayer;
    [SerializeField] Transform groundCheck;

    Rigidbody2D rb;
    SpriteRenderer sr;
    Animator anim;

    bool isGrounded;
    int jumpCount = 0;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        CheckGround();
        Move();
        Jump();
    }

    void CheckGround()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

        if (isGrounded && rb.velocity.y <= 0)
        {
            jumpCount = 0;
        }

        if (anim != null)
            anim.SetBool("IsGrounded", isGrounded);
    }

    void Move()
    {
        float input = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(input * moveSpeed, rb.velocity.y);

        if (input < 0) sr.flipX = true;
        else if (input > 0) sr.flipX = false;

        if (anim != null)
            anim.SetBool("IsRunning", input != 0);
    }

    void Jump()
    {
        if (Input.GetButtonDown("Jump"))
        {
            if (isGrounded)
            {
                jumpCount = 1;
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                anim?.SetTrigger("Jump");
            }
            else if (jumpCount == 1)
            {
                jumpCount = 2;
                rb.velocity = new Vector2(rb.velocity.x, doubleJumpForce);
                anim?.SetTrigger("Jump");
            }
        }
    }
}