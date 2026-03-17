using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class CharacterSelect : MonoBehaviour
{
    private int index;

    [SerializeField] GameObject[] Characters; // model hi?n th?
    [SerializeField] GameObject[] CharacterPrefabs; // prefab th?t
    [SerializeField] TextMeshProUGUI CharacterName;

    public static GameObject selectedCharacter;

    void Start()
    {
        index = 0;
        UpdateSelection();
    }

    public void OnLeftBtnClick()
    {
        if (index > 0) index--;
        UpdateSelection();
    }

    public void OnRightBtnClick()
    {
        if (index < Characters.Length - 1) index++;
        UpdateSelection();
    }

    public void OnPlayBtnClick()
    {
        SceneManager.LoadScene("SampleScene");
    }

    void UpdateSelection()
    {
        for (int i = 0; i < Characters.Length; i++)
        {
            bool isSelected = (i == index);

            Characters[i].GetComponent<SpriteRenderer>().color =
                isSelected ? Color.white : Color.black;

            Characters[i].GetComponent<Animator>().enabled = isSelected;

            if (isSelected)
            {
                selectedCharacter = CharacterPrefabs[i];
                CharacterName.text = CharacterPrefabs[i].name;
            }
        }
    }
}