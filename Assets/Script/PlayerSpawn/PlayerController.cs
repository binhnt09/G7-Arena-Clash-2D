using UnityEngine;

namespace Assets.Script.PlayerSpawn
{
    public class PlayerController : MonoBehaviour
    {
        private Character player;

        [SerializeField] private float jumpForce = 5f;
        [SerializeField] private float doubleJumpForce = 8f;
        [SerializeField] private LayerMask groundLayer;
        [SerializeField] private Transform groundCheck;

        private bool isGrounded;
        private int jumpCount = 0;

        void Start()
        {
            //player = Init.Player;
        }

        void Update()
        {
            if (player == null) return;

            HandleMovement();
            HandleJump();
        }

        void HandleMovement()
        {
            float input = Input.GetAxis("Horizontal");
            player.Move(input);
        }

        void HandleJump()
        {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, 0.2f, groundLayer);

            if (isGrounded) jumpCount = 0;

            if (Input.GetButtonDown("Jump"))
            {
                if (isGrounded)
                {
                    jumpCount = 1;
                    player.Jump(jumpForce);
                }
                else if (jumpCount == 1)
                {
                    jumpCount = 2;
                    player.Jump(doubleJumpForce);
                }
            }
        }
    }
}