using UnityEngine;

public class Samurai_Archer : Character
{
    public Samurai_Archer(GameObject obj) : base(obj)
    {
        speed = 6f;
    }

    protected override void Flip()
    {
        if (moveX > 0)
            transform.localScale = new Vector3(1, 1, 1);
        else if (moveX < 0)
            transform.localScale = new Vector3(-1, 1, 1);
    }
}