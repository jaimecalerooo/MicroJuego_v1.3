using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Scoring Display")]
    public Text scoreText;

    [Header("Pause UI Elements (Iteration 2)")]
    public GameObject pausePanel;
    public Button pauseButton;
    public Button resumeButton;
    public Button restartButton;
    public Button quitButton;

    [Header("Game Over UI Elements")]
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Button gameOverRestartButton;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void Start()
    {
        if (scoreText == null)
        {
            GameObject uiGo = GameObject.FindGameObjectWithTag("UI");
            if (uiGo != null)
            {
                scoreText = uiGo.GetComponent<Text>();
            }
        }

        UpdateScore(Player.SCORE);

        // Ocultar menús modales al inicio
        if (pausePanel != null) pausePanel.SetActive(false);
        if (gameOverPanel != null) gameOverPanel.SetActive(false);

        // Asignación de listeners a los botones
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);

        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (restartButton != null)
            restartButton.onClick.AddListener(OnRestartClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (gameOverRestartButton != null)
            gameOverRestartButton.onClick.AddListener(OnRestartClicked);
    }

    public void UpdateScore(int score)
    {
        if (scoreText != null)
        {
            scoreText.text = "Score: " + score;
        }
    }

    public void ShowPauseMenu(bool show)
    {
        if (pausePanel != null)
        {
            pausePanel.SetActive(show);
        }
    }

    public void ShowGameOverMenu(bool show)
    {
        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(show);
            if (finalScoreText != null)
            {
                finalScoreText.text = "Final Score: " + Player.SCORE;
            }
        }
    }

    // Callbacks de botones
    public void OnPauseClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.PauseGame();
    }

    public void OnResumeClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.ResumeGame();
    }

    public void OnRestartClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.RestartGame();
    }

    public void OnQuitClicked()
    {
        if (GameManager.Instance != null)
            GameManager.Instance.QuitGame();
    }
}
