using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using System.Collections;

/// <summary>
/// Panel yang muncul saat level selesai.
/// Menampilkan bintang dengan animasi dan tombol Next Level.
/// Mendukung half-star (0.5, 1, 1.5, 2, 2.5, 3).
/// </summary>
public class LevelCompletePanel : MonoBehaviour
{
    public static LevelCompletePanel Instance { get; private set; }

    [Header("Panel References")]
    [SerializeField] private GameObject panelContainer;
    [SerializeField] private TextMeshProUGUI levelCompleteText;
    [SerializeField] private TextMeshProUGUI timeText;
    
    [Header("Star References")]
    [Tooltip("3 Image untuk bintang penuh (Star1, Star2, Star3)")]
    [SerializeField] private Image[] starFullImages;
    
    [Tooltip("3 Image untuk bintang setengah (HalfStar1, HalfStar2, HalfStar3) - posisi sama dengan bintang penuh")]
    [SerializeField] private Image[] starHalfImages;
    
    [Header("Buttons")]
    [SerializeField] private Button nextLevelButton;
    [SerializeField] private Button retryButton;
    [SerializeField] private Button menuButton;
    
    [Header("Animation Settings")]
    [SerializeField] private float starDelay = 0.3f;
    [SerializeField] private float starAnimDuration = 0.3f;
    
    [Header("Scene Names")]
    [SerializeField] private string menuSceneName = "Level";
    
    private float currentStars = 0f;
    private int currentLevel = 1;
    private float completionTime = 0f;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        
        // Sembunyikan panel di awal
        if (panelContainer != null)
            panelContainer.SetActive(false);
    }

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (nextLevelButton != null)
            nextLevelButton.onClick.AddListener(OnNextLevelClicked);
        
        if (retryButton != null)
            retryButton.onClick.AddListener(OnRetryClicked);
        
        if (menuButton != null)
            menuButton.onClick.AddListener(OnMenuClicked);
    }

    /// <summary>
    /// Tampilkan panel level complete dengan jumlah bintang (mendukung half-star).
    /// </summary>
    /// <param name="level">Nomor level yang diselesaikan</param>
    /// <param name="stars">Jumlah bintang (0-3, bisa 0.5, 1, 1.5, 2, 2.5, 3)</param>
    public void ShowLevelComplete(int level, float stars)
    {
        currentLevel = level;
        currentStars = Mathf.Clamp(stars, 0f, 3f);
        
        // Stop timer dan ambil waktu
        if (LevelTimer.Instance != null)
        {
            completionTime = LevelTimer.Instance.StopTimer();
        }
        
        // Pause game
        Time.timeScale = 0f;
        
        // Update text
        if (levelCompleteText != null)
            levelCompleteText.text = $"Level {level} Complete!";
        
        // Update time text
        if (timeText != null)
            timeText.text = $"Time: {LevelTimer.FormatTime(completionTime)}";
        
        // Reset bintang
        ResetStars();
        
        // Tampilkan panel
        if (panelContainer != null)
            panelContainer.SetActive(true);
        
        // Animasi bintang
        StartCoroutine(AnimateStars(currentStars));
        
        // Simpan bintang
        SaveStars(level, stars);
        
        Debug.Log($"Level {level} Complete dengan {stars} bintang! Waktu: {completionTime:F2}s");
    }

    /// <summary>
    /// Tampilkan panel dengan perhitungan bintang otomatis dari LevelTimer.
    /// </summary>
    public void ShowLevelComplete(int level)
    {
        float stars = 3f; // Default
        
        // Hitung bintang dari waktu jika LevelTimer tersedia
        if (LevelTimer.Instance != null)
        {
            stars = LevelTimer.Instance.CalculateStars();
        }
        
        ShowLevelComplete(level, stars);
    }

    private void ResetStars()
    {
        // Reset bintang penuh
        if (starFullImages != null)
        {
            foreach (Image star in starFullImages)
            {
                if (star != null)
                {
                    star.transform.localScale = Vector3.zero;
                    star.gameObject.SetActive(false);
                }
            }
        }
        
        // Reset bintang setengah
        if (starHalfImages != null)
        {
            foreach (Image star in starHalfImages)
            {
                if (star != null)
                {
                    star.transform.localScale = Vector3.zero;
                    star.gameObject.SetActive(false);
                }
            }
        }
    }

    private IEnumerator AnimateStars(float starCount)
    {
        // Hitung berapa bintang penuh dan apakah ada setengah
        int fullStars = Mathf.FloorToInt(starCount);
        bool hasHalfStar = (starCount - fullStars) >= 0.5f;
        
        // Animasi bintang penuh
        if (starFullImages != null)
        {
            for (int i = 0; i < fullStars && i < starFullImages.Length; i++)
            {
                if (starFullImages[i] == null) continue;
                
                yield return new WaitForSecondsRealtime(starDelay);
                
                starFullImages[i].gameObject.SetActive(true);
                yield return StartCoroutine(AnimateStarScale(starFullImages[i].transform));
            }
        }
        
        // Animasi bintang setengah (jika ada)
        if (hasHalfStar && starHalfImages != null && fullStars < starHalfImages.Length)
        {
            if (starHalfImages[fullStars] != null)
            {
                yield return new WaitForSecondsRealtime(starDelay);
                
                starHalfImages[fullStars].gameObject.SetActive(true);
                yield return StartCoroutine(AnimateStarScale(starHalfImages[fullStars].transform));
            }
        }
    }

    private IEnumerator AnimateStarScale(Transform starTransform)
    {
        float elapsed = 0f;
        
        while (elapsed < starAnimDuration)
        {
            elapsed += Time.unscaledDeltaTime;
            float t = elapsed / starAnimDuration;
            
            // Easing: elastic out effect
            float scale = EaseOutBack(t);
            starTransform.localScale = Vector3.one * scale;
            
            yield return null;
        }
        
        starTransform.localScale = Vector3.one;
    }

    private float EaseOutBack(float t)
    {
        float c1 = 1.70158f;
        float c3 = c1 + 1;
        return 1 + c3 * Mathf.Pow(t - 1, 3) + c1 * Mathf.Pow(t - 1, 2);
    }

    /// <summary>
    /// Simpan bintang untuk level tertentu (mendukung half-star).
    /// Disimpan sebagai float * 10 untuk presisi.
    /// </summary>
    private void SaveStars(int level, float stars)
    {
        string key = $"Level_{level}_Stars";
        float previousStars = GetStars(level);
        
        Debug.Log($"[SaveStars] Attempting to save stars for Level {level}: new={stars}, previous={previousStars}");
        
        if (stars > previousStars)
        {
            // Simpan sebagai int (x10 untuk handle 0.5)
            int saveValue = Mathf.RoundToInt(stars * 10);
            PlayerPrefs.SetInt(key, saveValue);
            PlayerPrefs.Save();
            Debug.Log($"[SaveStars] SUCCESS! Saved {stars} stars (value={saveValue}) for Level {level}");
        }
        else
        {
            Debug.Log($"[SaveStars] Skipped - new stars ({stars}) not higher than previous ({previousStars})");
        }
    }

    /// <summary>
    /// Dapatkan jumlah bintang yang tersimpan untuk level tertentu (float).
    /// </summary>
    public static float GetStars(int level)
    {
        // Disimpan sebagai int x10, konversi kembali ke float
        string key = $"Level_{level}_Stars";
        int rawValue = PlayerPrefs.GetInt(key, 0);
        float stars = rawValue / 10f;
        Debug.Log($"[GetStars] Level {level}: key={key}, rawValue={rawValue}, stars={stars}");
        return stars;
    }

    /// <summary>
    /// Dapatkan total bintang dari semua level.
    /// </summary>
    public static float GetTotalStars(int maxLevel = 30)
    {
        float total = 0f;
        for (int i = 1; i <= maxLevel; i++)
        {
            total += GetStars(i);
        }
        return total;
    }

    // === BUTTON CALLBACKS ===
    
    private void OnNextLevelClicked()
    {
        Time.timeScale = 1f;
        
        if (LevelManager.Instance != null)
        {
            // Complete level dulu (unlock next level)
            LevelManager.Instance.CompleteLevel();
            
            int nextLevel = LevelManager.Instance.GetCurrentLevel();
            
            // Cek apakah sudah di level terakhir season
            if (nextLevel > LevelManager.Instance.levelsPerSeason)
            {
                SceneManager.LoadScene(menuSceneName);
            }
            else
            {
                // Pindah ke level berikutnya dalam scene yang sama
                LevelManager.Instance.MovePlayerToCurrentCheckpoint();
                
                // Reset dan start timer untuk level baru
                if (LevelTimer.Instance != null)
                    LevelTimer.Instance.StartTimer();
                
                panelContainer.SetActive(false);
            }
        }
    }

    private void OnRetryClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    private void OnMenuClicked()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(menuSceneName);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1f;
    }
}
