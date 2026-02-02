using UnityEngine;
using UnityEngine.UI;
using System.Collections;

/// <summary>
/// Menangani kematian dan respawn player.
/// Dengan efek fade saat mati.
/// </summary>
public class PlayerDeath : MonoBehaviour
{
    [Header("Settings")]
    public float respawnDelay = 0.5f;
    public float fadeDuration = 0.3f;
    
    private Rigidbody2D rb;
    private Vector3 fallbackSpawn;
    private Image fadePanel;
    private bool isDying = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        fallbackSpawn = transform.position;
        SetupFadePanel();
    }

    void SetupFadePanel()
    {
        // Cari FadePanel yang sudah ada
        GameObject existingPanel = GameObject.Find("FadePanel");
        if (existingPanel != null)
        {
            fadePanel = existingPanel.GetComponent<Image>();
            return;
        }

        // Buat baru jika tidak ada
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null) return;

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

    public void Die()
    {
        if (isDying) return; // Cegah multi-death
        isDying = true;
        
        Debug.Log("Player died!");
        
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.simulated = false;
        }
        
        StartCoroutine(DieWithFade());
    }

    IEnumerator DieWithFade()
    {
        // Fade ke hitam
        if (fadePanel != null)
        {
            yield return StartCoroutine(Fade(0, 1));
        }
        
        yield return new WaitForSeconds(respawnDelay);
        
        // Respawn
        Respawn();
        
        // Fade ke terang
        if (fadePanel != null)
        {
            yield return StartCoroutine(Fade(1, 0));
        }
        
        isDying = false;
    }

    void Respawn()
    {
        Vector3 respawnPos = fallbackSpawn;
        
        if (LevelManager.Instance != null)
        {
            Transform checkpoint = LevelManager.Instance.GetCurrentCheckpoint();
            if (checkpoint != null)
            {
                respawnPos = checkpoint.position;
            }
        }
        
        transform.position = respawnPos;
        Debug.Log("Player respawned di: " + respawnPos);
        
        if (rb != null)
        {
            rb.simulated = true;
        }
        
        ResetAllTraps();
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

    void ResetAllTraps()
    {
        FallingRock[] rocks = FindObjectsOfType<FallingRock>();
        foreach (FallingRock rock in rocks)
        {
            rock.ResetRock();
        }
        
        RisingObstacle[] risingObstacles = FindObjectsOfType<RisingObstacle>();
        foreach (RisingObstacle obstacle in risingObstacles)
        {
            obstacle.ResetObstacle();
        }
        
        MovingObstacle[] movingObstacles = FindObjectsOfType<MovingObstacle>();
        foreach (MovingObstacle obstacle in movingObstacles)
        {
            obstacle.ResetObstacle();
        }
        
        TrapTrigger[] triggers = FindObjectsOfType<TrapTrigger>();
        foreach (TrapTrigger trigger in triggers)
        {
            trigger.ResetTrigger();
        }
    }
}
