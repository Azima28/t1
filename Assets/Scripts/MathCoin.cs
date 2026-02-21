using UnityEngine;

/// <summary>
/// Koin jebakan Matematika. Kalau kena, game pause dan pop up soal.
/// Bisa kombinasikan untuk memanggil trap lain (layaknya TrapTrigger).
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class MathCoin : MonoBehaviour
{
    [Header("Combo Traps (Optional)")]
    [Tooltip("Trap yang ikut aktif saat koin kena (dibuat untuk lose-lose scenario)")]
    public FallingRock[] fallingRocks;
    public RisingObstacle[] risingObstacles;
    public RollingBall[] rollingBalls;

    private SpriteRenderer spriteRenderer;
    private Collider2D col;
    private bool hasTriggered = false;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        col = GetComponent<Collider2D>();
        
        // Pastikan koin adalah Trigger
        col.isTrigger = true;

        // Bikin manager otomatis kalau belum ada di scene
        if (MathTrapManager.Instance == null)
        {
            GameObject manager = new GameObject("MathTrapManager");
            manager.AddComponent<MathTrapManager>();
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        
        if (other.CompareTag("Player"))
        {
            PlayerDeath playerDeath = other.GetComponent<PlayerDeath>();
            if (playerDeath != null)
            {
                TriggerCoin(playerDeath);
            }
        }
    }

    void TriggerCoin(PlayerDeath player)
    {
        hasTriggered = true;
        
        // Sembunyikan Koin
        if (spriteRenderer != null) spriteRenderer.enabled = false;
        col.enabled = false;

        // Panggil Manager untuk pause game dan nanya Matematika
        MathTrapManager.Instance.ShowQuestion(this, player);

        // --- AKTIFKAN TRAP LAIN (COMBO TROLL) ---
        if (fallingRocks != null)
        {
            foreach (var trap in fallingRocks) { if (trap != null) trap.StartFalling(); }
        }
        
        if (risingObstacles != null)
        {
            foreach (var trap in risingObstacles) { if (trap != null) trap.StartRising(); }
        }
        
        if (rollingBalls != null)
        {
            foreach (var trap in rollingBalls) { if (trap != null) trap.StartRolling(); }
        }
    }

    /// <summary>
    /// Hasil dari jawaban matematika.
    /// dipanggil oleh MathTrapManager.
    /// </summary>
    public void Resolve(bool isSuccess)
    {
        if (isSuccess)
        {
            // Bener -> Bebas hambatan, koin hilang total
            gameObject.SetActive(false);
        }
        else
        {
            // Salah -> Biarkan mati (MathTrapManager bunuh player).
            // Nanti di-reset sama PlayerDeath.cs
        }
    }

    /// <summary>
    /// Reset saat player respawn. Dipanggil oleh PlayerDeath.
    /// </summary>
    public void ResetCoin()
    {
        hasTriggered = false;
        gameObject.SetActive(true);
        if (spriteRenderer != null) spriteRenderer.enabled = true;
        col.enabled = true;
    }
}
