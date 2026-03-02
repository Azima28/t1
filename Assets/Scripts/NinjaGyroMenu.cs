using UnityEngine;

/// <summary>
/// Script untuk menampilkan Ninja di background Main Menu.
/// - HP: ninja bergerak sesuai kemiringan (gyroscope)
/// - Laptop/Desktop: ninja mengikuti posisi kursor mouse
/// - Editor: bisa pakai keyboard (A/D) atau mouse
/// </summary>
public class NinjaGyroMenu : MonoBehaviour
{
    [Header("=== Ninja Sprite ===")]
    [Tooltip("SpriteRenderer dari karakter ninja")]
    public SpriteRenderer ninjaSpriteRenderer;

    [Header("=== Sprite Frames ===")]
    [Tooltip("Semua frame sprite untuk Idle animation")]
    public Sprite[] idleSprites;

    [Tooltip("Semua frame sprite untuk Run animation")]
    public Sprite[] runSprites;

    [Header("=== Animation Settings ===")]
    [Tooltip("FPS untuk idle animation")]
    public float idleFPS = 8f;

    [Tooltip("FPS untuk run animation")]
    public float runFPS = 12f;

    [Header("=== Movement Settings ===")]
    [Tooltip("Kecepatan gerak ninja")]
    public float speed = 3f;

    [Tooltip("Batas kiri (world X)")]
    public float minX = -7f;

    [Tooltip("Batas kanan (world X)")]
    public float maxX = 7f;

    [Tooltip("Dead zone - kemiringan minimal sebelum ninja bergerak (derajat)")]
    public float deadZone = 5f;

    [Tooltip("Maksimal derajat kemiringan yang dihitung")]
    public float maxTilt = 45f;

    [Header("=== Desktop / Laptop Settings ===")]
    [Tooltip("Gunakan mouse cursor untuk menggerakkan ninja di desktop/laptop")]
    public bool useMouseCursor = true;

    [Tooltip("Backup: gunakan keyboard (A/D atau Arrow) jika mouse tidak dipakai")]
    public bool useKeyboardFallback = true;

    [Tooltip("Kecepatan ninja mengejar posisi kursor")]
    public float mouseMoveSpeed = 5f;

    // Private
    private float tiltX = 0f;
    private bool isMoving = false;
    private bool facingRight = true;
    private bool gyroSupported = false;
    private Camera mainCam;

    // Animation
    private int currentFrame = 0;
    private float frameTimer = 0f;
    private bool wasMoving = false;

    void Start()
    {
        mainCam = Camera.main;

        // Cek dan aktifkan gyroscope
        if (SystemInfo.supportsGyroscope)
        {
            Input.gyro.enabled = true;
            gyroSupported = true;
            Debug.Log("[NinjaGyro] Gyroscope aktif!");
        }
        else
        {
            Debug.Log("[NinjaGyro] Gyroscope tidak tersedia, menggunakan mouse/keyboard.");
        }

        // Pastikan sprite renderer ada
        if (ninjaSpriteRenderer == null)
        {
            ninjaSpriteRenderer = GetComponentInChildren<SpriteRenderer>();
        }
    }

    void Update()
    {
        ReadInput();
        MoveNinja();
        AnimateSprite();
    }

    /// <summary>
    /// Membaca input dari gyroscope, mouse, atau keyboard
    /// </summary>
    void ReadInput()
    {
        if (gyroSupported)
        {
            // HP: pakai gravity vector dari gyroscope
            Vector3 gravity = Input.gyro.gravity;
            tiltX = gravity.x * maxTilt;
        }
        else if (useMouseCursor && mainCam != null)
        {
            // Desktop/Laptop: ninja mengikuti posisi kursor mouse
            Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
            float diff = mouseWorldPos.x - transform.position.x;

            // Deadzone kecil supaya tidak jitter
            if (Mathf.Abs(diff) > 0.3f)
            {
                tiltX = Mathf.Clamp(diff * 10f, -maxTilt, maxTilt);
            }
            else
            {
                tiltX = 0f;
            }
        }
        else if (useKeyboardFallback)
        {
            // Keyboard fallback
            float horizontal = Input.GetAxis("Horizontal");
            tiltX = horizontal * maxTilt;
        }

        // Clamp
        tiltX = Mathf.Clamp(tiltX, -maxTilt, maxTilt);
    }

    /// <summary>
    /// Menggerakkan ninja berdasarkan kemiringan
    /// </summary>
    void MoveNinja()
    {
        float absTilt = Mathf.Abs(tiltX);
        isMoving = absTilt > deadZone;

        if (isMoving)
        {
            if (!gyroSupported && useMouseCursor && mainCam != null)
            {
                // Mouse mode: ninja bergerak ke arah kursor
                Vector3 mouseWorldPos = mainCam.ScreenToWorldPoint(Input.mousePosition);
                Vector3 pos = transform.position;
                pos.x = Mathf.MoveTowards(pos.x, mouseWorldPos.x, mouseMoveSpeed * Time.deltaTime);
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                transform.position = pos;
            }
            else
            {
                // Gyro / Keyboard mode: kecepatan proporsional
                float normalizedTilt = tiltX / maxTilt;
                float moveSpeed = normalizedTilt * speed;

                Vector3 pos = transform.position;
                pos.x += moveSpeed * Time.deltaTime;
                pos.x = Mathf.Clamp(pos.x, minX, maxX);
                transform.position = pos;
            }

            // Flip arah
            if (tiltX > deadZone && !facingRight)
            {
                Flip();
            }
            else if (tiltX < -deadZone && facingRight)
            {
                Flip();
            }
        }
    }

    /// <summary>
    /// Flip sprite kiri-kanan
    /// </summary>
    void Flip()
    {
        facingRight = !facingRight;
        if (ninjaSpriteRenderer != null)
        {
            ninjaSpriteRenderer.flipX = !facingRight;
        }
    }

    /// <summary>
    /// Animasi sprite frame-by-frame
    /// </summary>
    void AnimateSprite()
    {
        if (ninjaSpriteRenderer == null) return;

        Sprite[] currentSprites = isMoving ? runSprites : idleSprites;
        float currentFPS = isMoving ? runFPS : idleFPS;

        // Kalau tidak ada sprites, skip
        if (currentSprites == null || currentSprites.Length == 0) return;

        // Reset frame jika animasi berubah
        if (isMoving != wasMoving)
        {
            currentFrame = 0;
            frameTimer = 0f;
            wasMoving = isMoving;
        }

        // Advance frame
        frameTimer += Time.deltaTime;
        if (frameTimer >= 1f / currentFPS)
        {
            frameTimer -= 1f / currentFPS;
            currentFrame = (currentFrame + 1) % currentSprites.Length;
        }

        // Set sprite
        ninjaSpriteRenderer.sprite = currentSprites[currentFrame];
    }

    /// <summary>
    /// Visualisasi batas area di Editor
    /// </summary>
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 leftBound = new Vector3(minX, transform.position.y, 0);
        Vector3 rightBound = new Vector3(maxX, transform.position.y, 0);
        Gizmos.DrawLine(leftBound + Vector3.up, leftBound + Vector3.down);
        Gizmos.DrawLine(rightBound + Vector3.up, rightBound + Vector3.down);
    }
}
