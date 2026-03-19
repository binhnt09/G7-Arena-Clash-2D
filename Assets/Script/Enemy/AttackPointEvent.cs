using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackPointEvent : MonoBehaviour
{
    [Header("Movement Setup")]
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 8f;

    [Header("Hitbox & Damage Setup")]
    [SerializeField] private Transform attackPoint;
    [SerializeField] private float attackRadius = 0.8f;
    [SerializeField] private LayerMask targetLayer;

    [SerializeField] private float[] attackDamages = { 5f, 10f, 15f };
    [SerializeField] private float[] energyGains = { 10f, 15f, 25f };

    private HpAndMpEnemy myEnergy; // Lưu ý: Sau này bạn nên đổi tên script này thành CharacterStats cho tổng quát
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

    void Update()
    {
        // Nếu đang thực hiện đòn đánh hoặc bị choáng, không cho phép nhận input di chuyển
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

        // 👉 A / D để di chuyển
        if (Input.GetKey(KeyCode.A)) moveInput = -1f;
        if (Input.GetKey(KeyCode.D)) moveInput = 1f;

        // Di chuyển
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);

        // Xoay mặt + animation
        if (moveInput != 0)
        {
            spriteRenderer.flipX = moveInput < 0; // quay trái khi A
            animator.SetBool("IsRunning", true);
        }
        else
        {
            animator.SetBool("IsRunning", false);
        }

        // Check ground (giữ nguyên logic của bạn)
        bool isGrounded = Mathf.Abs(rb.velocity.y) < 0.01f;
        animator.SetBool("IsGrounded", isGrounded);

        // 👉 Nhảy bằng W
        if (Input.GetKeyDown(KeyCode.W) && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            animator.SetTrigger("Jump");
        }
    }

    private void HandleCombatInput()
    {
        // Phím 1: Attack 1
        if (Input.GetKeyDown(KeyCode.Alpha1) || Input.GetKeyDown(KeyCode.Keypad1))
        {
            StartCoroutine(PerformSingleAttack("Attack"));
        }
        // Phím 2: Attack 2
        else if (Input.GetKeyDown(KeyCode.Alpha2) || Input.GetKeyDown(KeyCode.Keypad2))
        {
            StartCoroutine(PerformSingleAttack("Attack2"));
        }
        // Phím 3: Combo (Yêu cầu đủ năng lượng)
        else if (Input.GetKeyDown(KeyCode.Alpha3) || Input.GetKeyDown(KeyCode.Keypad3))
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
        // Phím 4: Hurt (Thường thì Hurt sẽ được gọi khi bị đối phương đánh trúng, nhưng mình gán vào phím 4 theo yêu cầu để bạn test)
        else if (Input.GetKeyDown(KeyCode.Alpha4) || Input.GetKeyDown(KeyCode.Keypad4))
        {
            animator.SetTrigger("HurtEnemy"); // Bạn cần đảm bảo Animator có param "Hurt"
            // StartCoroutine(HandleHurtState()); // Nếu bạn muốn character bị khựng lại khi ấn phím 4
        }
    }

    IEnumerator PerformSingleAttack(string attackName)
    {
        isBusy = true;
        isDoingCombo = false;
        animator.SetBool("IsRunning", false);
        animator.SetTrigger(attackName);

        yield return null; // Đợi 1 frame để Animator cập nhật

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

        // Chạy chuỗi combo tuần tự
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

    // Hàm này giữ nguyên để gọi trong Animation Event
    public void DealDamage(int attackIndex) 
    {
        if (attackPoint == null) return;

        Collider2D[] hitTargets = Physics2D.OverlapCircleAll(attackPoint.position, attackRadius, targetLayer);

        foreach (Collider2D hitTarget in hitTargets)
        {
            HpAndMpEnemy targetEnergy = hitTarget.GetComponent<HpAndMpEnemy>();

            if (targetEnergy != null && hitTarget.gameObject != this.gameObject) // Tránh tự chém trúng mình
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
