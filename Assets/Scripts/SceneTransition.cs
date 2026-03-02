using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Script untuk transisi layar hitam (fade in / fade out) saat pindah scene.
/// 
/// --- CARA SETUP DI UNITY ---
/// 1. Buat Canvas baru di Main Menu, beri nama "TransitionCanvas".
/// 2. Set "Sort Order" di Canvas jadi angka besar (misal: 100) supaya selalu di paling depan.
/// 3. Di dalam TransitionCanvas, buat UI Panel, beri nama "BlackBox".
/// 4. Ubah warna BlackBox jadi Hitam pekat (Alpha = 255).
/// 5. Buat Empty GameObject bernama "SceneManager", pasang script ini ke situ.
/// 6. Di Inspector "SceneTransition" ini:
///    - Assign BlackBox Panel ke kolom "fadeImage".
/// </summary>
public class SceneTransition : MonoBehaviour
{
    // Singleton pattern agar gampang dipanggil dari mana saja
    public static SceneTransition instance;

    [Header("UI Reference")]
    [Tooltip("Panel hitam yang menutupi layar (Image/Panel)")]
    public Image fadeImage;

    [Header("Pengaturan Fade")]
    [Tooltip("Waktu transisi dalam detik")]
    public float fadeDuration = 1f;

    void Awake()
    {
        // Pastikan hanya ada 1 transisi manager
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        // Saat scene baru mulai, layarnya otomatis berubah dari hitam pekat jadi transparan
        if (fadeImage != null)
        {
            // Mulai kondisi "Hitam" (Alpha 1)
            SetAlpha(1f);
            
            // Animasi transparan ke "Terang"
            StartCoroutine(FadeRoutine(1f, 0f, null));
        }
    }

    /// <summary>
    /// Panggil fungsi ini lewat script lain saat ingin ganti scene dengan layar hitam
    /// </summary>
    public void LoadScene(string sceneName)
    {
        if (fadeImage != null)
        {
            // Pastikan panel aktif jika sempat mati
            fadeImage.gameObject.SetActive(true);
            
            // Animasi dari "Terang" jadi "Hitam pekat"
            StartCoroutine(FadeRoutine(0f, 1f, () => 
            {
                // Setelah hitam total, pindah scene!
                SceneManager.LoadScene(sceneName);
            }));
        }
        else
        {
            // Kalau lupa assign kotak hitam, langsung pindah
            Debug.LogWarning("[SceneTransition] Anda lupa memasukkan panel hitam! Langsung lompat scene.");
            SceneManager.LoadScene(sceneName);
        }
    }

    // Proses animasinya mengubah warna Alpha
    private IEnumerator FadeRoutine(float startAlpha, float targetAlpha, System.Action onComplete)
    {
        float timer = 0f;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, timer / fadeDuration);
            SetAlpha(newAlpha);
            yield return null;
        }

        SetAlpha(targetAlpha);

        // Setelah layar bersih (game mulai), matikan panel panel biar tidak pencet-pencet tidak sengaja
        if (targetAlpha == 0f)
        {
            fadeImage.gameObject.SetActive(false);
        }

        // Jalankan perintah lanjutan jika ada (misal: load scene)
        if (onComplete != null)
        {
            onComplete.Invoke();
        }
    }

    // Fungsi pembantu ganti warna transparan (Alpha)
    private void SetAlpha(float alpha)
    {
        Color color = fadeImage.color;
        color.a = alpha;
        fadeImage.color = color;
    }
}
