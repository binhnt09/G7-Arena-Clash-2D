using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager instance;

    [Header("UI Panels")]
    public GameObject endGamePanel; // Kéo Panel_EndGame vào đây
    public TextMeshProUGUI winnerText; // Kéo Text hiển thị người thắng vào đây

    private bool isGameOver = false;

    void Awake()
    {
        instance = this;
    }

    // Sửa lại dòng này trong script BattleManager của bạn
    public void CheckGameOver(string loserName)
    {
        if (isGameOver) return;

        isGameOver = true;
        Time.timeScale = 0.2f;

        // LOGIC ĐÚNG: 
        // Nếu loserName là "Enemy" (Máy thua) -> Người thắng là "NGƯỜI CHƠI"
        // Nếu loserName là "Player" (Người thua) -> Người thắng là "MÁY (AI)"

        //string winner = (loserName == "Enemy") ? "NGƯỜI CHƠI" : "MÁY (AI)";

        //if (winnerText != null)
        //    winnerText.text = winner + " CHIẾN THẮNG!";
        string currentScene = SceneManager.GetActiveScene().name;
        bool isPvP = (currentScene == "PvsP_Scene") || (MenuManager.selectedMode == "Player") || (MenuManager.selectedMode == "Enemy");

        if (winnerText != null)
        {
            // Kiểm tra nếu là màn PvP (Bạn hãy thay đúng tên Scene PvP của bạn vào đây)
            if (isPvP)
            {
                winnerText.text = "K.O";
            }
            else
            {
                // Nếu là màn đấu máy thì mới hiện thông báo thắng thua
                string winner = (loserName == "Enemy") ? "NGƯỜI CHƠI" : "MÁY (AI)";
                winnerText.text = winner + " CHIẾN THẮNG!";
            }
        }

        Invoke("ShowEndPanel", 0.5f);
    }

    void ShowEndPanel()
    {
        Time.timeScale = 0f; // Dừng hẳn game
        if (endGamePanel != null)
            endGamePanel.SetActive(true);
    }
}
