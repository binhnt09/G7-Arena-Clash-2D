using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    // Start is called before the first frame update
    private int index;

    [SerializeField] GameObject[] Characters;
    [SerializeField] TextMeshProUGUI CharacterName;

    [SerializeField] GameObject[] CharacterPrefabs;
    public static GameObject selectCharacter;
    void Start()
    {
        index = 0;
        SelectCharacter();
    }

    public void OnLeftBtnClick()
    {
        if (index > 0)
        {
            index--;
        }
        SelectCharacter();
    }
    public void OnRightBtnClick()
    {
        if (index < Characters.Length - 1)
        {
            index++;
        }
        SelectCharacter();
    }

    public void OnPlayBtnClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    private void SelectCharacter()
    {
        for(int i =0; i< Characters.Length; i++)
        {
            if(i == index)
            {
                Characters[i].GetComponent<SpriteRenderer>().color = Color.white;
                Characters[i].GetComponent<Animator>().enabled = true;
                selectCharacter = CharacterPrefabs[i];
                CharacterName.text = CharacterPrefabs[i].name;
            }
            else
            {
                Characters[i].GetComponent<SpriteRenderer>().color = Color.black;
                Characters[i].GetComponent<Animator>().enabled = false;
            }
        }
    }
}
