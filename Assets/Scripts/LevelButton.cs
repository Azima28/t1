using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

/// <summary>
/// Script untuk setiap tombol level.
/// Menampilkan status lock/unlock dan load scene saat diklik.
/// </summary>
public class LevelButton : MonoBehaviour
{
    [Header("Level Settings")]
    [SerializeField] private int levelNumber = 1;
    [SerializeField] private string sceneToLoad = "SampleScene";

    [Header("UI References")]
    [SerializeField] private Button button;
    [SerializeField] private GameObject lockIcon;
    [SerializeField] private TextMeshProUGUI levelText;

    [Header("Star Display")]
    [Tooltip("3 Image untuk bintang penuh di level button")]
    [SerializeField] private Image[] starFullImages;
    
    [Tooltip("3 Image untuk bintang setengah (posisi sama dengan bintang penuh)")]
    [SerializeField] private Image[] starHalfImages;

    [Header("Visual Settings")]
    [SerializeField] private Color unlockedColor = Color.white;
    [SerializeField] private Color lockedColor = new Color(0.5f, 0.5f, 0.5f, 1f);

    private Image buttonImage;

    private void Awake()
    {
        // Auto-find button if not assigned
        if (button == null)
            button = GetComponent<Button>();

        if (button != null)
            buttonImage = button.GetComponent<Image>();
    }

    private void Start()
    {
        UpdateLockState();
    }

    private void OnEnable()
    {
        // Refresh state setiap kali tombol aktif (misal setelah kembali dari gameplay)
        UpdateLockState();
    }

    /// <summary>
    /// Update tampilan tombol berdasarkan status lock/unlock.
    /// </summary>
    public void UpdateLockState()
    {
        bool isUnlocked = LevelManager.IsLevelUnlocked(levelNumber);

        // Update button interactability
        if (button != null)
            button.interactable = isUnlocked;

        // Show/hide lock icon
        if (lockIcon != null)
            lockIcon.SetActive(!isUnlocked);

        // Update button color
        if (buttonImage != null)
            buttonImage.color = isUnlocked ? unlockedColor : lockedColor;

        // Update level text (optional)
        if (levelText != null)
            levelText.text = levelNumber.ToString();
        
        // Update star display
        UpdateStarDisplay(isUnlocked);
    }

    /// <summary>
    /// Update tampilan bintang berdasarkan skor tersimpan.
    /// Mendukung half-star display (0.5, 1, 1.5, 2, 2.5, 3).
    /// </summary>
    private void UpdateStarDisplay(bool isUnlocked)
    {
        // Dapatkan jumlah bintang yang tersimpan untuk level ini
        float stars = isUnlocked ? LevelCompletePanel.GetStars(levelNumber) : 0f;
        
        // Hitung berapa bintang penuh dan apakah ada setengah
        int fullStars = Mathf.FloorToInt(stars);
        bool hasHalfStar = (stars - fullStars) >= 0.5f;
        
        Debug.Log($"[LevelButton] Level {levelNumber}: isUnlocked={isUnlocked}, stars={stars}, fullStars={fullStars}, hasHalf={hasHalfStar}");
        
        // Update bintang penuh
        if (starFullImages != null)
        {
            Debug.Log($"[LevelButton] Level {levelNumber}: starFullImages.Length={starFullImages.Length}");
            for (int i = 0; i < starFullImages.Length; i++)
            {
                if (starFullImages[i] == null)
                {
                    Debug.LogWarning($"[LevelButton] Level {levelNumber}: starFullImages[{i}] is NULL!");
                    continue;
                }
                bool shouldShow = i < fullStars;
                starFullImages[i].gameObject.SetActive(shouldShow);
                Debug.Log($"[LevelButton] Level {levelNumber}: Star {i} SetActive({shouldShow}) - actual active={starFullImages[i].gameObject.activeSelf}");
            }
        }
        else
        {
            Debug.LogWarning($"[LevelButton] Level {levelNumber}: starFullImages array is NULL!");
        }
        
        // Update bintang setengah
        if (starHalfImages != null)
        {
            for (int i = 0; i < starHalfImages.Length; i++)
            {
                if (starHalfImages[i] == null) continue;
                // Tampilkan half star hanya di posisi setelah full stars
                starHalfImages[i].gameObject.SetActive(hasHalfStar && i == fullStars);
            }
        }
    }

    /// <summary>
    /// Dipanggil saat tombol diklik.
    /// Assign ke OnClick() di Inspector.
    /// </summary>
    public void OnLevelButtonClick()
    {
        if (!LevelManager.IsLevelUnlocked(levelNumber))
        {
            Debug.Log($"Level {levelNumber} masih terkunci!");
            return;
        }

        // Simpan level yang dipilih untuk spawn point di SampleScene
        // LevelManager akan membaca ini dan teleport player ke checkpoint yang sesuai
        PlayerPrefs.SetInt("SelectedLevel", levelNumber);
        PlayerPrefs.Save();

        Debug.Log($"Loading level {levelNumber}: {sceneToLoad}");
        SceneManager.LoadScene(sceneToLoad);
    }

    // === EDITOR HELPERS ===
    private void Reset()
    {
        // Auto-find components saat script ditambahkan
        button = GetComponent<Button>();
        levelText = GetComponentInChildren<TextMeshProUGUI>();
        
        // Cari lock icon (biasanya child dengan nama "Lock" atau "LockIcon")
        Transform lockTransform = transform.Find("Lock");
        if (lockTransform == null) lockTransform = transform.Find("LockIcon");
        if (lockTransform != null) lockIcon = lockTransform.gameObject;
    }

    private void OnValidate()
    {
        // Update text di editor saat levelNumber berubah
        if (levelText != null)
            levelText.text = levelNumber.ToString();
    }
}
