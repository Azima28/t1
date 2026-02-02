using UnityEngine;

/// <summary>
/// Taruh script ini di sebuah objek untuk menentukan satu zona kamera.
/// Anda bisa menambah zona sebanyak yang Anda mau.
/// </summary>
public class CameraZone : MonoBehaviour
{
    [Header("Batas Zone Ini")]
    [Tooltip("Seret objek di POJOK KIRI BAWAH zona ini")]
    public Transform boundBottomLeft;
    
    [Tooltip("Seret objek di POJOK KANAN ATAS zona ini")]
    public Transform boundTopRight;

    [Header("Info (Opsional)")]
    public string zoneName = "Zone 1";

    // Gambar batas di Scene View
    void OnDrawGizmos()
    {
        if (boundBottomLeft == null || boundTopRight == null) return;

        Gizmos.color = Color.cyan;
        
        Vector3 bottomLeft = boundBottomLeft.position;
        Vector3 topRight = boundTopRight.position;
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, 0);
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);

        // Label
#if UNITY_EDITOR
        UnityEditor.Handles.Label(
            new Vector3((bottomLeft.x + topRight.x) / 2, topRight.y + 0.5f, 0),
            zoneName
        );
#endif
    }
}
