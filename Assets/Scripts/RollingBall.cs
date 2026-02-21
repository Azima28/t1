using UnityEngine;

/// <summary>
/// Bola menggelinding dengan sistem "Ground-Snapping".
/// Selalu menempel pada permukaan ground/slope meskipun collider slope tidak beraturan.
/// Menghindari masalah bola "nembus" atau "stuck" di physics.
/// 
/// SETUP:
/// 1. Taruh bola sedikit di atas slope.
/// 2. Set Ground Layer (misal Terrain atau Ground).
/// 3. Set Direction (1=kanan, -1=kiri).
/// </summary>
[RequireComponent(typeof(CircleCollider2D))]
public class RollingBall : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Arah: 1 = ke kanan, -1 = ke kiri")]
    public float direction = 1f;
    
    [Tooltip("Kecepatan menggelinding")]
    public float speed = 7f;
    
    [Tooltip("Percepatan")]
    public float acceleration = 1f;
    
    [Tooltip("Kecepatan rotasi visual")]
    public float rotationSpeed = 360f;

    [Header("Ground Snapping")]
    [Tooltip("Layer tanah/slope agar bola menempel")]
    public LayerMask groundLayer;
    
    [Tooltip("Jarak deteksi ke bawah")]
    public float snapDistance = 3f;
    
    [Tooltip("Seberapa cepat bola menempel ke tanah (leSmoothing)")]
    public float snapSmoothness = 20f;

    [Header("Activation")]
    public bool requireTrigger = false;
    public bool hideBeforeTrigger = false;
    
    [Header("Damage")]
    public bool canKillPlayer = true;

    private CircleCollider2D col;
    private SpriteRenderer sprite;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool isActive = false;
    private float currentSpeed;
    private float verticalVelocity = 0f;
    private float radius;

    void Start()
    {
        col = GetComponent<CircleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        
        // Set Rigidbody ke Kinematic agar bola tidak jatuh pakai fisika bawaan Unity, 
        // tapi sistem Trigger (sentuhan dengan Player) tetap aktif.
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.simulated = true;
        }

        startPosition = transform.position;
        startRotation = transform.rotation;
        
        radius = col.radius * transform.lossyScale.y;
        col.isTrigger = true;

        if (!requireTrigger)
        {
            StartRolling();
        }
        else
        {
            if (sprite != null && hideBeforeTrigger)
                sprite.enabled = false;
        }
    }

    public void StartRolling()
    {
        if (isActive) return;
        isActive = true;
        currentSpeed = speed;
        verticalVelocity = 0f;
        
        if (sprite != null) sprite.enabled = true;
        col.enabled = true;
    }

    void Update()
    {
        if (!isActive) return;

        // 1. Tambah kecepatan horizontal (ignore timeScale = 0)
        currentSpeed += acceleration * Time.unscaledDeltaTime;

        // 2. Cek Tembok (Wall Bouncing)
        float moveX = direction * currentSpeed * Time.unscaledDeltaTime;
        RaycastHit2D wallHit = Physics2D.Raycast(transform.position, new Vector2(direction, 0), radius + Mathf.Abs(moveX), groundLayer);
        
        if (wallHit.collider != null)
        {
            // Jika mendeteksi tembok, ganti arah (mantul)
            direction *= -1f;
            moveX = direction * currentSpeed * Time.unscaledDeltaTime; // Hitung ulang gerak dengan arah baru
        }

        // 3. Gerak Horizontal
        Vector3 nextPos = transform.position + new Vector3(moveX, 0, 0);

        // 4. Ground Snapping & Gravity
        RaycastHit2D hit = Physics2D.Raycast(nextPos, Vector2.down, snapDistance, groundLayer);
        
        if (hit.collider != null)
        {
            float targetY = hit.point.y + radius;
            float smoothedY = Mathf.Lerp(transform.position.y, targetY, Time.unscaledDeltaTime * snapSmoothness);
            
            transform.position = new Vector3(nextPos.x, smoothedY, 0);
            verticalVelocity = 0f;
        }
        else
        {
            verticalVelocity += Physics2D.gravity.y * 3f * Time.unscaledDeltaTime;
            nextPos.y += verticalVelocity * Time.unscaledDeltaTime;
            transform.position = nextPos;
        }

        // 5. Rotasi Visual
        transform.Rotate(0, 0, -direction * rotationSpeed * Time.unscaledDeltaTime);
        
        // 6. Manual Player Detection (karena OnTriggerEnter2D tidak jalan saat timeScale = 0)
        if (canKillPlayer)
        {
            CheckPlayerHitManual();
        }
    }
    
    /// <summary>
    /// Cek jarak bola ke player secara manual.
    /// Diperlukan karena saat game pause (timeScale=0), physics engine mati
    /// dan OnTriggerEnter2D tidak dipanggil.
    /// </summary>
    void CheckPlayerHitManual()
    {
        GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj == null) return;
        
        float dist = Vector2.Distance(transform.position, playerObj.transform.position);
        
        // Jika jarak cukup dekat (radius bola + sedikit toleransi)
        if (dist < radius + 0.5f)
        {
            PlayerDeath death = playerObj.GetComponent<PlayerDeath>();
            if (death != null)
            {
                // Tutup Math UI dulu biar ga stuck
                if (MathTrapManager.Instance != null)
                {
                    MathTrapManager.Instance.ForceClose();
                }
                
                death.Die();
            }
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!canKillPlayer || !isActive) return;

        if (other.CompareTag("Player"))
        {
            PlayerDeath death = other.GetComponent<PlayerDeath>();
            if (death != null)
            {
                death.Die();
            }
        }
    }

    public void ResetBall()
    {
        transform.position = startPosition;
        transform.rotation = startRotation;
        currentSpeed = speed;
        verticalVelocity = 0f;
        isActive = false;

        if (requireTrigger)
        {
            if (sprite != null && hideBeforeTrigger)
                sprite.enabled = false;
        }
        else
        {
            StartRolling();
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawRay(transform.position, Vector3.down * snapDistance);
    }
}
