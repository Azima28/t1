using UnityEngine;

[RequireComponent(typeof(MeshRenderer))]
public class ParallaxMeshBackground : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    private Material material;
    public Transform cam;
    
    [Tooltip("Kecepatan pergerakan parallax. Semakin kecil nilainya, semakin terasa jauh posisinya.")]
    public float parallaxSpeed = 0.1f;

    [Header("Layering (Untuk Menaruhnya di Belakang Level)")]
    [Tooltip("Gunakan 'Default' atau buat Sorting Layer baru di Unity bernama 'Background'")]
    public string sortingLayerName = "Default";
    
    [Tooltip("Angka negatif akan merender background di belakang layer lantai/player.")]
    public int sortingOrder = -10;

    // Nama property untuk offset Texture (Beda antara Standard dan URP)
    private string texturePropName = "_MainTex";

    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        material = meshRenderer.material;
        
        // PENTING: Pindahkan urutan gambarnya ke belakang lantai 2D
        meshRenderer.sortingLayerName = sortingLayerName;
        meshRenderer.sortingOrder = sortingOrder;

        if (cam == null)
        {
            cam = Camera.main.transform;
        }

        // Cek jika shader ini menggunakan "_BaseMap" (Ciri khas URP) dan bukan "_MainTex"
        if (material.HasProperty("_BaseMap"))
        {
            texturePropName = "_BaseMap";
        }
    }

    void Update()
    {
        // Hitung pergeseran texture (UV Offset) berdasarkan posisi kamera
        float offset = cam.position.x * parallaxSpeed;
        
        // Terapkan ke material (hanya sumbu X) menggunakan nama properti yang sesuai
        material.SetTextureOffset(texturePropName, new Vector2(offset, 0));
        
        // Pastikan objek quad mengikuti kamera secara X 
        transform.position = new Vector3(cam.position.x, transform.position.y, transform.position.z);
    }
}
