using UnityEngine;

/// <summary>
/// Bola yang menggelinding mengikuti gravitasi dan slope.
/// Bisa membunuh player saat menyentuh.
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(CircleCollider2D))]
public class RollingBall : MonoBehaviour
{
    [Header("Settings")]
    [Tooltip("Kecepatan awal menggelinding")]
    public float initialSpeed = 2f;
    
    [Tooltip("Arah awal (-1 = kiri, 1 = kanan, 0 = diam)")]
    public float initialDirection = 0f;
    
    [Header("Physics")]
    [Tooltip("Friction/gesekan permukaan (0 = licin, 1 = kasar)")]
    public float friction = 0.4f;
    
    [Tooltip("Bounciness/pantulan (0 = tidak mental, 1 = sangat mental)")]
    public float bounciness = 0.2f;
    
    [Header("Activation")]
    [Tooltip("Butuh trigger untuk mulai bergerak")]
    public bool requireTrigger = false;
    
    [Tooltip("Sembunyikan sebelum trigger")]
    public bool hideBeforeTrigger = false;
    
    [Header("Damage")]
    public bool canKillPlayer = true;
    
    private Rigidbody2D rb;
    private CircleCollider2D col;
    private SpriteRenderer sprite;
    private Vector3 startPosition;
    private bool isActive = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<CircleCollider2D>();
        sprite = GetComponent<SpriteRenderer>();
        
        startPosition = transform.position;
        
        // Setup Rigidbody untuk rolling natural
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        rb.angularDrag = 0.5f; // Sedikit drag untuk rotasi
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // Setup Physics Material
        PhysicsMaterial2D material = new PhysicsMaterial2D("BallMaterial");
        material.friction = friction;
        material.bounciness = bounciness;
        col.sharedMaterial = material;
        
        // Auto-start jika tidak butuh trigger
        if (!requireTrigger)
        {
            StartRolling();
        }
        else
        {
            // Freeze sampai di-trigger
            rb.bodyType = RigidbodyType2D.Kinematic;
            rb.velocity = Vector2.zero;
            
            if (sprite != null && hideBeforeTrigger)
            {
                sprite.enabled = false;
            }
        }
    }

    /// <summary>
    /// Mulai menggelinding (dipanggil oleh TrapTrigger)
    /// </summary>
    public void StartRolling()
    {
        if (isActive) return;
        
        isActive = true;
        
        if (sprite != null) sprite.enabled = true;
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 1f;
        
        // Berikan kecepatan awal
        if (Mathf.Abs(initialDirection) > 0.1f)
        {
            rb.velocity = new Vector2(initialDirection * initialSpeed, 0);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!canKillPlayer) return;
        if (!isActive && requireTrigger) return;
        
        if (collision.collider.CompareTag("Player"))
        {
            PlayerDeath death = collision.collider.GetComponent<PlayerDeath>();
            if (death != null)
            {
                death.Die();
            }
        }
    }

    /// <summary>
    /// Reset ke posisi awal
    /// </summary>
    public void ResetBall()
    {
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        
        if (requireTrigger)
        {
            isActive = false;
            rb.bodyType = RigidbodyType2D.Kinematic;
            
            if (sprite != null && hideBeforeTrigger)
            {
                sprite.enabled = false;
            }
        }
        else
        {
            StartRolling();
        }
    }
}
