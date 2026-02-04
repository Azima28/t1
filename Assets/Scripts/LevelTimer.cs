using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Timer/Stopwatch untuk level.
/// Menghitung waktu dari awal level dan menentukan bintang berdasarkan waktu.
/// Mendukung half-star (0.5, 1, 1.5, 2, 2.5, 3).
/// </summary>
public class LevelTimer : MonoBehaviour
{
    public static LevelTimer Instance { get; private set; }

    [Header("UI References")]
    [Tooltip("Text untuk menampilkan waktu (optional)")]
    [SerializeField] private TextMeshProUGUI timerText;
    
    [Header("Time Thresholds (seconds)")]
    [Tooltip("Waktu untuk dapat 3 bintang (tercepat)")]
    [SerializeField] private float time3Stars = 30f;
    
    [Tooltip("Waktu untuk dapat 2.5 bintang")]
    [SerializeField] private float time2_5Stars = 45f;
    
    [Tooltip("Waktu untuk dapat 2 bintang")]
    [SerializeField] private float time2Stars = 60f;
    
    [Tooltip("Waktu untuk dapat 1.5 bintang")]
    [SerializeField] private float time1_5Stars = 90f;
    
    [Tooltip("Waktu untuk dapat 1 bintang")]
    [SerializeField] private float time1Stars = 120f;
    
    [Tooltip("Waktu untuk dapat 0.5 bintang (paling lambat, lebih dari ini = 0)")]
    [SerializeField] private float time0_5Stars = 180f;

    [Header("Settings")]
    [SerializeField] private bool showMilliseconds = false;
    
    private float elapsedTime = 0f;
    private bool isRunning = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        // Mulai timer otomatis saat scene dimulai
        StartTimer();
    }

    private void Update()
    {
        if (isRunning)
        {
            elapsedTime += Time.deltaTime;
            UpdateTimerDisplay();
        }
    }

    /// <summary>
    /// Mulai atau reset timer.
    /// </summary>
    public void StartTimer()
    {
        elapsedTime = 0f;
        isRunning = true;
        Debug.Log("Timer started!");
    }

    /// <summary>
    /// Stop timer dan return waktu total.
    /// </summary>
    public float StopTimer()
    {
        isRunning = false;
        Debug.Log($"Timer stopped at {elapsedTime:F2} seconds");
        return elapsedTime;
    }

    /// <summary>
    /// Pause timer tanpa reset.
    /// </summary>
    public void PauseTimer()
    {
        isRunning = false;
    }

    /// <summary>
    /// Resume timer setelah pause.
    /// </summary>
    public void ResumeTimer()
    {
        isRunning = true;
    }

    /// <summary>
    /// Reset timer ke 0.
    /// </summary>
    public void ResetTimer()
    {
        elapsedTime = 0f;
        UpdateTimerDisplay();
    }

    /// <summary>
    /// Dapatkan waktu elapsed saat ini.
    /// </summary>
    public float GetElapsedTime()
    {
        return elapsedTime;
    }

    /// <summary>
    /// Hitung bintang berdasarkan waktu (mendukung half-star).
    /// Return value: 0, 0.5, 1, 1.5, 2, 2.5, atau 3
    /// </summary>
    public float CalculateStars()
    {
        return CalculateStarsFromTime(elapsedTime);
    }

    /// <summary>
    /// Hitung bintang dari waktu tertentu.
    /// </summary>
    public float CalculateStarsFromTime(float time)
    {
        if (time <= time3Stars) return 3f;
        if (time <= time2_5Stars) return 2.5f;
        if (time <= time2Stars) return 2f;
        if (time <= time1_5Stars) return 1.5f;
        if (time <= time1Stars) return 1f;
        if (time <= time0_5Stars) return 0.5f;
        return 0f;
    }

    /// <summary>
    /// Update tampilan timer di UI.
    /// </summary>
    private void UpdateTimerDisplay()
    {
        if (timerText == null) return;

        int minutes = Mathf.FloorToInt(elapsedTime / 60f);
        int seconds = Mathf.FloorToInt(elapsedTime % 60f);
        
        if (showMilliseconds)
        {
            int milliseconds = Mathf.FloorToInt((elapsedTime * 100f) % 100f);
            timerText.text = $"{minutes:00}:{seconds:00}:{milliseconds:00}";
        }
        else
        {
            timerText.text = $"{minutes:00}:{seconds:00}";
        }
    }

    /// <summary>
    /// Format waktu ke string untuk display.
    /// </summary>
    public static string FormatTime(float time)
    {
        int minutes = Mathf.FloorToInt(time / 60f);
        int seconds = Mathf.FloorToInt(time % 60f);
        return $"{minutes:00}:{seconds:00}";
    }
}
