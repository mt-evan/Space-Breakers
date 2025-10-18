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

    private bool pierceActive = false;
    private bool shieldActive = false;

    public bool IsPierceActive() { return pierceActive; }
    public bool IsShieldActive() { return shieldActive; }

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

    public void ActivatePowerUp(PowerUpController.PowerUpType type)
    {
        // Stop any existing coroutine of the same type to reset its timer if collected again
        StopCoroutine(type.ToString() + "PowerUpRoutine");
        StartCoroutine(type.ToString() + "PowerUpRoutine");
    }

    IEnumerator PiercePowerUpRoutine()
    {
        pierceActive = true;
        ballController.SetPierce(true);
        yield return new WaitForSeconds(15f); // Duration of the power-up
        pierceActive = false;
        ballController.SetPierce(false);
    }

    IEnumerator ShieldPowerUpRoutine()
    {
        shieldActive = true;
        yield return new WaitForSeconds(20f); // Duration of the power-up
        shieldActive = false;
    }

    IEnumerator FreezePowerUpRoutine()
    {
        if (currentSwarm != null)
        {
            currentSwarm.StopSwarm(); // Stop movement and firing
        }
        yield return new WaitForSeconds(15f); // Duration of the power-up
        if (currentSwarm != null)
        {
            currentSwarm.ResumeSwarm(); // Resume movement and firing
        }
    }
}

