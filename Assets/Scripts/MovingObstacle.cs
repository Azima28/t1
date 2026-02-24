using UnityEngine;

/// <summary>
/// Obstacle yang bergerak horizontal (kiri/kanan).
/// Bisa satu arah atau bolak-balik.
/// </summary>
public class MovingObstacle : MonoBehaviour
{
    public enum MoveDirection
    {
        LeftOnly,      // Ke kiri saja lalu balik
        RightOnly,     // Ke kanan saja lalu balik
        LeftAndRight   // Kiri dan kanan (ping-pong)
    }

    [Header("Movement")]
    [Tooltip("Arah gerak")]
    public MoveDirection direction = MoveDirection.LeftAndRight;
    
    [Tooltip("Jarak gerak ke kiri (dari posisi awal)")]
    public float leftDistance = 3f;
    
    [Tooltip("Jarak gerak ke kanan (dari posisi awal)")]
    public float rightDistance = 3f;
    
    [Tooltip("Kecepatan gerak")]
    public float speed = 4f;
    
    [Header("Behavior")]
    [Tooltip("Centang agar obstacle DIAM SELAMANYA di ujung (tidak pernah kembali)")]
    public bool stayForever = false;
    
    [Tooltip("Loop terus menerus")]
    public bool loopMode = true;
    
    [Tooltip("Butuh trigger untuk mulai bergerak")]
    public bool requireTrigger = false;
    
    [Tooltip("Delay saat sampai di ujung (detik)")]
    public float delayAtEdge = 0f;
    
    [Header("Visibility")]
    [Tooltip("Sembunyikan sebelum trigger")]
    public bool hideBeforeTrigger = false;
    
    [Header("Damage")]
    public bool canKillPlayer = true;
    
    [Tooltip("Damage terus menerus (bahkan saat diam)")]
    public bool alwaysDamage = false;
    
    private Vector3 startPosition;
    private Vector3 leftPosition;
    private Vector3 rightPosition;
    private Vector3 targetPosition;
    private SpriteRenderer sprite;
    private Rigidbody2D rb;
    private bool isActive = false;
    private bool isWaiting = false;
    private bool movingRight = true;
    private bool hasReachedEnd = false;

    void Start()
    {
        sprite = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        
        startPosition = transform.position;
        leftPosition = startPosition + Vector3.left * leftDistance;
        rightPosition = startPosition + Vector3.right * rightDistance;
        
        // Setup Rigidbody
        if (rb != null)
        {
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.gravityScale = 0;
            rb.freezeRotation = true;
        }
        
        // Set target awal berdasarkan direction
        SetInitialTarget();
        
        // Auto-start jika tidak butuh trigger
        if (!requireTrigger)
        {
            isActive = true;
            if (sprite != null) sprite.enabled = true;
        }
        else
        {
            if (sprite != null && hideBeforeTrigger)
            {
                sprite.enabled = false;
            }
        }
    }

    void SetInitialTarget()
    {
        switch (direction)
        {
            case MoveDirection.LeftOnly:
                targetPosition = leftPosition;
                movingRight = false;
                break;
            case MoveDirection.RightOnly:
                targetPosition = rightPosition;
                movingRight = true;
                break;
            case MoveDirection.LeftAndRight:
                targetPosition = rightPosition; // Mulai ke kanan
                movingRight = true;
                break;
        }
    }

    /// <summary>
    /// Mulai bergerak (dipanggil oleh TrapTrigger)
    /// </summary>
    public void StartMoving()
    {
        if (isActive) return;
        
        isActive = true;
        hasReachedEnd = false;
        
        if (sprite != null) sprite.enabled = true;
    }

    void Update()
    {
        if (!isActive || isWaiting) return;
        
        // Gerak ke target
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, speed * Time.deltaTime);
        
        // Cek apakah sudah sampai target
        if (Vector3.Distance(transform.position, targetPosition) < 0.01f)
        {
            OnReachTarget();
        }
    }

    void OnReachTarget()
    {
        switch (direction)
        {
            case MoveDirection.LeftOnly:
                HandleOneWayMovement(leftPosition, startPosition);
                break;
                
            case MoveDirection.RightOnly:
                HandleOneWayMovement(rightPosition, startPosition);
                break;
                
            case MoveDirection.LeftAndRight:
                HandlePingPongMovement();
                break;
        }
    }

    void HandleOneWayMovement(Vector3 endPos, Vector3 returnPos)
    {
        if (!hasReachedEnd)
        {
            // Sudah sampai ujung
            hasReachedEnd = true;
            
            // Diam selamanya di tujuan jika dicentang
            if (stayForever)
            {
                isActive = false;
                return;
            }
            
            // Balik ke start
            targetPosition = returnPos;
            
            if (delayAtEdge > 0)
            {
                isWaiting = true;
                Invoke(nameof(EndWait), delayAtEdge);
            }
        }
        else
        {
            // Sudah kembali ke start
            if (loopMode)
            {
                hasReachedEnd = false;
                targetPosition = endPos;
                
                if (delayAtEdge > 0)
                {
                    isWaiting = true;
                    Invoke(nameof(EndWait), delayAtEdge);
                }
            }
            else
            {
                isActive = false;
            }
        }
    }

    void HandlePingPongMovement()
    {
        if (movingRight)
        {
            // Sampai kanan, balik ke kiri
            targetPosition = leftPosition;
            movingRight = false;
        }
        else
        {
            // Sampai kiri, balik ke kanan
            targetPosition = rightPosition;
            movingRight = true;
        }
        
        if (delayAtEdge > 0)
        {
            isWaiting = true;
            Invoke(nameof(EndWait), delayAtEdge);
        }
        
        // Jika tidak loop, cek apakah sudah kembali ke posisi tengah
        if (!loopMode && Vector3.Distance(transform.position, startPosition) < 0.1f)
        {
            isActive = false;
        }
    }

    void EndWait()
    {
        isWaiting = false;
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
        
        isWaiting = false;
        hasReachedEnd = false;
        transform.position = startPosition;
        
        SetInitialTarget();
        
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
        }
        
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
    
    // Gambar gizmo untuk visualisasi
    void OnDrawGizmosSelected()
    {
        Vector3 pos = Application.isPlaying ? startPosition : transform.position;
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(pos + Vector3.left * leftDistance, 0.2f);
        Gizmos.DrawLine(pos, pos + Vector3.left * leftDistance);
        
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(pos + Vector3.right * rightDistance, 0.2f);
        Gizmos.DrawLine(pos, pos + Vector3.right * rightDistance);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(pos, 0.2f);
    }
}
