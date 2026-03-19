using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BattlePvsP : MonoBehaviour
{
    [Header("Danh sách 5 Prefab Nhân Vật")]
    public GameObject[] characterPrefabs; // Bạn sẽ kéo 5 prefab nhân vật vào đây trong Inspector

    [Header("Vị trí xuất hiện")]
    public Transform player1SpawnPoint; // Vị trí đứng của Player 1
    public Transform player2SpawnPoint; // Vị trí đứng của Player 2

    [Header("UI Thanh Máu và Nộ - Player 1")]
    public Image p1HpBar;
    public Image p1MpBar;

    [Header("UI Thanh Máu và Nộ - Player 2")]
    public Image p2HpBar;
    public Image p2MpBar;

    void Start()
    {
        SpawnPlayers();
    }

    private void SpawnPlayers()
    {
        // 1. SINH RA PLAYER 1
        GameObject p1 = Instantiate(characterPrefabs[GameData.player1Index], player1SpawnPoint.position, Quaternion.identity);
        Enemy p1Script = p1.GetComponent<Enemy>();
        if (p1Script != null)
        {
            p1Script.isPlayer2 = false; // Đảm bảo không bị tick
        }
        EnemySamurai p1Samurai = p1.GetComponent<EnemySamurai>();
        if (p1Samurai != null) p1Samurai.isPlayer2 = false;
        HpAndMpEnemy p1Energy = p1.GetComponent<HpAndMpEnemy>();
        if (p1Energy != null)
        {
            p1Energy.healthBarFill = p1HpBar;
            p1Energy.energyBarFill = p1MpBar;
        }

        // 2. SINH RA PLAYER 2
        //GameObject p2 = Instantiate(characterPrefabs[GameData.player2Index], player2SpawnPoint.position, Quaternion.identity);
        GameObject p2 = Instantiate(characterPrefabs[GameData.player2Index], player2SpawnPoint.position, Quaternion.Euler(0, 180, 0));
        Enemy p2Script = p2.GetComponent<Enemy>();
        if (p2Script != null)
        {
            p2Script.isPlayer2 = true; //Tự động tick vào ô isPlayer2
        }
        EnemySamurai p2Samurai = p2.GetComponent<EnemySamurai>();
        if (p2Samurai != null) p2Samurai.isPlayer2 = true;
        HpAndMpEnemy p2Energy = p2.GetComponent<HpAndMpEnemy>();
        if (p2Energy != null)
        {
            p2Energy.healthBarFill = p2HpBar;
            p2Energy.energyBarFill = p2MpBar;
        }
    }
}
