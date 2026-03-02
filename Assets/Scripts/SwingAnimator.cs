using UnityEngine;

/// <summary>
/// Script untuk membuat UI (seperti papan judul) berayun-ayun (swing) seperti digantung dari atas.
/// 
/// --- CARA SETUP DI UNITY ---
/// 1. Buat Empty GameObject di UI (misal nama "TitlePivot")
/// 2. Set posisi "TitlePivot" di bagian ATAS tempat paku/tali akan digantung
/// 3. Pindahkan Gambar Tali dan Gambar Judul menjadi CHILD dari "TitlePivot"
/// 4. Pasang script ini di "TitlePivot"
/// </summary>
public class SwingAnimator : MonoBehaviour
{
    [Header("=== Pengaturan Ayunan ===")]
    [Tooltip("Seberapa jauh ayunannya (dalam derajat rotasi)")]
    public float maxSwingAngle = 10f;

    [Tooltip("Kecepatan ayunan")]
    public float swingSpeed = 2f;

    [Tooltip("Apakah ayunan semakin lama semakin pelan (seperti pendulum asli) atau berayun terus-menerus?")]
    public bool continuousSwing = true;

    [Tooltip("Hanya dipakai jika Continuous Swing = false. Seberapa cepat ayunan melambat/berhenti.")]
    public float damping = 0.5f;

    private float timer = 0f;
    private float currentMaxAngle;
    private RectTransform rectTransform;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        currentMaxAngle = maxSwingAngle;
        
        // Supaya pas mulai gak nunggu, langsung mulai ayun di posisi miring
        timer = Mathf.PI / 2f; // Mulai dari ujung ayunan
    }

    void Update()
    {
        if (rectTransform == null) return;

        // Ayunan menggunakan fungsi Sinusoida (Mathf.Sin) untuk gerakan pendulum yang natural
        timer += Time.deltaTime * swingSpeed;
        
        float angle = Mathf.Sin(timer) * currentMaxAngle;
        
        // Terapkan rotasi di sumbu Z (ayunan 2D)
        rectTransform.localRotation = Quaternion.Euler(0, 0, angle);

        // Jika tidak berayun selamanya (Continuous), perlahan-lahan kurangi sudut maksimum ayunan
        if (!continuousSwing && currentMaxAngle > 0.1f)
        {
            currentMaxAngle = Mathf.Lerp(currentMaxAngle, 0, Time.deltaTime * damping);
        }
    }

    /// <summary>
    /// Panggil fungsi ini (misal lewat button atau script lain) 
    /// jika ingin menendang papannya supaya berayun kencang lagi
    /// </summary>
    public void KickSwing(float forceAngle)
    {
        currentMaxAngle = forceAngle;
        timer = Mathf.PI / 2f;
    }
}
