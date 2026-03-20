using System;
using UnityEngine;

namespace Assets.Script.PlayerSpawn
{
    public class PlayerController : MonoBehaviour
    {
        public Character player { get; set; }

        void Start()
        {
            player = Init.Player;
        }

        void Update()
        {
            if (Init.Player != null)
            {
                Init.Player.Update(); // gọi update trong Character
            }

            if (Init.Player != null && Init.Player.isBlocking)
            {
                Debug.Log("Player đang block => Đã được giam nửa sát thương!");
            }
        }

        public void DealDamage(float damage)
        {
            player.DealDamage(damage);
        }
        private void OnDrawGizmos()
        {
            // Kiểm tra nếu player đã được khởi tạo và có attackPoint
            if (Init.Player != null && Init.Player.attackPoint != null)
            {
                Gizmos.color = Color.red; // Màu đỏ
                Gizmos.DrawWireSphere(Init.Player.attackPoint.position, Init.Player.attackRange);
            }
            // Hoặc dùng biến local player nếu Init.Player chưa có
            else if (player != null && player.attackPoint != null)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawWireSphere(player.attackPoint.position, player.attackRange);
            }
        }
    }
}