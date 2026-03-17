using UnityEngine;

namespace Assets.Script.PlayerSpawn.Characters
{
    public class Fighter : Character
    {
        public Fighter(GameObject obj) : base(obj)
        {
            speed = 8f;
        }

        protected override void Flip()
        {
            if (moveX > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveX < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }
    }
}