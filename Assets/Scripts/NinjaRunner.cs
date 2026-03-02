using UnityEngine;

/// <summary>
/// Script untuk menampilkan Ninja yang lari di tempat (continuous running animation).
/// Cocok ditaruh di pojok layar (misal: Pojok Kiri Bawah) pada Scene Level.
/// 
/// --- CARA SETUP DI UNITY ---
/// 1. Buka Scene "Level".
/// 2. Buat Empty GameObject bernama "NinjaRunner" (atau copy dari MainMenu lalu ubah scriptnya).
/// 3. Posisikan di pojok kiri bawah layar.
/// 4. Tambahkan SpriteRenderer dan pasang script ini.
/// 5. Masukkan semua frame 'Run__000' sampai 'Run__009' ke kolom Run Sprites di Inspector.
/// </summary>
public class NinjaRunner : MonoBehaviour
{
    [Header("=== Ninja Sprite ===")]
    [Tooltip("SpriteRenderer dari karakter ninja")]
    public SpriteRenderer ninjaSpriteRenderer;

    [Header("=== Run Animation Frames ===")]
    [Tooltip("Semua frame sprite untuk animasi Lari (Run__000 s/d Run__009)")]
    public Sprite[] runSprites;

    [Header("=== Animation Settings ===")]
    [Tooltip("Kecepatan lari (Frame Per Second)")]
    public float runFPS = 12f;

    // Private variables untuk menghitung frame
    private int currentFrame = 0;
    private float frameTimer = 0f;

    void Start()
    {
        // Cari otomatis jika lupa di-assign
        if (ninjaSpriteRenderer == null)
        {
            ninjaSpriteRenderer = GetComponent<SpriteRenderer>();
            
            // Kalau tidak ada di object ini, cari di child
            if (ninjaSpriteRenderer == null)
                ninjaSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }

        // Peringatan jika sprite belum diisi
        if (runSprites == null || runSprites.Length == 0)
        {
            Debug.LogWarning("[NinjaRunner] Anda belum memasukkan gambar Run Sprites di Inspector!");
        }
    }

    void Update()
    {
        AnimateRun();
    }

    /// <summary>
    /// Memutar animasi frame-by-frame terus menerus
    /// </summary>
    void AnimateRun()
    {
        if (ninjaSpriteRenderer == null || runSprites == null || runSprites.Length == 0) 
            return;

        // Tambah timer berdasarkan waktu nyata
        frameTimer += Time.deltaTime;

        // Jika waktunya ganti frame (berdasarkan FPS)
        if (frameTimer >= 1f / runFPS)
        {
            frameTimer -= 1f / runFPS; // Kurangi timer untuk frame berikutnya
            
            // Lanjut ke frame selanjutnya, kalau mentok balik ke 0 (looping)
            currentFrame = (currentFrame + 1) % runSprites.Length;
        }

        // Ganti gambar sprite saat ini
        ninjaSpriteRenderer.sprite = runSprites[currentFrame];
    }
}
