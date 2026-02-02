using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Mengelola level dalam 1 scene (zone-based).
/// Juga mengelola checkpoint dan camera zone per level.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }
    
    private const string SEASON_KEY = "CurrentSeason";
    private const string LEVEL_KEY = "CurrentLevel";
    
    [Header("Season Settings")]
    [Tooltip("Season berapa scene ini (1, 2, atau 3)")]
    public int currentSeason = 1;
    
    [Tooltip("Jumlah level per season")]
    public int levelsPerSeason = 10;
    
    [Header("Checkpoints (1 per level)")]
    [Tooltip("Checkpoint untuk setiap level. Index 0 = Level 1, dst.")]
    public Transform[] levelCheckpoints;
    
    [Header("Camera Zones (1 per level)")]
    [Tooltip("Camera zone untuk setiap level. Index 0 = Level 1, dst.")]
    public CameraZone[] levelCameraZones;
    
    [Header("Scene Names")]
    public string nextSeasonScene = "";
    public string gameCompleteScene = "GameComplete";
    
    [Header("Current Progress")]
    [SerializeField] private int currentLevel = 1;
    
    [Header("Testing")]
    [Tooltip("0 = pakai level tersimpan, 1-10 = langsung ke level tersebut")]
    public int testLevel = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    void Start()
    {
        LoadProgress();
        
        // Jika testLevel diset, override ke level tersebut
        if (testLevel > 0 && testLevel <= levelsPerSeason)
        {
            currentLevel = testLevel;
            Debug.Log($"[TEST MODE] Override ke Level {testLevel}");
        }
        
        // Pindahkan player ke checkpoint level saat ini
        MovePlayerToCurrentCheckpoint();
        
        // Set camera zone untuk level saat ini
        SetCameraZoneForLevel(currentLevel);
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentSeason() => PlayerPrefs.GetInt(SEASON_KEY, 1);

    /// <summary>
    /// Dapatkan checkpoint untuk level tertentu
    /// </summary>
    public Transform GetCheckpoint(int level)
    {
        int index = level - 1;
        if (index >= 0 && index < levelCheckpoints.Length)
        {
            return levelCheckpoints[index];
        }
        return null;
    }

    /// <summary>
    /// Dapatkan camera zone untuk level tertentu
    /// </summary>
    public CameraZone GetCameraZone(int level)
    {
        int index = level - 1;
        if (index >= 0 && index < levelCameraZones.Length)
        {
            return levelCameraZones[index];
        }
        return null;
    }

    /// <summary>
    /// Dapatkan checkpoint untuk level saat ini
    /// </summary>
    public Transform GetCurrentCheckpoint()
    {
        return GetCheckpoint(currentLevel);
    }

    /// <summary>
    /// Set camera zone untuk level tertentu
    /// </summary>
    public void SetCameraZoneForLevel(int level)
    {
        CameraZone zone = GetCameraZone(level);
        if (zone != null && CameraFollow.Instance != null)
        {
            CameraFollow.Instance.SetZone(zone);
            Debug.Log($"Camera zone set to Level {level}");
        }
    }

    /// <summary>
    /// Pindahkan player ke checkpoint level saat ini dan set camera zone
    /// </summary>
    public void MovePlayerToCurrentCheckpoint()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Transform checkpoint = GetCurrentCheckpoint();
        
        if (player != null && checkpoint != null)
        {
            player.transform.position = checkpoint.position;
            Debug.Log($"Player moved to Level {currentLevel} checkpoint");
        }
        
        // Set camera zone juga
        SetCameraZoneForLevel(currentLevel);
    }

    public void LoadProgress()
    {
        int savedSeason = PlayerPrefs.GetInt(SEASON_KEY, 1);
        int savedLevel = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        
        if (savedSeason != currentSeason)
        {
            currentLevel = 1;
        }
        else
        {
            currentLevel = savedLevel;
        }
        
        Debug.Log($"Loaded: Season {currentSeason}, Level {currentLevel}");
    }

    public void SaveProgress()
    {
        PlayerPrefs.SetInt(SEASON_KEY, currentSeason);
        PlayerPrefs.SetInt(LEVEL_KEY, currentLevel);
        PlayerPrefs.Save();
        Debug.Log($"Saved: Season {currentSeason}, Level {currentLevel}");
    }

    public void CompleteLevel()
    {
        currentLevel++;
        
        if (currentLevel > levelsPerSeason)
        {
            CompleteSeason();
        }
        else
        {
            SaveProgress();
            Debug.Log($"Level Complete! Now at Season {currentSeason}, Level {currentLevel}");
        }
    }

    private void CompleteSeason()
    {
        Debug.Log($"Season {currentSeason} Complete!");
        
        int nextSeason = currentSeason + 1;
        PlayerPrefs.SetInt(SEASON_KEY, nextSeason);
        PlayerPrefs.SetInt(LEVEL_KEY, 1);
        PlayerPrefs.Save();
        
        if (!string.IsNullOrEmpty(nextSeasonScene))
        {
            SceneManager.LoadScene(nextSeasonScene);
        }
        else if (!string.IsNullOrEmpty(gameCompleteScene))
        {
            SceneManager.LoadScene(gameCompleteScene);
        }
    }

    public void ResetAllProgress()
    {
        PlayerPrefs.SetInt(SEASON_KEY, 1);
        PlayerPrefs.SetInt(LEVEL_KEY, 1);
        PlayerPrefs.Save();
        currentLevel = 1;
        Debug.Log("All progress reset!");
    }
}
