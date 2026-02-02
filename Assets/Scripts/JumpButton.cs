using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Tombol Jump untuk mobile. Pasang di UI Button.
/// </summary>
public class JumpButton : MonoBehaviour, IPointerDownHandler
{
    // Flag yang dibaca PlayerMovement
    public static bool IsPressed { get; private set; }

    public void OnPointerDown(PointerEventData eventData)
    {
        IsPressed = true;
    }

    void LateUpdate()
    {
        // Reset setiap frame agar hanya terbaca sekali
        IsPressed = false;
    }
}
