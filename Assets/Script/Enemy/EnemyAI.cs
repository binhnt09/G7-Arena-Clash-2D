using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float enemySpeed = 2f;
    [SerializeField] private float attackRange = 1f;

    [Header("Combat & Random Attacks")]
    [SerializeField] private float attackCooldown = 0.8f; // Khoảng nghỉ giữa các đòn đánh
    [SerializeField] private float stopDurationAfterAttack = 0.8f; // dừng lại sau khi đánh

    private Transform target;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer;

    private float lastAttackTime;
    private bool isBusy = false;

    void Awake()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null) target = player.transform;
        //target = FindObjectOfType<Player>().transform;

        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }
    void Update()
    {
        MoveToEnemy();
    }
    private void MoveToEnemy()
    {
        if (target == null) return;

        float distanceToPlayer = Vector2.Distance(transform.position, target.position);
        float direction = (target.position.x > transform.position.x) ? 1 : -1; // Luôn xác định hướng về phía Player
        if (isBusy)
        {
            rb.velocity = new Vector2(0, rb.velocity.y);
            animator.SetBool("IsRunning", false);
            return;
        }


        if (target.position.y > transform.position.y + 2f && Mathf.Abs(rb.velocity.y) < 0.1f)
        {
            Jump();
        }
        float xDistance = Mathf.Abs(transform.position.x - target.position.x);

        if (xDistance <= attackRange && Time.time >= lastAttackTime + attackCooldown)
        {
            StartCoroutine(AttackRoutine());
        }
        else
        {
            rb.velocity = new Vector2(direction * enemySpeed, rb.velocity.y); // luôn đuổi theo ko dừng
            spriteRenderer.flipX = direction > 0; // hướng mặt theo hướng run
            animator.SetBool("IsRunning", true);
        }
        animator.SetBool("IsGrounded", Mathf.Abs(rb.velocity.y) < 0.01f);

    }
    IEnumerator AttackRoutine()
    {
        isBusy = true;
        animator.SetBool("IsRunning", false);
        //animator.SetTrigger("Attack");

        int randomChoice = Random.Range(1, 5);

        if (randomChoice == 1) yield return StartCoroutine(SingleAttack("Attack"));
        else if (randomChoice == 2) yield return StartCoroutine(SingleAttack("Attack2"));
        else if (randomChoice == 3) yield return StartCoroutine(SingleAttack("Attack3"));
        else if (randomChoice == 4)
        {
            yield return StartCoroutine(SingleAttack("Attack"));
            yield return StartCoroutine(SingleAttack("Attack2"));
            yield return StartCoroutine(SingleAttack("Attack3"));
        }

        lastAttackTime = Time.time;

        //yield return new WaitForSeconds(attackCooldown);

        yield return new WaitForSeconds(stopDurationAfterAttack);

        isBusy = false; // Sau khi đánh xong mới cho phép đuổi tiếp
    }
    IEnumerator SingleAttack(string attackName)
    {
        animator.SetTrigger(attackName);

        yield return null;
        
        float animLength = animator.GetCurrentAnimatorStateInfo(0).length;

        float waitTime = Mathf.Max(0f, animLength - 0.05f);
        yield return new WaitForSeconds(waitTime);
    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, 8f);
        animator.SetTrigger("Jump");
    }
}
