using UnityEngine;

/// <summary>
/// Trampolin sederhana dengan 2 sprite (idle dan bounce).
/// Tidak perlu Animator, cukup swap sprite.
/// </summary>
public class Trampoline : MonoBehaviour
{
    [Header("Bounce Settings")]
    [Tooltip("Kekuatan pantulan ke atas")]
    public float bounceForce = 20f;
    
    [Header("Sprites (2 frame)")]
    [Tooltip("Sprite saat idle/normal")]
    public Sprite spriteIdle;
    
    [Tooltip("Sprite saat bounce/tertekan")]
    public Sprite spriteBounce; 
    
    [Tooltip("Durasi sprite bounce (detik)")]
    public float bounceDuration = 0.15f;
    
    [Header("References")]
    [Tooltip("SpriteRenderer (kosongkan jika di GameObject ini)")]
    public SpriteRenderer targetRenderer;
    
    private bool isBouncing = false;

    void Start()
    {
        // Auto-find SpriteRenderer
        if (targetRenderer == null)
        {
            targetRenderer = GetComponent<SpriteRenderer>();
        }
        
        // Set sprite idle
        if (targetRenderer != null && spriteIdle != null)
        {
            targetRenderer.sprite = spriteIdle;
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (!collision.collider.CompareTag("Player")) return;
        
        // Cek apakah player dari atas
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y < -0.5f)
            {
                BouncePlayer(collision.collider);
                break;
            }
        }
    }

    void BouncePlayer(Collider2D player)
    {
        // Bounce player
        Rigidbody2D rb = player.GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.velocity = new Vector2(rb.velocity.x, 0);
            rb.AddForce(Vector2.up * bounceForce, ForceMode2D.Impulse);
        }
        
        // Animasi sprite
        if (!isBouncing && targetRenderer != null)
        {
            StartCoroutine(BounceAnimation());
        }
    }

    System.Collections.IEnumerator BounceAnimation()
    {
        isBouncing = true;
        
        // Ganti ke sprite bounce
        if (spriteBounce != null)
        {
            targetRenderer.sprite = spriteBounce;
        }
        
        yield return new WaitForSeconds(bounceDuration);
        
        // Kembali ke sprite idle
        if (spriteIdle != null)
        {
            targetRenderer.sprite = spriteIdle;
        }
        
        isBouncing = false;
    }
}
