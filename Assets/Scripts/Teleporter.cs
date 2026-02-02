using UnityEngine;

/// <summary>
/// Teleporter untuk PINDAH TEMPAT saja (tanpa naik level).
/// Tanpa efek fade - langsung pindah.
/// </summary>
public class Teleporter : MonoBehaviour
{
    [Header("Teleport Settings")]
    public Transform destination;
    
    [Header("Camera Zone Tujuan (Opsional)")]
    [Tooltip("Jika di-assign, kamera akan ganti zone saat teleport")]
    public CameraZone destinationZone;

    private bool isTeleporting = false;

    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;
        if (destination == null) return;
        if (isTeleporting) return;
        
        isTeleporting = true;
        
        // Langsung teleport (tanpa fade)
        other.transform.position = destination.position;

        // Ganti camera zone jika ada
        if (destinationZone != null && CameraFollow.Instance != null)
        {
            CameraFollow.Instance.SetZone(destinationZone);
        }

        // Reset flag setelah delay kecil
        Invoke(nameof(ResetTeleport), 0.1f);
    }

    void ResetTeleport()
    {
        isTeleporting = false;
    }
}
