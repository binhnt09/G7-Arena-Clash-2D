using UnityEngine;

public abstract class Character
{
    protected Rigidbody2D rb;
    protected Animator animator;
    protected Transform transform;

    protected float moveX;
    protected float speed;

    public Character(GameObject obj)
    {
        rb = obj.GetComponent<Rigidbody2D>();
        animator = obj.GetComponent<Animator>();
        transform = obj.transform;
    }

    public void Move(float inputX)
    {
        moveX = inputX;

        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);

        if (animator != null)
            animator.SetBool("IsRunning", moveX != 0);

        Flip();
    }

    public void Jump(float force)
    {
        rb.velocity = new Vector2(rb.velocity.x, force);

        if (animator != null)
            animator.SetTrigger("Jump");
    }

    protected abstract void Flip();
}