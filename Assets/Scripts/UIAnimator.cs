using UnityEngine;

/// <summary>
/// Script untuk membuat animasi UI (tombol/panel) masuk dari luar layar (slide in).
/// </summary>
public class UIAnimator : MonoBehaviour
{
    [Header("=== Target UI ===")]
    [Tooltip("UI yang mau dianimasikan (biasanya otomatis terisi sendiri)")]
    public RectTransform uiElement;

    [Header("=== Pengaturan Animasi ===")]
    [Tooltip("Titik awal/offset (X = 1000 artinya animasi mulai dari kanan layar)")]
    public Vector2 startOffset = new Vector2(1000f, 0f);
    
    [Tooltip("Berapa detik animasi berjalan")]
    public float duration = 1f;

    [Tooltip("Kehalusan animasi")]
    public AnimationCurve animationCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    private Vector3 normalPosition; // Posisi tujuan (posisi asli ditaruh di editor)
    private float timer = 0f;
    private bool isAnimating = false;

    void Start()
    {
        // Jika UI Element kosong, ambil otomatis dari Object ini
        if (uiElement == null)
        {
            uiElement = GetComponent<RectTransform>();
        }

        if (uiElement != null)
        {
            // PENTING: Simpan posisi asli (world space) yang kamu atur di Unity Editor
            normalPosition = uiElement.position;

            // Sebelum tampil, lempar jauh UI-nya ke arah "startOffset"
            // Konversi offset ke world space secara kasar (1 unit offset = 1 pixel di kanvas standar)
            // Di Canvas mode Screen Space Overlay, 1 unit coordinate = 1 pixel layar.
            Vector3 worldOffset = new Vector3(startOffset.x, startOffset.y, 0);
            uiElement.position = normalPosition + worldOffset;

            // Memulai Timer
            timer = 0f;
            isAnimating = true;
        }
    }

    void Update()
    {
        if (isAnimating && uiElement != null)
        {
            // Hitung sudah berapa detik berjalan
            timer += Time.deltaTime;
            float progress = Mathf.Clamp01(timer / duration); // persentase 0 sampai 1
            
            // Baca nilai kurva animasi supaya gerakannya halus (tidak kaku)
            float curveValue = animationCurve.Evaluate(progress);

            // Gerakkan perlahan dari "Start" perlahan ke posisi "Normal" menggunakan posisi dunia
            Vector3 worldOffset = new Vector3(startOffset.x, startOffset.y, 0);
            uiElement.position = Vector3.Lerp(normalPosition + worldOffset, normalPosition, curveValue);

            // Kalau selesai, stop animasi
            if (progress >= 1f)
            {
                isAnimating = false;
                uiElement.position = normalPosition; // pastikan pas 100% tepat
            }
        }
    }
}
