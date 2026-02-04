using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PilihLevel : MonoBehaviour
{
    public void LoadLevel1()
    {
        PlayerPrefs.SetInt("SelectedLevel", 1);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadLevel2()
    {
        PlayerPrefs.SetInt("SelectedLevel", 2);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadLevel3()
    {
        PlayerPrefs.SetInt("SelectedLevel", 3);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadLevel4()
    {
        PlayerPrefs.SetInt("SelectedLevel", 4);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }

    public void LoadLevel5()
    {
        PlayerPrefs.SetInt("SelectedLevel", 5);
        PlayerPrefs.Save();
        SceneManager.LoadScene("SampleScene");
    }
}

