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
        
        // Debug: tampilkan info checkpoint
        Debug.Log($"=== LEVEL MANAGER DEBUG ===");
        Debug.Log($"Current Level: {currentLevel}");
        Debug.Log($"Checkpoint Index: {currentLevel - 1}");
        Debug.Log($"Total Checkpoints: {levelCheckpoints.Length}");
        
        if (currentLevel - 1 >= 0 && currentLevel - 1 < levelCheckpoints.Length)
        {
            Transform cp = levelCheckpoints[currentLevel - 1];
            Debug.Log($"Checkpoint Object: {(cp != null ? cp.name : "NULL")}");
            Debug.Log($"Checkpoint Position: {(cp != null ? cp.position.ToString() : "N/A")}");
        }
        else
        {
            Debug.LogError($"Checkpoint index {currentLevel - 1} out of range!");
        }
        
        // Pindahkan player ke checkpoint level saat ini
        MovePlayerToCurrentCheckpoint();
        
        // Set camera zone untuk level saat ini
        SetCameraZoneForLevel(currentLevel);
    }

    public int GetCurrentLevel() => currentLevel;
    public int GetCurrentSeason() => PlayerPrefs.GetInt(SEASON_KEY, 1);

    /// <summary>
    /// Check apakah level sudah terbuka.
    /// Level 1 selalu terbuka, level lainnya terbuka jika <= savedLevel.
    /// </summary>
    public static bool IsLevelUnlocked(int level)
    {
        // Level 1 selalu terbuka
        if (level <= 1) return true;
        
        // Cek level tersimpan
        int savedLevel = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        return level <= savedLevel;
    }

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
        
        // Cek apakah ada SelectedLevel dari menu level selection
        // SelectedLevel = level yang dipilih player dari tombol
        // CurrentLevel = progress unlock tertinggi
        int selectedLevel = PlayerPrefs.GetInt("SelectedLevel", 0);
        
        if (selectedLevel > 0)
        {
            // Player memilih level dari menu, spawn di level tersebut
            currentLevel = selectedLevel;
            Debug.Log($"Player selected Level {selectedLevel} from menu");
            
            // Hapus SelectedLevel agar tidak mempengaruhi session berikutnya
            PlayerPrefs.DeleteKey("SelectedLevel");
            PlayerPrefs.Save();
        }
        else if (savedSeason != currentSeason)
        {
            currentLevel = 1;
        }
        else
        {
            // Muat level terakhir yang tersimpan (misal saat Retry)
            currentLevel = PlayerPrefs.GetInt(LEVEL_KEY, 1);
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

    /// <summary>
    /// Reset semua progress. Hanya Level 1 yang terbuka.
    /// </summary>
    [ContextMenu("Reset All Progress")]
    public void ResetAllProgress()
    {
        PlayerPrefs.SetInt(SEASON_KEY, 1);
        PlayerPrefs.SetInt(LEVEL_KEY, 1);
        PlayerPrefs.Save();
        currentLevel = 1;
        Debug.Log("All progress reset! Only Level 1 is unlocked.");
    }

    /// <summary>
    /// Unlock semua level (untuk testing).
    /// Panggil dari Console atau buat button untuk ini.
    /// </summary>
    [ContextMenu("Unlock All Levels")]
    public void UnlockAllLevels()
    {
        PlayerPrefs.SetInt(LEVEL_KEY, levelsPerSeason);
        PlayerPrefs.Save();
        Debug.Log($"All {levelsPerSeason} levels unlocked!");
    }

    /// <summary>
    /// Unlock level tertentu.
    /// </summary>
    public void UnlockLevel(int level)
    {
        int currentUnlocked = PlayerPrefs.GetInt(LEVEL_KEY, 1);
        if (level > currentUnlocked)
        {
            PlayerPrefs.SetInt(LEVEL_KEY, level);
            PlayerPrefs.Save();
            Debug.Log($"Level {level} unlocked!");
        }
    }
}
