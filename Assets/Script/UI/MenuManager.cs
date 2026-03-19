using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    public void PlayVsAI()
    {
        SceneManager.LoadScene("SampleScene");
    }

    public void PlayVsPlayer()
    {
        SceneManager.LoadScene("PvsP_Scene");
    }
    //    public static string selectedMode;

    //    public void PlayVsAI()
    //    {
    //        selectedMode = "AI"; // Ghi nhớ chọn đánh máy
    //        SceneManager.LoadScene("SelectCharacter"); // Chuyển đến màn chọn nhân vật
    //    }

    //    public void PlayVsPlayer()
    //    {
    //        selectedMode = "Player"; // Ghi nhớ chọn đánh người
    //        SceneManager.LoadScene("SelectCharacter"); // Chuyển đến màn chọn nhân vật
    //    }
    //}
}