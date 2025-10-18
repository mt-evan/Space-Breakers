using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public Text scoreText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text highScoreText;
    public GameObject levelClearPanel;

    [Header("Game Object References")]
    public AlienSwarmController swarmPrefab;
    public PlayerController playerController;
    public BallController ballController;

    private AlienSwarmController currentSwarm;
    private int currentLevel = 0;
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
        levelClearPanel.SetActive(false);
        if (scoreText != null) scoreText.gameObject.SetActive(true);
        UpdateScoreUI();
        StartCoroutine(StartNextLevel());
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateScoreUI();
    }

    void UpdateScoreUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
    }

    public void WaveCleared()
    {
        StartCoroutine(StartNextLevel());
    }

    IEnumerator StartNextLevel()
    {
        if (currentLevel > 0)
        {
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            levelClearPanel.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            levelClearPanel.SetActive(false);
        }

        currentLevel++;
        if (scoreText != null) scoreText.gameObject.SetActive(true);

        // Reset player and ball position for new level
        if (playerController != null)
        {
            playerController.ResetPlayerPosition();
        }
        if (ballController != null)
        {
            ballController.ResetBallToPlayer();
        }

        currentSwarm = Instantiate(swarmPrefab, Vector3.zero, Quaternion.identity);
        currentSwarm.InitializeLevel(currentLevel);
    }

    public void GameOver()
    {
        if (currentSwarm != null)
        {
            currentSwarm.StopSwarm();
        }

        Time.timeScale = 0f;
        if (scoreText != null)
        {
            scoreText.gameObject.SetActive(false);
        }

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

