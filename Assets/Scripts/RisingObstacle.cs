using UnityEngine;

/// <summary>
/// Obstacle yang muncul dari bawah ke atas.
/// Bisa naik-turun terus menerus (loop) atau sekali saja.
/// </summary>
public class RisingObstacle : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Jarak naik dari posisi awal")]
    public float riseHeight = 2f;
    
    [Tooltip("Kecepatan naik/turun")]
    public float speed = 8f;
    
    [Header("Activation")]
    [Tooltip("Centang jika butuh trigger dulu untuk bergerak")]
    public bool requireTrigger = true;
    
    [Header("Loop Mode")]
    [Tooltip("Centang untuk naik-turun terus menerus")]
    public bool loopMode = false;
    
    [Tooltip("Delay saat di atas (detik)")]
    public float delayAtTop = 0.5f;
    
    [Tooltip("Delay saat di bawah (detik)")]
    public float delayAtBottom = 0.5f;
    
    [Header("Triggered Mode (Non-Loop)")]
    [Tooltip("Centang agar obstacle DIAM SELAMANYA di tujuan (tidak pernah kembali)")]
    public bool stayForever = false;
    
    [Tooltip("Tinggi khusus saat stay forever, biarkan 0 jika ingin sesuai dengan rise height")]
    public float stayForeverHeight = 0f;
    
    [Tooltip("Delay sebelum turun (hanya berlaku jika stayForever = false)")]
    public float stayDuration = 0f;
    
    [Header("Physics")]
    [Tooltip("Centang agar tidak menggelinding/jatuh")]
    public bool stayInPlace = true;
    
    [Header("Visibility")]
    [Tooltip("Sembunyikan sebelum aktif (hanya jika require trigger)")]
    public bool hideBeforeTrigger = true;
    
    [Header("Damage")]
    public bool canKillPlayer = true;
    
    [Tooltip("Centang untuk damage terus menerus (bahkan saat diam)")]
    public bool alwaysDamage = false;
    
    private Vector3 startPosition;
    private Vector3 targetPosition;
    private Vector3 stayForeverPosition;
    private SpriteRenderer sprite;
    private Collider2D col;
    private Rigidbody2D rb;
    private bool isActive = false;
    private bool movingUp = true;
    private bool isWaiting = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        rb = GetComponent<Rigidbody2D>();
        
        startPosition = transform.position;
        targetPosition = startPosition + new Vector3(0, riseHeight, 0);
        stayForeverPosition = startPosition + new Vector3(0, stayForeverHeight != 0f ? stayForeverHeight : riseHeight, 0);
        
        // Setup Rigidbody
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            if (stayInPlace) rb.freezeRotation = true;
        }
        
        // Jika TIDAK butuh trigger, langsung aktif
        if (!requireTrigger)
        {
            isActive = true;
            if (sprite != null) sprite.enabled = true;
        }
        else
        {
            // Butuh trigger - sembunyikan jika opsi aktif
            if (sprite != null && hideBeforeTrigger)
            {
                sprite.enabled = false;
            }
        }
    }

    /// <summary>
    /// Mulai bergerak (dipanggil oleh TrapTrigger)
    /// </summary>
    public void StartRising()
    {
        if (isActive) return;
        
        isActive = true;
        movingUp = true;
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            rb.velocity = Vector2.zero;
        }
        
        if (sprite != null) sprite.enabled = true;
    }

    void Update()
    {
        if (!isActive || isWaiting) return;
        
        if (loopMode)
        {
            UpdateLoopMode();
        }
        else
        {
            UpdateTriggeredMode();
        }
    }

    void UpdateLoopMode()
    {
        Vector3 target = movingUp ? targetPosition : startPosition;
        transform.position = Vector3.MoveTowards(transform.position, target, speed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, target) < 0.01f)
        {
            isWaiting = true;
            float delay = movingUp ? delayAtTop : delayAtBottom;
            movingUp = !movingUp;
            Invoke(nameof(EndWait), delay);
        }
    }

    void UpdateTriggeredMode()
    {
        if (!movingUp) return;
        
        // Pindah ke target tertinggi HINGGA selesai
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            movingUp = false;
            
            if (rb != null)
            {
                rb.bodyType = RigidbodyType2D.Kinematic;
                rb.velocity = Vector2.zero;
            }
            
            // Logika penurunan
            if (stayForever)
            {
                // Jika stay forever, apakah ada tinggi khusus? (Asumsi tidak 0)
                if (stayForeverHeight > 0.01f && stayForeverHeight != riseHeight)
                {
                    Invoke(nameof(StartLoweringToStayForeverHeight), stayDuration);
                }
                // Jika tidak ada tinggi khusus atau tingginya sama, tidak ngapa-ngapain. (Stay di target)
            }
            else
            {
                // Kalau tidak stay forever, maka turun ke start secara normal
                if (stayDuration >= 0)
                {
                    Invoke(nameof(StartLowering), stayDuration);
                }
            }
        }
    }

    void EndWait()
    {
        isWaiting = false;
    }

    void StartLoweringToStayForeverHeight()
    {
        StartCoroutine(LowerDownTo(stayForeverPosition, true));
    }

    void StartLowering()
    {
        StartCoroutine(LowerDownTo(startPosition, false));
    }

    System.Collections.IEnumerator LowerDownTo(Vector3 destination, bool keepActive)
    {
        while (Vector3.Distance(transform.position, destination) > 0.01f)
        {
            transform.position = Vector3.MoveTowards(transform.position, destination, speed * Time.deltaTime);
            yield return null;
        }
        
        transform.position = destination;
        
        if (!keepActive)
        {
            if (!loopMode)
            {
                isActive = false;
                if (sprite != null && hideBeforeTrigger && requireTrigger)
                {
                    sprite.enabled = false;
                }
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        HandlePlayerCollision(collision.collider);
    }
    
    void OnTriggerEnter2D(Collider2D other)
    {
        HandlePlayerCollision(other);
    }

    void HandlePlayerCollision(Collider2D other)
    {
        if (!canKillPlayer) return;
        
        // Jika tidak alwaysDamage, hanya damage saat aktif
        if (!alwaysDamage && !isActive) return;
        
        if (other.CompareTag("Player"))
        {
            PlayerDeath death = other.GetComponent<PlayerDeath>();
            if (death != null)
            {
                death.Die();
            }
        }
    }

    /// <summary>
    /// Reset ke posisi awal
    /// </summary>
    public void ResetObstacle()
    {
        CancelInvoke();
        StopAllCoroutines();
        
        isWaiting = false;
        movingUp = true;
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            rb.angularVelocity = 0;
            rb.gravityScale = 0;
        }
        
        // Reset berdasarkan apakah butuh trigger
        if (!requireTrigger)
        {
            isActive = true;
            if (sprite != null) sprite.enabled = true;
        }
        else
        {
            isActive = false;
            if (sprite != null && hideBeforeTrigger)
            {
                sprite.enabled = false;
            }
        }
    }
}
