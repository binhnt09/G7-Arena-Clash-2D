using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class CharacterSelectPvsP : MonoBehaviour
{
    [Header("Cấu hình Game")]
    public int totalCharacters = 5; // Tổng số nhân vật bạn có (0 đến 4)
    public string battleSceneName = "BattleScene";

    [Header("Trạng thái Player 1")]
    public int p1Index = 0;
    public bool p1Locked = false; // Đã chốt chưa?
    public TextMeshProUGUI p1SelectionText;

    [Header("Trạng thái Player 2")]
    public int p2Index = 0;
    public bool p2Locked = false; // Đã chốt chưa?
    public TextMeshProUGUI p2SelectionText;

    [Header("UI Chung")]
    public TextMeshProUGUI instructionText;

    void Start()
    {
        p1Index = 0;
        p2Index = 0;
        p1Locked = false;
        p2Locked = false;
        UpdateUI();
    }

    void Update()
    {
        // Liên tục kiểm tra phím bấm mỗi khung hình
        HandlePlayer1Input();
        HandlePlayer2Input();
    }

    private void HandlePlayer1Input()
    {
        // Nếu P1 CHƯA chốt thì mới cho phép di chuyển chọn
        if (!p1Locked)
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                p1Index--;
                if (p1Index < 0) p1Index = totalCharacters - 1; // Vòng lại nhân vật cuối
                UpdateUI();
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                p1Index++;
                if (p1Index >= totalCharacters) p1Index = 0; // Vòng lại nhân vật đầu
                UpdateUI();
            }
            // Phím R: XÁC NHẬN
            else if (Input.GetKeyDown(KeyCode.R))
            {
                p1Locked = true;
                GameData.player1Index = p1Index; // Lưu vào Data
                UpdateUI();
                CheckStartGame();
            }
        }
        else // Nếu đã chốt rồi
        {
            // Phím T: HỦY CHỐT
            if (Input.GetKeyDown(KeyCode.T))
            {
                p1Locked = false;
                UpdateUI();
            }
        }
    }

    private void HandlePlayer2Input()
    {
        // Nếu P2 CHƯA chốt thì mới cho phép di chuyển chọn
        if (!p2Locked)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                p2Index--;
                if (p2Index < 0) p2Index = totalCharacters - 1;
                UpdateUI();
            }
            else if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                p2Index++;
                if (p2Index >= totalCharacters) p2Index = 0;
                UpdateUI();
            }
            // Phím N: XÁC NHẬN
            else if (Input.GetKeyDown(KeyCode.N))
            {
                p2Locked = true;
                GameData.player2Index = p2Index; // Lưu vào Data
                UpdateUI();
                CheckStartGame();
            }
        }
        else // Nếu đã chốt rồi
        {
            // Phím M: HỦY CHỐT
            if (Input.GetKeyDown(KeyCode.M))
            {
                p2Locked = false;
                UpdateUI();
            }
        }
    }

    private void UpdateUI()
    {
        // Cập nhật Text cho Player 1
        if (p1SelectionText != null)
        {
            if (p1Locked)
                p1SelectionText.text = "P1 ĐÃ CHỐT: Nhân vật " + p1Index;
            else
                p1SelectionText.text = "P1 đang trỏ: Nhân vật " + p1Index;
        }

        // Cập nhật Text cho Player 2
        if (p2SelectionText != null)
        {
            if (p2Locked)
                p2SelectionText.text = "P2 ĐÃ CHỐT: Nhân vật " + p2Index;
            else
                p2SelectionText.text = "P2 đang trỏ: Nhân vật " + p2Index;
        }

        // Cập nhật hướng dẫn chung
        if (instructionText != null)
        {
            if (p1Locked && p2Locked)
                instructionText.text = "CHUẨN BỊ VÀO TRẬN...";
            else
                instructionText.text = "CHỌN NHÂN VẬT (P1: R/T | P2: N/M)";
        }
    }

    private void CheckStartGame()
    {
        // Nếu cả 2 cùng khóa (Locked) thì Load Scene chiến đấu
        if (p1Locked && p2Locked)
        {
            Debug.Log("Cả hai đã sẵn sàng! Chuyển Scene...");
            SceneManager.LoadScene(battleSceneName);
        }
    }
}
