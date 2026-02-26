using UnityEngine;

/// <summary>
/// Menghancurkan GameObject ini setelah waktu yang ditentukan (delay).
/// Cocok untuk efek partikel, debu, atau gambar sekali pakai.
/// </summary>
public class DestroyAfterTime : MonoBehaviour
{
    [Tooltip("Waktu tunggu sebelum objek ini dihancurkan (detik)")]
    public float destroyDelay = 0.5f;

    void Start()
    {
        // Langsung hancurkan objek ini (gameObject) setelah destroyDelay detik
        Destroy(gameObject, destroyDelay);
    }
}
