using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public bool IsPaused { get; private set; } = false;
    public bool IsGameOver { get; private set; } = false;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            Time.timeScale = 1f;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) || Input.GetButtonDown("Cancel"))
        {
            if (!IsGameOver)
            {
                TogglePause();
            }
        }
    }

    public void TogglePause()
    {
        if (IsPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    public void PauseGame()
    {
        IsPaused = true;
        Time.timeScale = 0f; // La escena se congela

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPauseMenu(true);
        }
    }

    public void ResumeGame()
    {
        IsPaused = false;
        Time.timeScale = 1f; // La escena reanuda su ejecución

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowPauseMenu(false);
        }
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        Player.SCORE = 0;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void GameOver()
    {
        IsGameOver = true;
        Time.timeScale = 0f;

        if (UIManager.Instance != null)
        {
            UIManager.Instance.ShowGameOverMenu(true);
        }
        else
        {
            RestartGame();
        }
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
