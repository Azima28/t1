using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Pause : MonoBehaviour
{
    [Header("UI")]
    public GameObject pausePanel;
    public Button[] buttonsToDisable;

    [Header("Gameplay To Disable (PLAYER / INPUT)")]
    public MonoBehaviour[] scriptsToDisable; // PlayerMovement, Controller, dll

    public static bool IsPaused { get; private set; } = false;

    // ================= PAUSE =================
    public void PauseGame()
    {
        if (IsPaused) return;

        Time.timeScale = 0f;
        IsPaused = true;
        
        if (pausePanel != null)
            pausePanel.SetActive(true);
        else
            Debug.LogError("PausePanel is not assigned! Cannot show pause menu.");

        foreach (Button btn in buttonsToDisable)
            btn.interactable = false;

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = false;
        }
    }

    // ================= RESUME =================
    public void Resume()
    {
        Time.timeScale = 1f;
        IsPaused = false;

        if (pausePanel != null)
            pausePanel.SetActive(false);

        foreach (Button btn in buttonsToDisable)
            btn.interactable = true;

        foreach (MonoBehaviour script in scriptsToDisable)
        {
            if (script != null)
                script.enabled = true;
        }
    }

    // ================= REPLAY =================
    public void Replay()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    // ================= EXIT → MAIN MENU =================
    public void Exit()
    {
        Time.timeScale = 1f;
        IsPaused = false;
        SceneManager.LoadScene("Main Menu");
    }

    public void PlayGame()
    {
        SceneManager.LoadScene("Level");
    }

    public void QuitSeason()
    {
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game Keluar");
    }
}
