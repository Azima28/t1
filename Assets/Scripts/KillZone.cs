using UnityEngine;

/// <summary>
/// Kill Zone - area yang langsung membunuh player saat disentuh.
/// Untuk jurang/lubang/area berbahaya.
/// </summary>
[RequireComponent(typeof(Collider2D))]
public class KillZone : MonoBehaviour
{
    void Start()
    {
        // Pastikan collider adalah trigger
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.isTrigger = true;
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("[KillZone] Player jatuh!");
            
            PlayerDeath death = other.GetComponent<PlayerDeath>();
            if (death != null)
            {
                death.Die();
            }
        }
    }
}
