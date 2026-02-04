using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Portal untuk naik level.
/// - Selalu bisa teleport
/// - Hanya naik level jika belum selesai
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class LevelPortal : MonoBehaviour
{
    [Header("Level Requirement")]
    [Tooltip("Portal ini untuk menyelesaikan level berapa? (1-10)")]
    public int forLevel = 1;
    
    [Header("Fade Settings")]
    public float fadeDuration = 0.5f;
    
    [Header("Destination")]
    public Transform destinationPoint;
    public CameraZone destinationZone;
    
    private Image fadePanel;
    private bool isTeleporting = false;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
        
        SetupFadePanel();
    }

    void SetupFadePanel()
    {
        GameObject existingPanel = GameObject.Find("FadePanel");
        if (existingPanel != null)
        {
            fadePanel = existingPanel.GetComponent<Image>();
            if (fadePanel != null) return;
        }

        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            GameObject canvasObj = new GameObject("FadeCanvas");
            canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
            canvas.sortingOrder = 999;
            canvasObj.AddComponent<CanvasScaler>();
        }

        GameObject panelObj = new GameObject("FadePanel");
        panelObj.transform.SetParent(canvas.transform, false);
        panelObj.transform.SetAsLastSibling();
        
        fadePanel = panelObj.AddComponent<Image>();
        fadePanel.color = new Color(0, 0, 0, 0);
        fadePanel.raycastTarget = false;

        RectTransform rect = fadePanel.rectTransform;
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (isTeleporting) return;
        if (!other.CompareTag("Player")) return;
        
        isTeleporting = true;
        StartCoroutine(TeleportWithFade(other.transform));
    }

    IEnumerator TeleportWithFade(Transform player)
    {
        // Fade ke hitam
        if (fadePanel != null)
        {
            yield return StartCoroutine(Fade(0, 1));
        }

        // Cek apakah perlu naik level
        if (LevelManager.Instance != null)
        {
            int currentLevel = LevelManager.Instance.GetCurrentLevel();
            
            // Hanya naik level jika player di level yang sesuai dengan portal ini
            if (currentLevel == forLevel)
            {
                // Tampilkan panel level complete dengan bintang
                if (LevelCompletePanel.Instance != null)
                {
                    LevelCompletePanel.Instance.ShowLevelComplete(forLevel);
                    isTeleporting = false;
                    
                    // Fade kembali ke terang
                    if (fadePanel != null)
                    {
                        yield return StartCoroutine(Fade(1, 0));
                    }
                    yield break; // Stop disini, panel yang handle selanjutnya
                }
                else
                {
                    // Fallback jika tidak ada panel
                    LevelManager.Instance.CompleteLevel();
                    Debug.Log($"Level {forLevel} Complete! Now at Level {LevelManager.Instance.GetCurrentLevel()}");
                }
            }
            else
            {
                Debug.Log($"Portal untuk Level {forLevel}, tapi player sudah di Level {currentLevel}. Teleport saja tanpa naik level.");
            }
        }

        // Pindahkan player (selalu teleport)
        if (destinationPoint != null)
        {
            player.position = destinationPoint.position;
        }

        // Ganti camera zone
        if (destinationZone != null && CameraFollow.Instance != null)
        {
            CameraFollow.Instance.SetZone(destinationZone);
        }

        // Fade ke terang
        if (fadePanel != null)
        {
            yield return StartCoroutine(Fade(1, 0));
        }
        
        isTeleporting = false;
    }

    IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color color = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            color.a = Mathf.Lerp(startAlpha, endAlpha, elapsed / fadeDuration);
            fadePanel.color = color;
            yield return null;
        }

        color.a = endAlpha;
        fadePanel.color = color;
    }
}
