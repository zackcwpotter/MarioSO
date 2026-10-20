using UnityEngine;
using TMPro;
using UnityEngine.UI;
using UnityEngine.Events; 

public class HUDManagerWeek5 : MonoBehaviour
{
    // Reordered and renamed to perfectly match your screenshot
    public GameConstants gameConstants;
    public GameObject scoreText;
    public GameObject highscoreText;
    public GameObject pauseButton;
    public GameObject backToMainButton; 
    public RectTransform restartButton;
    public GameObject gameOverPanel;
    public IntVariable gameScore;

    private RectTransform scoreRect;

    // top right corner buttons, right to left: replay, pause, home
    private const float buttonSize = 120f;
    private const float buttonSpacing = 10f;
    private static readonly Vector2 cornerMargin = new Vector2(40, 30);

    private GameObject mainMenuText;

    // UnityEvent for when the player clicks to return to the main menu
    public UnityEvent onReturnToMenu;

    void Awake()
    {
        scoreRect = scoreText.GetComponent<RectTransform>();

        if (pauseButton == null)
            pauseButton = FindChild("Pause Button");
        if (backToMainButton == null)
            backToMainButton = FindChild("Home Button");

        CreateMainMenuText();
    }

    void Start()
    {
        // Initialize HUD directly using the gameScore ScriptableObject
        GameStart();
        SetScore();
    }

    private void CreateMainMenuText()
    {
        TextMeshProUGUI scoreTMP = scoreText.GetComponent<TextMeshProUGUI>();

        mainMenuText = new GameObject("Main Menu Text", typeof(RectTransform));
        mainMenuText.transform.SetParent(transform, false);

        TextMeshProUGUI label = mainMenuText.AddComponent<TextMeshProUGUI>();
        label.text = "MAIN MENU";
        label.font = scoreTMP.font;
        label.fontSize = scoreTMP.fontSize;
        label.color = Color.white;
        label.alignment = TextAlignmentOptions.Center;
        label.textWrappingMode = TextWrappingModes.NoWrap;

        Button button = mainMenuText.AddComponent<Button>();
        button.targetGraphic = label;
        
        // Use a UnityEvent instead of calling GameManager
        button.onClick.AddListener(() => onReturnToMenu.Invoke());

        RectTransform rect = mainMenuText.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.sizeDelta = new Vector2(600, 80);
        rect.anchoredPosition = new Vector2(0, -310);

        mainMenuText.SetActive(false);
    }

    private GameObject FindChild(string childName)
    {
        foreach (Transform t in GetComponentsInChildren<Transform>(true))
        {
            if (t.name == childName)
                return t.gameObject;
        }
        return null;
    }

    private void PlaceInCorner(RectTransform rect, int slot)
    {
        if (rect == null)
            return;

        rect.localScale = Vector3.one;
        rect.anchorMin = new Vector2(1, 1);
        rect.anchorMax = new Vector2(1, 1);
        rect.pivot = new Vector2(1, 1);
        rect.sizeDelta = new Vector2(buttonSize, buttonSize);
        rect.anchoredPosition = new Vector2(
            -cornerMargin.x - slot * (buttonSize + buttonSpacing),
            -cornerMargin.y);
    }

    public void GameStart()
    {
        gameOverPanel.SetActive(false);
        highscoreText.SetActive(false);
        if (pauseButton != null)
            pauseButton.SetActive(true);

        scoreRect.anchorMin = new Vector2(0, 1);
        scoreRect.anchorMax = new Vector2(0, 1);
        scoreRect.pivot = new Vector2(0, 1);
        scoreRect.anchoredPosition = new Vector2(40, -30);

        PlaceInCorner(restartButton, 0);
        if (pauseButton != null)
            PlaceInCorner(pauseButton.GetComponent<RectTransform>(), 1);
        mainMenuText.SetActive(false);

        if (backToMainButton != null)
        {
            backToMainButton.SetActive(true);
            PlaceInCorner(backToMainButton.GetComponent<RectTransform>(), 2);
        }
    }

    // CHANGED: No longer asks for an int, reads directly from gameScore!
    public void SetScore()
    {
        scoreText.GetComponent<TextMeshProUGUI>().text =
            "Score: " + gameScore.Value.ToString();
    }

    public void GameOver()
    {
        gameOverPanel.SetActive(true);
        if (pauseButton != null)
            pauseButton.SetActive(false);

        if (backToMainButton != null)
            backToMainButton.SetActive(false);
        mainMenuText.transform.SetAsLastSibling(); 
        mainMenuText.SetActive(true);

        scoreRect.anchorMin = new Vector2(0.5f, 0.5f);
        scoreRect.anchorMax = new Vector2(0.5f, 0.5f);
        scoreRect.pivot = new Vector2(0.5f, 0.5f);
        scoreRect.anchoredPosition = new Vector2(0, -60);

        restartButton.anchorMin = new Vector2(0.5f, 0.5f);
        restartButton.anchorMax = new Vector2(0.5f, 0.5f);
        restartButton.pivot = new Vector2(0.5f, 0.5f);
        restartButton.anchoredPosition = new Vector2(0, -190);

        highscoreText.GetComponent<TextMeshProUGUI>().text =
            "High Score: " + gameScore.previousHighestValue.ToString("D3");

        highscoreText.SetActive(true);
    }
}