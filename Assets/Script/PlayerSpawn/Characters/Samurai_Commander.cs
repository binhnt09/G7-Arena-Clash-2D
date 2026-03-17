using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.PlayerSpawn.Characters
{
    public class Samurai_Commander : Character
    {
        public Samurai_Commander(GameObject gameObject) : base(gameObject)
        {
            speed = 10;
        }

        protected override void FlipSprite()
        {
            Vector3 scale = transform.localScale;

            if (moveX > 0)
                scale.x = Mathf.Abs(scale.x);
            else if (moveX < 0)
                scale.x = -Mathf.Abs(scale.x);

            transform.localScale = scale;
        }
    }
}
