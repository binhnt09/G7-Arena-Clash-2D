using System;
using UnityEngine;

namespace Assets.Script.PlayerSpawn
{
    public class PlayerController : MonoBehaviour
    {
        Character player;

        void Start()
        {
            player = Init.Player;
        }

        void Update()
        {
            if (Init.Player != null)
            {
                Init.Player.Update();
            }
        }
    }
}