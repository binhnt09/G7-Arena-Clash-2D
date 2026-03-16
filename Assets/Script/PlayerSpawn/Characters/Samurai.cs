using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace Assets.Script.PlayerSpawn.Characters
{
    public class Samurai:Character
    {
        public Samurai(GameObject gameObject) : base(gameObject) 
        {
            speed = 1;
        }

        protected override void FlipSprite()
        {
            if (moveX > 0)
                transform.localScale = new Vector3(1, 1, 1);
            else if (moveX < 0)
                transform.localScale = new Vector3(-1, 1, 1);
        }

    }
}
