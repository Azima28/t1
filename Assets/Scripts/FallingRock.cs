using UnityEngine;

/// <summary>
/// Batu yang jatuh saat di-trigger.
/// - Bisa disembunyikan sebelum trigger
/// - Nembus player saat jatuh, solid setelah mendarat
/// </summary>
[RequireComponent(typeof(Rigidbody2D))]
[RequireComponent(typeof(Collider2D))]
public class FallingRock : MonoBehaviour
{
    [Header("Settings")]
    public float fallSpeed = 8f;
    
    [Header("Visibility")]
    [Tooltip("Centang untuk menyembunyikan batu sebelum trigger")]
    public bool hideBeforeTrigger = true;
    
    [Header("Ground Detection")]
    public float groundCheckDistance = 1.5f;
    public LayerMask groundLayer;
    
    private Rigidbody2D rb;
    private Collider2D col;
    private SpriteRenderer sprite;
    private Vector3 startPosition;
    private bool isFalling = false;
    private bool canDamage = false;
    private bool hasLanded = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        col = GetComponent<Collider2D>();
        sprite = GetComponent<SpriteRenderer>();
        
        startPosition = transform.position;
        
        // Sembunyikan jika opsi aktif
        if (sprite != null && hideBeforeTrigger)
        {
            sprite.enabled = false;
        }
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.freezeRotation = true;
        
        col.isTrigger = true;
    }

    public void StartFalling()
    {
        if (isFalling) return;
        
        isFalling = true;
        canDamage = true;
        hasLanded = false;
        
        // Munculkan batu
        if (sprite != null) sprite.enabled = true;
        
        col.isTrigger = true;
        
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.gravityScale = 3f;
        rb.velocity = new Vector2(0, -fallSpeed);
        rb.freezeRotation = true;
    }

    void FixedUpdate()
    {
        if (!isFalling || hasLanded) return;
        
        Vector2 rayStart = transform.position;
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, groundCheckDistance, groundLayer);
        
        if (hit.collider != null)
        {
            float halfHeight = col != null ? col.bounds.size.y / 2f : 0.5f;
            transform.position = new Vector3(transform.position.x, hit.point.y + halfHeight, transform.position.z);
            
            StopAndLand();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (canDamage && other.CompareTag("Player"))
        {
            PlayerDeath death = other.GetComponent<PlayerDeath>();
            if (death != null)
            {
                death.Die();
            }
        }
    }

    void StopAndLand()
    {
        hasLanded = true;
        canDamage = false;
        isFalling = false;
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.velocity = Vector2.zero;
        rb.gravityScale = 0;
        
        col.isTrigger = false;
    }

    public void ResetRock()
    {
        isFalling = false;
        canDamage = false;
        hasLanded = false;
        
        rb.bodyType = RigidbodyType2D.Kinematic;
        rb.gravityScale = 0;
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0;
        rb.freezeRotation = true;
        
        transform.position = startPosition;
        transform.rotation = Quaternion.identity;
        
        col.isTrigger = true;
        
        // Sembunyikan lagi jika opsi aktif
        if (sprite != null && hideBeforeTrigger)
        {
            sprite.enabled = false;
        }
    }
}
