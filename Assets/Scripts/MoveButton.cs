using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Tombol gerak kiri/kanan untuk mobile.
/// Mendukung geser antar tombol tanpa melepas sentuhan.
/// </summary>
public class MoveButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Arah Tombol")]
    [Tooltip("-1 untuk Kiri, 1 untuk Kanan")]
    public float direction = 0f;

    // Nilai input yang dibaca PlayerMovement
    private static float currentInput = 0f;
    public static float Input => currentInput;
    
    // Track tombol mana yang aktif
    private static MoveButton activeButton = null;
    
    // Track pointer ID untuk menghindari konflik multi-touch
    private static int activePointerId = -1;

    public void OnPointerDown(PointerEventData eventData)
    {
        activeButton = this;
        activePointerId = eventData.pointerId;
        currentInput = direction;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Hanya reset jika ini pointer yang sama
        if (eventData.pointerId == activePointerId)
        {
            activeButton = null;
            activePointerId = -1;
            currentInput = 0f;
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Jika ada sentuhan aktif dan pointer ID sama
        if (activeButton != null && eventData.pointerId == activePointerId)
        {
            activeButton = this;
            currentInput = direction;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Tidak perlu melakukan apa-apa di sini
        // OnPointerEnter di tombol lain yang akan handle
    }
}
