using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [Header("Target")]
    public Transform player;

    [Header("Settings")]
    [Range(0.01f, 1f)]
    public float smoothTime = 0.15f;  // Waktu untuk mencapai target (lebih kecil = lebih responsif)
    public Vector3 offset = new Vector3(0, 2, -10);
    
    [Tooltip("Jika dicentang, kamera TIDAK ikut naik turun saat loncat")]
    public bool lockVertical = false;

    [Header("Camera Zone (Otomatis dari CameraZone)")]
    [Tooltip("Zone aktif saat ini - bisa diubah oleh Teleporter")]
    public CameraZone currentZone;

    private Camera cam;
    private float camHalfHeight;
    private float camHalfWidth;
    private Vector3 velocity = Vector3.zero;  // Untuk SmoothDamp

    // Singleton agar mudah diakses dari Teleporter
    public static CameraFollow Instance { get; private set; }

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        cam = GetComponent<Camera>();
        if (cam != null)
        {
            camHalfHeight = cam.orthographicSize;
            camHalfWidth = camHalfHeight * cam.aspect;
        }

        // Cari zone pertama jika belum di-assign
        if (currentZone == null)
        {
            currentZone = FindObjectOfType<CameraZone>();
        }
    }

    void LateUpdate()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null)
                player = playerObj.transform;
            return;
        }

        // Hitung target position
        Vector3 targetPosition = player.position + offset;

        // Lock Y jika diaktifkan
        if (lockVertical)
        {
            targetPosition.y = transform.position.y;
        }

        // Clamp target ke batas zone DULU
        if (currentZone != null && currentZone.boundBottomLeft != null && currentZone.boundTopRight != null)
        {
            float minX = currentZone.boundBottomLeft.position.x + camHalfWidth;
            float maxX = currentZone.boundTopRight.position.x - camHalfWidth;
            float minY = currentZone.boundBottomLeft.position.y + camHalfHeight;
            float maxY = currentZone.boundTopRight.position.y - camHalfHeight;

            targetPosition.x = Mathf.Clamp(targetPosition.x, minX, maxX);
            targetPosition.y = Mathf.Clamp(targetPosition.y, minY, maxY);
        }

        // SmoothDamp untuk pergerakan ultra-halus tanpa blink
        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position, 
            targetPosition, 
            ref velocity, 
            smoothTime
        );

        // Pastikan Z tetap (untuk 2D)
        smoothedPosition.z = offset.z;

        transform.position = smoothedPosition;
    }

    /// <summary>
    /// Panggil fungsi ini untuk ganti zone kamera (dari Teleporter)
    /// </summary>
    public void SetZone(CameraZone newZone)
    {
        if (newZone != null)
        {
            currentZone = newZone;
            velocity = Vector3.zero;  // Reset velocity saat ganti zone
            Debug.Log("Camera zone changed to: " + newZone.zoneName);
        }
    }

    // Gambar batas zone aktif di Scene View
    void OnDrawGizmosSelected()
    {
        if (currentZone == null || currentZone.boundBottomLeft == null || currentZone.boundTopRight == null) return;

        Gizmos.color = Color.yellow;
        
        Vector3 bottomLeft = currentZone.boundBottomLeft.position;
        Vector3 topRight = currentZone.boundTopRight.position;
        Vector3 bottomRight = new Vector3(topRight.x, bottomLeft.y, 0);
        Vector3 topLeft = new Vector3(bottomLeft.x, topRight.y, 0);

        Gizmos.DrawLine(bottomLeft, bottomRight);
        Gizmos.DrawLine(bottomRight, topRight);
        Gizmos.DrawLine(topRight, topLeft);
        Gizmos.DrawLine(topLeft, bottomLeft);
    }
}
