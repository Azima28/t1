using UnityEngine;

/// <summary>
/// Trigger khusus untuk menggerakkan MovingObstacle sejauh/selangkah demi selangkah yang kita atur.
/// Berguna untuk memindahkan obstacle ke posisi baru setiap kali diinjak.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class CustomMoveTrigger : MonoBehaviour
{
    [Header("Target & Movement")]
    [Tooltip("Obstacle yang akan digerakkan (Seret objek Moving Obstacle dari Hierarchy ke kotak ini)")]
    public MovingObstacle targetObstacle;
    
    [Tooltip("Geser seberapa jauh dari posisinya SAAT INI? (Misal X: -3 untuk geser kiri sejauh 3, Y: 2 untuk naik 2 meter)")]
    public Vector2 moveOffset;

    private bool hasTriggered = false;

    void Start()
    {
        // Pastikan colider objek ini diset sebagai trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Cegah nyala berkali-kali kalau belum di-reset
        if (hasTriggered) return;
        
        // Cuma player yang bisa memicu
        if (other.CompareTag("Player"))
        {
            hasTriggered = true;
            
            // Suruh target berpindah sejauh offset
            if (targetObstacle != null)
            {
                targetObstacle.MoveFurther(moveOffset);
            }
            else
            {
                Debug.LogWarning("Target Obstacle belum dimasukkan ke Custom Move Trigger!", this);
            }
        }
    }

    // Bisa dipanggil script lain kalau mau trigger ini bisa diinjak/dipakai ulang
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}
