using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelectPvsP : MonoBehaviour
{
    private int currentPlayerSelecting = 1; // Bắt đầu là Player 1 chọn trước

    // Hàm này sẽ được gọi khi bạn bấm vào nút chọn nhân vật
    public void SelectCharacter(int characterIndex)
    {
        if (currentPlayerSelecting == 1)
        {
            GameData.player1Index = characterIndex; // Lưu nhân vật cho Player 1
            currentPlayerSelecting = 2; // Chuyển lượt cho Player 2
            Debug.Log("Player 1 đã chọn nhân vật số: " + characterIndex + ". Tới lượt Player 2!");
            // (Tùy chọn) Ở đây bạn có thể đổi text trên màn hình thành "Player 2 Select"
        }
        else if (currentPlayerSelecting == 2)
        {
            GameData.player2Index = characterIndex; // Lưu nhân vật cho Player 2
            Debug.Log("Player 2 đã chọn nhân vật số: " + characterIndex + ". Vào game thôi!");

            // Tải Scene chiến đấu (Nhớ đổi tên "BattleScene" thành tên Scene game của bạn)
            SceneManager.LoadScene("PvsP_Scene");
        }
    }
}
