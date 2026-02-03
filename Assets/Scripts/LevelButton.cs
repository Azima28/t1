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

        // Simpan level yang sedang dimainkan (opsional, untuk track progress)
        PlayerPrefs.SetInt("CurrentLevel", levelNumber);
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
