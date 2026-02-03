using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class LevelPagingController : MonoBehaviour
{
    [Header("Paging Settings")]
    [SerializeField] int maxPage = 10;
    int currentPage = 1;
    Vector3 startLocalPos;
    [SerializeField] Vector3 pageStep;
    [SerializeField] RectTransform levelPageRect;

    [SerializeField] float tweenTime = 0.5f;
    [SerializeField] LeanTweenType tweenType = LeanTweenType.easeOutBack;

    [Header("Button Settings")]
    [SerializeField] private Button nextButton;
    [SerializeField] private Button prevButton;

    [Header("Level Label")]
    [SerializeField] private TextMeshProUGUI pageLabel;
    [SerializeField] private string labelPrefix = "Level ";

    [Header("Final Level UI")]
    [SerializeField] private TextMeshProUGUI finalLevelText;

    [Header("Swipe Settings")]
    [SerializeField] private float minSwipeDistance = 50f;
    [SerializeField] private float maxSwipeTime = 1f;

    [Header("Dot Indicator")]
    [SerializeField] private Image[] dots; // ISI 3 DOT SAJA
    [SerializeField] private Color activeDotColor = Color.white;
    [SerializeField] private Color inactiveDotColor = Color.gray;

    private Vector2 startPoint;
    private float startTime;

    private void Reset()
    {
        // Otomatis cari RectTransform yang mungkin adalah kontainer level
        if (levelPageRect == null)
        {
            levelPageRect = GetComponentInChildren<RectTransform>();
            if (levelPageRect == gameObject.GetComponent<RectTransform>())
            {
                // Jangan pakai diri sendiri kalau ada child lain
                var children = GetComponentsInChildren<RectTransform>();
                if (children.Length > 1) levelPageRect = children[1];
            }
        }
    }

    private void Awake()
    {
        if (levelPageRect == null)
        {
            // Coba cari sekali lagi saat runtime
            levelPageRect = GetComponentInChildren<RectTransform>();
            
            if (levelPageRect == null || levelPageRect == gameObject.GetComponent<RectTransform>())
            {
                 Debug.LogError($"[LevelPagingController] levelPageRect belum di assign di GameObject: {gameObject.name}!\n" +
                                "Silahkan drag UI Panel/Object yang berisi kumpulan level ke slot 'Level Page Rect' di Inspector.");
                 return;
            }
        }

        startLocalPos = levelPageRect.localPosition;
        UpdateUI();
    }

    private void Update()
    {
        if (levelPageRect == null) return;
        if (LeanTween.isTweening(levelPageRect)) return;
        HandleSwipeInput();
    }

    #region Swipe
    private void HandleSwipeInput()
    {
#if UNITY_EDITOR
        if (Input.GetMouseButtonDown(0))
        {
            startPoint = Input.mousePosition;
            startTime = Time.time;
        }
        else if (Input.GetMouseButtonUp(0))
        {
            DetectSwipe(Input.mousePosition, Time.time);
        }
#else
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            if (touch.phase == TouchPhase.Began)
            {
                startPoint = touch.position;
                startTime = Time.time;
            }
            else if (touch.phase == TouchPhase.Ended)
            {
                DetectSwipe(touch.position, Time.time);
            }
        }
#endif
    }

    private void DetectSwipe(Vector2 endPoint, float endTime)
    {
        float distance = Vector2.Distance(startPoint, endPoint);
        float time = endTime - startTime;

        if (distance > minSwipeDistance && time < maxSwipeTime)
        {
            if (endPoint.x > startPoint.x)
                Previous();
            else
                Next();
        }
    }
    #endregion

    public void Next()
    {
        if (currentPage >= maxPage) return;
        currentPage++;
        MovePage();
        UpdateUI();
    }

    public void Previous()
    {
        if (currentPage <= 1) return;
        currentPage--;
        MovePage();
        UpdateUI();
    }

    private void MovePage()
    {
        if (levelPageRect == null) return;
        Vector3 targetPos = startLocalPos + pageStep * (currentPage - 1);
        levelPageRect.LeanMoveLocal(targetPos, tweenTime).setEase(tweenType);
    }

    private void UpdateUI()
    {
        UpdateButtons();
        UpdateLabel();
        UpdateDots();
        UpdateFinalLevelText();
    }

    private void UpdateButtons()
    {
        if (nextButton != null)
            nextButton.gameObject.SetActive(currentPage < maxPage);

        if (prevButton != null)
            prevButton.gameObject.SetActive(currentPage > 1);
    }

    private void UpdateLabel()
    {
        if (pageLabel != null)
            pageLabel.text = labelPrefix + currentPage;
    }

    // =========================
    // DOT LOGIC
    // =========================
    private void UpdateDots()
    {
        if (dots == null || dots.Length < 3) return;

        // KHUSUS LEVEL TERAKHIR (Misal: Level 10)
        if (currentPage == maxPage)
        {
            // Sembunyikan dot kiri (0) dan kanan (2)
            dots[0].gameObject.SetActive(false);
            dots[2].gameObject.SetActive(false);

            // Tampilkan dot tengah (1) saja sebagai aktif
            dots[1].gameObject.SetActive(true);
            dots[1].color = activeDotColor;
            
            return;
        }

        // NORMAL: Tampilkan semua dot dan muter 1-2-3
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].gameObject.SetActive(true);
        }

        int activeIndex = (currentPage - 1) % dots.Length;
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].color = (i == activeIndex) ? activeDotColor : inactiveDotColor;
        }
    }

    // =========================
    // FINAL LEVEL TEXT
    // =========================
    private void UpdateFinalLevelText()
    {
        if (finalLevelText == null) return;
        finalLevelText.gameObject.SetActive(currentPage == maxPage);
    }
}
