using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float speed = 5f;
    public float jumpForce = 10f;

    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private bool isGrounded = false;
    private float moveX;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // Cek apakah Rigidbody2D ada
        if (rb == null)
        {
            Debug.LogError("PlayerMovement butuh Rigidbody2D! Tambahkan Rigidbody2D ke object ini.");
            return;
        }
        
        rb.freezeRotation = true;
        
        // Memastikan deteksi tabrakan lebih akurat untuk objek yang bergerak cepat
        rb.collisionDetectionMode = CollisionDetectionMode2D.Continuous;
        
        // Menghaluskan pergerakan sprite
        rb.interpolation = RigidbodyInterpolation2D.Interpolate;
        
        // PENTING: Cegah Rigidbody "tidur" agar ground detection selalu aktif
        rb.sleepMode = RigidbodySleepMode2D.NeverSleep;

        // Biar tidak nempel di tembok saat loncat (friction = 0)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            PhysicsMaterial2D noFriction = new PhysicsMaterial2D("NoFriction");
            noFriction.friction = 0f;
            noFriction.bounciness = 0f;
            col.sharedMaterial = noFriction;
        }
    }

    void Update()
    {
        if (rb == null) return;

        // Gabungkan input dari MoveButton (mobile) DAN keyboard
        float mobileInput = MoveButton.Input;
        float keyboardInput = Input.GetAxisRaw("Horizontal");
        
        // Prioritas: mobile jika ada, kalau tidak pakai keyboard
        if (Mathf.Abs(mobileInput) > 0.1f)
        {
            moveX = mobileInput;
        }
        else
        {
            moveX = keyboardInput;
        }

        // Berbalik arah sesuai gerakan (sprite default menghadap kiri)
        if (moveX > 0)
        {
            spriteRenderer.flipX = true; // Menghadap kanan
        }
        else if (moveX < 0)
        {
            spriteRenderer.flipX = false; // Menghadap kiri
        }

        // Set animator parameters
        if (animator != null)
        {
            bool isMoving = Mathf.Abs(moveX) > 0;
            
            animator.SetBool("IsIdle", !isMoving && isGrounded);
            animator.SetBool("IsRunning", isMoving && isGrounded);
            animator.SetBool("IsJump", !isGrounded);
        }

        // Loncat (Space keyboard ATAU JumpButton mobile)
        bool jumpPressed = Input.GetKeyDown(KeyCode.Space) || JumpButton.IsPressed;
        if (jumpPressed && isGrounded)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            isGrounded = false;
        }
    }

    void FixedUpdate()
    {
        if (rb == null) return;

        // Terapkan pergerakan di FixedUpdate untuk kestabilan fisika
        rb.velocity = new Vector2(moveX * speed, rb.velocity.y);
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        // Mengecek apakah tabrakan terjadi dari bawah (tanah)
        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.y > 0.5f) // Normal ke atas berarti kita di atas sesuatu
            {
                isGrounded = true;
                break;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        // Deteksi langsung saat mendarat
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
