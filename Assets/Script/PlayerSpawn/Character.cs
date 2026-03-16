using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using UnityEngine;

public abstract class Character 
{
    public int hp { get; set; }
    public float speed { get; set; }

    public Rigidbody2D body { get; set; }
    public Animator animator { get; set; }
    public BoxCollider2D boxCollider2D { get; set; }
    public Transform transform { get; set; }


    protected float moveX;
    protected float moveY;
    public Character(GameObject gameObject)
    {
        hp = 3;

        body = gameObject.GetComponent<Rigidbody2D>();
        animator = gameObject.GetComponent<Animator>();
        boxCollider2D = gameObject.GetComponent<BoxCollider2D>();
        transform = gameObject.transform;
    }

     public void Update()
    {
        InputHandler();
        Move();
        FlipSprite();
    }

    protected virtual void InputHandler()
    {
        moveX = Input.GetAxis("Horizontal");
        moveY = Input.GetAxis("Vertical");
    }

    protected virtual void Move()
    {
        body.velocity = new Vector2(moveX * speed, moveY * speed);
        Debug.Log(body.velocity);
    }

    protected abstract void FlipSprite();
}