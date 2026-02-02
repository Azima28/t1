using UnityEngine;

public class DestructibleCircle : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Mengecek apakah yang menyentuh adalah Player
        if (other.CompareTag("Player") || other.GetComponent<PlayerMovement>() != null)
        {
            Break();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Juga mengecek tabrakan biasa jika tidak diset sebagai Trigger
        if (collision.gameObject.CompareTag("Player") || collision.gameObject.GetComponent<PlayerMovement>() != null)
        {
            Break();
        }
    }

    private void Break()
    {
        Debug.Log("Lingkaran pecah!");
        // Di sini bisa ditambahkan efek partikel atau suara sebelum hancur
        Destroy(gameObject);
    }
}
