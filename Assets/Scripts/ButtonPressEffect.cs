using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// Animasi tombol mengecil saat ditekan.
/// Mendukung geser antar tombol - animasi ikut berpindah.
/// </summary>
public class ButtonPressEffect : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
{
    [Header("Settings")]
    [Tooltip("Ukuran saat ditekan (0.9 = 90% dari ukuran asli)")]
    public float pressedScale = 0.85f;
    
    [Tooltip("Kecepatan animasi")]
    public float animationSpeed = 12f;

    private Vector3 originalScale;
    private Vector3 targetScale;
    
    // Track apakah ada sentuhan aktif
    private static bool isTouchingAny = false;

    void Start()
    {
        originalScale = transform.localScale;
        targetScale = originalScale;
    }

    void Update()
    {
        transform.localScale = Vector3.Lerp(transform.localScale, targetScale, animationSpeed * Time.deltaTime);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isTouchingAny = true;
        targetScale = originalScale * pressedScale;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isTouchingAny = false;
        targetScale = originalScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Jika user geser masuk sambil menyentuh
        if (isTouchingAny)
        {
            targetScale = originalScale * pressedScale;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Jika user geser keluar, kembalikan ukuran
        if (isTouchingAny)
        {
            targetScale = originalScale;
        }
    }
}
