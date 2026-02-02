using UnityEngine;

/// <summary>
/// Background mengikuti kamera (tetap di belakang).
/// Pasang di object Background.
/// </summary>
public class BackgroundFollow : MonoBehaviour
{
    [Header("Offset dari Kamera")]
    public Vector3 offset = new Vector3(0, 0, 10); // Z positif = di belakang kamera
    
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void LateUpdate()
    {
        if (cameraTransform != null)
        {
            // Ikuti posisi kamera + offset
            Vector3 newPos = cameraTransform.position + offset;
            transform.position = newPos;
        }
    }
}
