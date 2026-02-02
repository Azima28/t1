using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

/// <summary>
/// Virtual Joystick untuk kontrol sentuh mobile.
/// Pasang script ini ke Image UI yang akan jadi background joystick.
/// PENTING: Pada Handle Image, MATIKAN "Raycast Target" di Inspector!
/// </summary>
public class VirtualJoystick : MonoBehaviour, IDragHandler, IPointerDownHandler, IPointerUpHandler
{
    [Header("Joystick Components")]
    [Tooltip("Image lingkaran kecil yang digerakkan (stick/handle)")]
    public RectTransform handle;
    
    [Header("Settings")]
    [Tooltip("Seberapa jauh handle bisa bergerak dari tengah (dalam pixel)")]
    public float handleRadius = 50f;

    private RectTransform background;
    private Vector2 inputVector = Vector2.zero;

    // Properti untuk dibaca PlayerMovement
    public float Horizontal => inputVector.x;
    public float Vertical => inputVector.y;
    public Vector2 Direction => inputVector;

    // Singleton agar mudah diakses
    public static VirtualJoystick Instance { get; private set; }

    void Awake()
    {
        Instance = this;
        background = GetComponent<RectTransform>();
        
        // Pastikan handle di tengah saat mulai
        if (handle != null)
        {
            handle.anchoredPosition = Vector2.zero;
            
            // Matikan raycast pada handle agar tidak menangkap klik
            Image handleImage = handle.GetComponent<Image>();
            if (handleImage != null)
            {
                handleImage.raycastTarget = false;
            }
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        OnDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Konversi posisi sentuh ke posisi lokal dalam background
        Vector2 localPoint;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            background,
            eventData.position,
            eventData.pressEventCamera,
            out localPoint
        );

        // Clamp ke radius maksimum
        if (localPoint.magnitude > handleRadius)
        {
            localPoint = localPoint.normalized * handleRadius;
        }

        // Normalisasi input (-1 sampai 1)
        inputVector = localPoint / handleRadius;

        // Gerakkan handle ke posisi sentuh (sudah di-clamp)
        handle.anchoredPosition = localPoint;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        // Reset ke tengah saat dilepas
        inputVector = Vector2.zero;
        handle.anchoredPosition = Vector2.zero;
    }
}
