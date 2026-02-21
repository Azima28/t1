using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;

/// <summary>
/// Singleton yang mengatur UI Pertanyaan Matematika secara dinamis.
/// Tidak perlu membuat Canvas manual, script yang akan buat (Auto-Generate).
/// </summary>
public class MathTrapManager : MonoBehaviour
{
    public static MathTrapManager Instance { get; private set; }

    [Header("UI Settings")]
    public float timeoutSeconds = 10f; // Waktu untuk menjawab (jika tidak mau pakai batas waktu, set angka besar)

    private GameObject canvasObj;
    private GameObject panelObj;
    private Text questionText;
    private InputField answerInput;
    private Text timerText;
    
    // Status
    private bool isQuestionActive = false;
    private int expectedAnswer;
    private MathCoin currentCoin;
    private PlayerDeath currentPlayer;
    private float timeRemaining;

    void Awake()
    {
        // Singleton pattern
        if (Instance == null)
        {
            Instance = this;
            // Pindahkan root objectnya ke Don't Destroy kalo dirasa perlu, 
            // tapi karena ini di tiap level mending ngga usah.
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        // Auto-Generate UI saat Awake agar tidak pusing pasang manual
        GenerateUI();
    }

    /// <summary>
    /// Membuat UI Canvas dan kawan-kawannya lewat script
    /// </summary>
    void GenerateUI()
    {
        // Cek EventSystem agar InputField bisa diklik
        if (FindObjectOfType<EventSystem>() == null)
        {
            GameObject eventSystem = new GameObject("EventSystem");
            eventSystem.AddComponent<EventSystem>();
            eventSystem.AddComponent<StandaloneInputModule>();
        }

        // 1. Buat Canvas
        canvasObj = new GameObject("MathTrapCanvas");
        Canvas canvas = canvasObj.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 999; // Paling depan
        CanvasScaler scaler = canvasObj.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920, 1080);
        canvasObj.AddComponent<GraphicRaycaster>();
        canvasObj.transform.SetParent(this.transform); // Masukkan di bawah Manager

        // 2. Buat Panel Gelap (Background)
        panelObj = new GameObject("DarkPanel");
        panelObj.transform.SetParent(canvasObj.transform, false);
        Image panelImg = panelObj.AddComponent<Image>();
        panelImg.color = new Color(0, 0, 0, 0.85f); // Hitam transparan
        RectTransform panelRect = panelObj.GetComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        // 3. Buat Container Kotak
        GameObject boxObj = new GameObject("QuestionBox");
        boxObj.transform.SetParent(panelObj.transform, false);
        Image boxImg = boxObj.AddComponent<Image>();
        boxImg.color = new Color(0.2f, 0.2f, 0.2f, 1f); // Abu-abu gelap
        RectTransform boxRect = boxObj.GetComponent<RectTransform>();
        boxRect.anchorMin = new Vector2(0.5f, 0.5f);
        boxRect.anchorMax = new Vector2(0.5f, 0.5f);
        boxRect.sizeDelta = new Vector2(600, 400);

        // 4. Bikin Teks Judul/Peringatan
        GameObject titleObj = new GameObject("TitleText");
        titleObj.transform.SetParent(boxObj.transform, false);
        Text titleText = titleObj.AddComponent<Text>();
        titleText.text = "JAWAB CEPAT!";
        titleText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        titleText.fontSize = 40;
        titleText.color = Color.red;
        titleText.alignment = TextAnchor.MiddleCenter;
        RectTransform titleRect = titleObj.GetComponent<RectTransform>();
        titleRect.anchorMin = new Vector2(0.5f, 1f);
        titleRect.anchorMax = new Vector2(0.5f, 1f);
        titleRect.anchoredPosition = new Vector2(0, -50);
        titleRect.sizeDelta = new Vector2(500, 60);

        // 5. Teks Timer
        GameObject timerObj = new GameObject("TimerText");
        timerObj.transform.SetParent(boxObj.transform, false);
        timerText = timerObj.AddComponent<Text>();
        timerText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        timerText.fontSize = 30;
        timerText.color = Color.yellow;
        timerText.alignment = TextAnchor.MiddleCenter;
        RectTransform timerRect = timerObj.GetComponent<RectTransform>();
        timerRect.anchorMin = new Vector2(0.5f, 1f);
        timerRect.anchorMax = new Vector2(0.5f, 1f);
        timerRect.anchoredPosition = new Vector2(0, -100);
        timerRect.sizeDelta = new Vector2(500, 40);

        // 6. Teks Soal
        GameObject questionObj = new GameObject("QuestionText");
        questionObj.transform.SetParent(boxObj.transform, false);
        questionText = questionObj.AddComponent<Text>();
        questionText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        questionText.fontSize = 60;
        questionText.color = Color.white;
        questionText.alignment = TextAnchor.MiddleCenter;
        RectTransform qRect = questionObj.GetComponent<RectTransform>();
        qRect.anchoredPosition = new Vector2(0, 30);
        qRect.sizeDelta = new Vector2(500, 100);

        // 7. Input Field (Tempat Jawaban)
        GameObject inputObj = new GameObject("AnswerInput");
        inputObj.transform.SetParent(boxObj.transform, false);
        Image inputBg = inputObj.AddComponent<Image>();
        inputBg.color = Color.white;
        answerInput = inputObj.AddComponent<InputField>();
        
        GameObject placeholderObj = new GameObject("Placeholder");
        placeholderObj.transform.SetParent(inputObj.transform, false);
        Text placeholderText = placeholderObj.AddComponent<Text>();
        placeholderText.text = "Jawaban...";
        placeholderText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        placeholderText.fontSize = 40;
        placeholderText.color = new Color(0.2f, 0.2f, 0.2f, 0.5f);
        placeholderText.alignment = TextAnchor.MiddleCenter;
        RectTransform pRect = placeholderText.GetComponent<RectTransform>();
        pRect.anchorMin = Vector2.zero; pRect.anchorMax = Vector2.one;
        pRect.offsetMin = Vector2.zero; pRect.offsetMax = Vector2.zero;

        GameObject textObj = new GameObject("Text");
        textObj.transform.SetParent(inputObj.transform, false);
        Text inputText = textObj.AddComponent<Text>();
        inputText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        inputText.fontSize = 40;
        inputText.color = Color.black;
        inputText.alignment = TextAnchor.MiddleCenter;
        RectTransform tRect = inputText.GetComponent<RectTransform>();
        tRect.anchorMin = Vector2.zero; tRect.anchorMax = Vector2.one;
        tRect.offsetMin = Vector2.zero; tRect.offsetMax = Vector2.zero;

        answerInput.textComponent = inputText;
        answerInput.placeholder = placeholderText;
        answerInput.contentType = InputField.ContentType.IntegerNumber; // Hanya bisa angka

        RectTransform inputRect = inputObj.GetComponent<RectTransform>();
        inputRect.anchoredPosition = new Vector2(0, -60);
        inputRect.sizeDelta = new Vector2(300, 70);

        // 8. Submit Button
        GameObject btnObj = new GameObject("SubmitBtn");
        btnObj.transform.SetParent(boxObj.transform, false);
        Image btnImg = btnObj.AddComponent<Image>();
        btnImg.color = new Color(0.1f, 0.6f, 0.1f);
        Button submitBtn = btnObj.AddComponent<Button>();
        
        GameObject btnTextObj = new GameObject("BtnText");
        btnTextObj.transform.SetParent(btnObj.transform, false);
        Text btnText = btnTextObj.AddComponent<Text>();
        btnText.text = "JAWAB";
        btnText.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        btnText.fontSize = 30;
        btnText.color = Color.white;
        btnText.alignment = TextAnchor.MiddleCenter;
        RectTransform btnTextRect = btnTextObj.GetComponent<RectTransform>();
        btnTextRect.anchorMin = Vector2.zero; btnTextRect.anchorMax = Vector2.one;
        btnTextRect.offsetMin = Vector2.zero; btnTextRect.offsetMax = Vector2.zero;

        RectTransform btnRect = btnObj.GetComponent<RectTransform>();
        btnRect.anchoredPosition = new Vector2(0, -150);
        btnRect.sizeDelta = new Vector2(200, 60);

        // Tambah Listener Tombol
        submitBtn.onClick.AddListener(CheckAnswer);

        // Sembunyikan Panel Awal
        panelObj.SetActive(false);
    }

    /// <summary>
    /// Menampilkan UI Soal MTK. Dipanggil oleh MathCoin.
    /// </summary>
    public void ShowQuestion(MathCoin coin, PlayerDeath player)
    {
        if (isQuestionActive) return;

        currentCoin = coin;
        currentPlayer = player;
        
        // Bikin soal random
        int a = Random.Range(10, 50);
        int b = Random.Range(10, 50);
        
        // Operasi (+ atau -)
        bool isAddition = Random.value > 0.5f;
        if (isAddition)
        {
            expectedAnswer = a + b;
            questionText.text = a + " + " + b + " = ?";
        }
        else
        {
            expectedAnswer = a - b;
            questionText.text = a + " - " + b + " = ?";
        }

        answerInput.text = ""; // Reset input
        timeRemaining = timeoutSeconds;
        isQuestionActive = true;
        panelObj.SetActive(true);

        // Pause Game
        Time.timeScale = 0f;

        // Fokus otomatis ke input field (supaya ga perlu klik lagi, tapi untuk HP tetep harus disesuaikan)
        answerInput.Select();
        answerInput.ActivateInputField();

        StartCoroutine(TimerRoutine());
    }

    IEnumerator TimerRoutine()
    {
        while (isQuestionActive && timeRemaining > 0)
        {
            // Karena timeScale 0, gunakan unscaledDeltaTime
            timeRemaining -= Time.unscaledDeltaTime; 
            timerText.text = "Waktu: " + Mathf.Ceil(timeRemaining) + "s";
            yield return null;
        }

        if (isQuestionActive && timeRemaining <= 0)
        {
            // Timeout -> Mati
            FailTrap();
        }
    }

    /// <summary>
    /// Dipanggil saat tekan Jawab
    /// </summary>
    public void CheckAnswer()
    {
        if (!isQuestionActive) return;

        int playerAnswer;
        if (int.TryParse(answerInput.text, out playerAnswer))
        {
            if (playerAnswer == expectedAnswer)
            {
                // Benar!
                SucceedTrap();
            }
            else
            {
                // Salah!
                FailTrap();
            }
        }
        else
        {
            // Input kosong / ngasal -> Salah!
            FailTrap();
        }
    }

    /// <summary>
    /// Pemain Benar - Resume game, Hancurkan koin.
    /// </summary>
    void SucceedTrap()
    {
        CloseUI();
        if (currentCoin != null)
        {
            currentCoin.Resolve(true);
        }
    }

    /// <summary>
    /// Pemain Salah / Waktu Habis - Resume game, Bunuh player.
    /// </summary>
    void FailTrap()
    {
        CloseUI();
        if (currentCoin != null)
        {
            currentCoin.Resolve(false);
        }

        if (currentPlayer != null)
        {
            currentPlayer.Die(); // Kena mental matik!
        }
    }

    void CloseUI()
    {
        isQuestionActive = false;
        panelObj.SetActive(false);
        Time.timeScale = 1f; // Resume
        StopAllCoroutines();
    }
    
    /// <summary>
    /// Tutup paksa UI saat player mati dari sumber lain (misal Rolling Ball)
    /// Dipanggil dari luar (misal RollingBall.CheckPlayerHitManual)
    /// </summary>
    public void ForceClose()
    {
        if (isQuestionActive)
        {
            CloseUI();
        }
    }
}
