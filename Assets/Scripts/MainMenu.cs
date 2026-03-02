using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    public void PlayGame()
    {
        // Panggil transisi layar hitam sebelum pindah scene
        if (SceneTransition.instance != null)
        {
            SceneTransition.instance.LoadScene("Level");
        }
        else
        {
            // Fallback kalau lupa pasang script transisi di scene
            SceneManager.LoadScene("Level");
        }
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}
