using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float jumpForce = 10f;
    
    [Header("Effects")]
    [Tooltip("Efek asap/partikel saat double jump")]
    public GameObject doubleJumpEffect;

    [Header("Ground Snapping (untuk slope)")]
    [Tooltip("Aktifkan agar player menempel di slope")]
    public bool enableGroundSnapping = true;
    
    [Tooltip("Jarak raycast ke bawah untuk deteksi ground")]
    public float groundCheckDistance = 0.3f;
    
    [Tooltip("Kekuatan snap ke ground (makin besar makin kuat)")]
    public float snapForce = 20f;
    
    [Tooltip("Layer untuk ground")]
    public LayerMask groundLayer;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Collider2D playerCollider;
    private bool isGrounded = false;
    private bool canDoubleJump = false; // Status apakah sedang bisa double jump
    private float moveX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        playerCollider = GetComponent<Collider2D>();
        
        if (rb == null)
        {
            Debug.LogError("PlayerMovement butuh Rigidbody2D!");
            return;
        }
        
        rb.freezeRotation = true;
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        // No friction - player licin
        if (playerCollider != null)
        {
            PhysicsMaterial2D noFriction = new PhysicsMaterial2D("NoFriction");
            noFriction.friction = 0f;
            noFriction.bounciness = 0f;
            playerCollider.sharedMaterial = noFriction;
        }
        
        if (groundLayer == 0)
        {
            groundLayer = LayerMask.GetMask("Default", "Ground", "Slope");
        }
    }

    void Update()
    {
        if (rb == null) return;

        // Input handling
        float mobileInput = MoveButton.Input;
        float keyboardInput = Input.GetAxisRaw("Horizontal");
        
        if (Mathf.Abs(mobileInput) > 0.1f)
        {
            moveX = mobileInput;
        }
        else
        {
            moveX = keyboardInput;
        }

        // Flip sprite
        if (moveX > 0)
        {
            spriteRenderer.flipX = false;
        }
        else if (moveX < 0)
        {
            spriteRenderer.flipX = true;
        }

        // Animator
        if (animator != null)
        {
            bool isMoving = Mathf.Abs(moveX) > 0;
            animator.SetBool("IsIdle", !isMoving && isGrounded);
            animator.SetBool("IsRunning", isMoving && isGrounded);
            
            // Pisahkan loncat (naik) dan jatuh (turun)
            bool isJumping = !isGrounded && rb.velocity.y > 0.1f;
            bool isFalling = !isGrounded && rb.velocity.y < -0.1f;
            
            animator.SetBool("IsJump", isJumping);
            animator.SetBool("IsFall", isFalling);
        }

        // Jump
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || JumpButton.IsPressed;
        if (jumpPressed)
        {
            if (isGrounded)
            {
                // Loncat pertama dari tanah
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                isGrounded = false;
                canDoubleJump = true; // Izinkan double jump setelah lompatan pertama
            }
            else if (canDoubleJump)
            {
                // Double jump selama di udara (hanya jika sudah lompat dari tanah, bukan saat fall/jatuh biasa)
                rb.velocity = new Vector2(rb.velocity.x, jumpForce);
                canDoubleJump = false; // Matikan double jump agar tidak bisa lompat berkali-kali
                
                // Munculkan efek asap saat double jump
                if (doubleJumpEffect != null)
                {
                    // Posisi efek sedikit di bawah player (di area kaki)
                    Vector3 effectPosition = transform.position + new Vector3(0, -0.5f, 0);
                    Instantiate(doubleJumpEffect, effectPosition, Quaternion.identity);
                }
            }
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Movement
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
        
        // Ground Snapping
        if (enableGroundSnapping && isGrounded && rb.velocity.y <= 0.1f)
        {
            ApplyGroundSnapping();
        }
    }

    void ApplyGroundSnapping()
    {
        Vector2 rayStart = transform.position;
        float rayDistance = groundCheckDistance;
        
        if (playerCollider != null)
        {
            rayStart = (Vector2)transform.position + Vector2.down * (playerCollider.bounds.extents.y * 0.5f);
            rayDistance = playerCollider.bounds.extents.y + groundCheckDistance;
        }
        
        RaycastHit2D hit = Physics2D.Raycast(rayStart, Vector2.down, rayDistance, groundLayer);
        
        if (hit.collider != null)
        {
            float distanceToGround = hit.distance - (playerCollider != null ? playerCollider.bounds.extents.y : 0.5f);
            
            if (distanceToGround > 0.01f && distanceToGround < groundCheckDistance)
            {
                rb.velocity = new Vector2(rb.velocity.x, -snapForce * Time.fixedDeltaTime);
            }
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}
