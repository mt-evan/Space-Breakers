using UnityEngine;
using UnityEngine.UI; // <-- This line is essential for UI code
using UnityEngine.SceneManagement; // <-- This line is needed for restarting the scene

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    public Text scoreText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text highScoreText;

    private int score = 0;
    private const string HighScoreKey = "HighScore";

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        gameOverPanel.SetActive(false);
        UpdateScoreUI();
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        scoreText.text = "Score: " + score;
    }

    public void GameOver()
    {
        Time.timeScale = 0f;
        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }
        finalScoreText.text = "Your Score: " + score;
        highScoreText.text = "High Score: " + highScore;
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
}

