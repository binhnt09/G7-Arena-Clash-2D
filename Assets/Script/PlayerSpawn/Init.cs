using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Assets.Script.PlayerSpawn.Characters;

//public class Init : MonoBehaviour
//{
//   public static Character Player; 
//    void Start()
//    {
//        GameObject selectedCharacter = CharacterSelect.selectCharacter;
//        Vector3 spawnPos = transform.position;
//        spawnPos.z = 0f;

//        GameObject characterClone = Instantiate(selectedCharacter, spawnPos, Quaternion.identity);
//        Rigidbody2D rb = characterClone.GetComponent<Rigidbody2D>();
//        rb.freezeRotation = true;
//        SpriteRenderer sr = characterClone.GetComponent<SpriteRenderer>();
//        sr.sortingLayerName = "Player";
//        sr.sortingOrder = 0;

//        Debug.Log(selectedCharacter.name);
//        switch (selectedCharacter.name)
//        {

//            case "Samurai_Archer":
//                Player = new Samurai_Archer(characterClone);
//                break;
//            case "Samurai_Commander":
//                Player = new Samurai_Commander(characterClone);
//                break;
//            case "Fighter":
//                Player = new Fighter(characterClone);
//                break;
//            case "Samurai":
//                Player = new Samurai(characterClone);
//                break;
//        }
//    }
//}


public class Init : MonoBehaviour
{
    public static Character Player;

    [SerializeField] private LayerMask groundLayer;

    void Start()
    {
        GameObject selectedCharacter = CharacterSelect.selectCharacter;
        Vector3 spawnPos = transform.position;
        spawnPos.z = 0f;

        GameObject characterClone = Instantiate(selectedCharacter, spawnPos, Quaternion.identity);

        var vcam = FindObjectOfType<Cinemachine.CinemachineVirtualCamera>();
        if (vcam != null)
        {
            vcam.Follow = characterClone.transform;
        }

        HpAndMpPlayer playerScript = characterClone.GetComponent<HpAndMpPlayer>();

        // 3. Tìm 2 cái Fill trong Scene và gán vào
        // Nó s? tìm ?úng tên "Hp_Fill" và "Mp_Fill" mà b?n ??t trong Hierarchy
        Image hpImg = GameObject.Find("Hp_Fill").GetComponent<Image>();
        Image mpImg = GameObject.Find("Mp_Fill").GetComponent<Image>();

        playerScript.SetupUI(hpImg, mpImg);

        Rigidbody2D rb = characterClone.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;

        SpriteRenderer sr = characterClone.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Player";
        sr.sortingOrder = 0;

        Transform groundCheck = characterClone.transform.Find("GroundCheck");

        Player = new Character(characterClone, groundCheck, groundLayer);
    }
}
