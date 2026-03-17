using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Assets.Script.PlayerSpawn.Characters;

public class Init : MonoBehaviour
{
   public static Character Player; 
    void Start()
    {
        GameObject selectedCharacter = CharacterSelect.selectCharacter;
        Vector3 spawnPos = transform.position;
        spawnPos.z = 0f;

        GameObject characterClone = Instantiate(selectedCharacter, spawnPos, Quaternion.identity);
        Rigidbody2D rb = characterClone.GetComponent<Rigidbody2D>();
        rb.freezeRotation = true;
        SpriteRenderer sr = characterClone.GetComponent<SpriteRenderer>();
        sr.sortingLayerName = "Player";
        sr.sortingOrder = 0;

        Debug.Log(selectedCharacter.name);
        switch (selectedCharacter.name)
        {
            
            case "Samurai_Archer":
                Player = new Samurai_Archer(characterClone);
                break;
            case "Samurai_Commander":
                Player = new Samurai_Commander(characterClone);
                break;
            case "Fighter":
                Player = new Fighter(characterClone);
                break;
            case "Samurai":
                Player = new Samurai(characterClone);
                break;
        }
    }
}
