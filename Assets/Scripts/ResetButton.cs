using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Tombol untuk reset progress level.
/// Pasang di UI Button, panggil ResetGame() dari OnClick.
/// </summary>
public class ResetButton : MonoBehaviour
{
    /// <summary>
    /// Reset semua progress dan reload scene
    /// </summary>
    public void ResetGame()
    {
        if (LevelManager.Instance != null)
        {
            LevelManager.Instance.ResetAllProgress();
        }
        
        // Reload scene saat ini
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
