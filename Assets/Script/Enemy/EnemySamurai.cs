using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySamurai : MonoBehaviour
{
    [Header("Player Settings")]
    public bool isPlayer2 = false; // Biến này để BattleManager tự động tick

    [Header("Movement Setup")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Hitbox & Damage Setup")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private float[] attackDamages = { 5f, 10f, 15f };
    [SerializeField] private float[] energyGains = { 10f, 15f, 25f };

    // Phím bấm di chuyển
    private KeyCode moveLeft;
    private KeyCode moveRight;
    private KeyCode jumpKey;

    // Phím bấm chiến đấu
    private KeyCode attack1Key;
    private KeyCode attack2Key;
    private KeyCode comboKey;
    private KeyCode hurtKey;

    private HpAndMpEnemy myEnergy;
    private bool isDoingCombo = false;
    private bool isBusy = false;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myEnergy = GetComponent<HpAndMpEnemy>();
    }

    void Start()
    {
        // Gán phím cho Player 1 và Player 2 ở Start (sau khi BattleManager đã tick isPlayer2)
        if (!isPlayer2)
        {
            // --- PLAYER 1 (Phím Chữ) ---
            moveLeft = KeyCode.A;
            moveRight = KeyCode.D;
            jumpKey = KeyCode.Space;

            attack1Key = KeyCode.J;
            attack2Key = KeyCode.K;
            comboKey = KeyCode.L;
            hurtKey = KeyCode.U;
        }
        else
        {
            // --- PLAYER 2 (Phím Mũi Tên & Numpad) ---
            moveLeft = KeyCode.LeftArrow;
            moveRight = KeyCode.RightArrow;
            jumpKey = KeyCode.UpArrow;

            attack1Key = KeyCode.Keypad1;
            attack2Key = KeyCode.Keypad2;
            comboKey = KeyCode.Keypad3;
            hurtKey = KeyCode.Keypad4;
        }
    }

    void Update()
    {
        if (isBusy)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("IsRunning", false);
            return;
        }

        HandleMovement();
        HandleCombatInput();
    }

    private void HandleMovement()
    {
        float moveInput = 0f;

        // Nhận input từ biến KeyCode thay vì phím cứng
        if (Input.GetKey(moveLeft)) moveInput = -1f;
        if (Input.GetKey(moveRight)) moveInput = 1f;

        // Di chuyển
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Xoay mặt và chạy animation
        if (moveInput != 0)
        {
            spriteRenderer.flipX = moveInput > 0;
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        bool isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;
        animator.SetBool("IsGrounded", isGrounded);

        if (Input.GetKeyDown(jumpKey) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }
    }

    private void HandleCombatInput()
    {
        if (Input.GetKeyDown(attack1Key))
        {
            StartCoroutine(PerformSingleAttack("Attack"));
        }
        else if (Input.GetKeyDown(attack2Key))
        {
            StartCoroutine(PerformSingleAttack("Attack2"));
        }
        else if (Input.GetKeyDown(comboKey))
        {
            if (myEnergy != null && myEnergy.currentEnergy >= myEnergy.maxEnergy)
            {
                StartCoroutine(PerformCombo());
            }
            else
            {
                Debug.Log("Chưa đủ năng lượng để dùng Combo!");
            }
        }
        else if (Input.GetKeyDown(hurtKey))
        {
            animator.SetTrigger("HurtEnemy");
        }
    }

    IEnumerator PerformSingleAttack(string attackName)
    {
        isBusy = true;
        isDoingCombo = false;
        animator.SetBool("IsRunning", false);
        animator.SetTrigger(attackName);

        yield return null;

        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(Mathf.Max(0f, animLength - 0.05f));

        isBusy = false;
    }

    IEnumerator PerformCombo()
    {
        isBusy = true;
        isDoingCombo = true;
        animator.SetBool("IsRunning", false);

        animator.SetTrigger("Shield");
        yield return new WaitForSeconds(0.8f);

        if (myEnergy != null) myEnergy.HideAura();

        yield return StartCoroutine(PlayComboPart("Attack"));
        yield return StartCoroutine(PlayComboPart("Attack2"));
        yield return StartCoroutine(PlayComboPart("Attack3"));

        if (myEnergy != null) myEnergy.ResetEnergy();

        isDoingCombo = false;
        isBusy = false;
    }

    IEnumerator PlayComboPart(string attackName)
    {
        animator.SetTrigger(attackName);
        yield return null;
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(Mathf.Max(0f, animLength - 0.05f));
    }

    public void DealDamage(int attackIndex)
    {
        if (attackPoint == null) return;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, targetLayer);

        foreach (Collider2D hitTarget in hitTargets)
        {
            HpAndMpEnemy targetEnergy = hitTarget.GetComponent<HpAndMpEnemy>();

            if (targetEnergy != null && hitTarget.gameObject != this.gameObject)
            {
                targetEnergy.TakeDamageCombo(attackDamages[attackIndex], isDoingCombo);
                targetEnergy.GainEnergy(energyGains[attackIndex]);

                if (myEnergy != null)
                {
                    myEnergy.GainEnergy(energyGains[attackIndex]);
                }
            }
        }
    }

    void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, attackRadius);
    }
}
