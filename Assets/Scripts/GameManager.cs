using Utils;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    [Header("Game State")]
    public bool isPaused = false;
    public bool isGameOver = false;

    [Header("Player Stats")]
    public int score = 0;
    public int lives = 3;
    public int level = 1;

    [Header("Settings")]
    public float gameSpeed = 1f;

    public delegate void OnScoreChanged(int newScore);
    public event OnScoreChanged onScoreChanged;
    public delegate void OnGameOver();
    public event OnGameOver onGameOver;

    protected override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
    }

    private void Start()
    {
        InitializeGame();
    }

    public void InitializeGame()
    {
        score = 0;
        lives = 3;
        level = 1;
        isPaused = false;
        isGameOver = false;
        gameSpeed = 1f;
        Time.timeScale = gameSpeed;
    }

    public void AddScore(int points)
    {
        score += points;
        onScoreChanged?.Invoke(score);
        Debug.Log($"Score: {score}");
    }

    public void LoseLife()
    {
        lives--;
        if (lives <= 0) GameOver();
        else Debug.Log($"Lives remaining: {lives}");
    }

    public void AddLife()
    {
        lives++;
        Debug.Log($"Lives: {lives}");
    }

    public void PauseGame()
    {
        isPaused = true;
        Time.timeScale = 0f;
        Debug.Log("Game Paused");
    }

    public void ResumeGame()
    {
        isPaused = false;
        Time.timeScale = gameSpeed;
        Debug.Log("Game Resumed");
    }

    public void TogglePause()
    {
        if (isPaused) ResumeGame();
        else PauseGame();
    }

    public void GameOver()
    {
        isGameOver = true;
        Time.timeScale = 0f;
        onGameOver?.Invoke();
        Debug.Log("Game Over!");
    }

    public void RestartGame()
    {
        Time.timeScale = gameSpeed;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        InitializeGame();
    }

    public void LoadNextLevel()
    {
        level++;
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (nextSceneIndex < SceneManager.sceneCountInBuildSettings) SceneManager.LoadScene(nextSceneIndex);
        else Debug.Log("No more levels!");
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = gameSpeed;
        SceneManager.LoadScene(sceneName);
    }

    public void QuitGame()
    {
        Debug.Log("Quitting game...");
        Application.Quit();
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #endif
    }
}
