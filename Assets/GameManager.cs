using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("UI References")]
    public Text scoreText;
    public Text levelText;
    public GameObject gameOverPanel;
    public Text finalScoreText;
    public Text highScoreText;
    public Text finalLevelText;
    public GameObject levelClearPanel;

    [Header("Game Object References")]
    public AlienSwarmController swarmPrefab;
    public PlayerController playerController;
    public BallController ballController;
    public ForceFieldEffect[] forceFieldWalls;

    private AlienSwarmController currentSwarm;
    private int currentLevel = 0;
    private int score = 0;
    private const string HighScoreKey = "HighScore";

    private bool pierceActive = false;
    public bool IsPierceActive() { return pierceActive; }

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
        UpdateGameUI();
        StartCoroutine(StartNextLevel());
    }

    public void AddScore(int points)
    {
        score += points;
        UpdateGameUI();
    }

    public void SubtractScore(int points)
    {
        score -= points;
        if (score < 0) score = 0;
        UpdateGameUI();
    }

    void UpdateGameUI()
    {
        if (scoreText != null) scoreText.text = "Score: " + score;
        if (levelText != null) levelText.text = "Level: " + currentLevel;
    }

    public void WaveCleared()
    {
        StartCoroutine(StartNextLevel());
    }

    IEnumerator StartNextLevel()
    {
        SoundManager.instance.PlayLevelClear();

        if (currentLevel > 0)
        {
            if (scoreText != null) scoreText.gameObject.SetActive(false);
            if (levelText != null) levelText.gameObject.SetActive(false);
            levelClearPanel.SetActive(true);
            yield return new WaitForSeconds(2.0f);
            levelClearPanel.SetActive(false);
        }

        currentLevel++;
        if (scoreText != null) scoreText.gameObject.SetActive(true);
        if (levelText != null) levelText.gameObject.SetActive(true);
        UpdateGameUI();

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
        SoundManager.instance.PlayGameOver();

        if (currentSwarm != null)
        {
            currentSwarm.StopSwarm();
        }

        Time.timeScale = 0f;
        if (scoreText != null) scoreText.gameObject.SetActive(false);
        if (levelText != null) levelText.gameObject.SetActive(false);

        int highScore = PlayerPrefs.GetInt(HighScoreKey, 0);
        if (score > highScore)
        {
            highScore = score;
            PlayerPrefs.SetInt(HighScoreKey, highScore);
            PlayerPrefs.Save();
        }
        finalScoreText.text = "Your Score: " + score;
        highScoreText.text = "High Score: " + highScore;
        finalLevelText.text = "Level Reached: " + currentLevel;
        gameOverPanel.SetActive(true);
    }

    public void RestartGame()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ActivatePowerUp(PowerUpController.PowerUpType type)
    {
        SoundManager.instance.PlayPowerUp();

        // Stop any existing coroutine of the same type to reset its timer if collected again
        StopCoroutine(type.ToString() + "PowerUpRoutine");
        StartCoroutine(type.ToString() + "PowerUpRoutine");
    }

    public bool IsShieldActive()
    {
        if (playerController != null)
        {
            return playerController.IsShieldActive();
        }
        return false;
    }

    IEnumerator PiercePowerUpRoutine()
    {
        pierceActive = true;

        if (ballController != null)
        {
            ballController.SetAppearance(true);
        }

        int ballLayer = LayerMask.NameToLayer("Ball");
        int alienLayer = LayerMask.NameToLayer("Aliens");

        Physics2D.IgnoreLayerCollision(ballLayer, alienLayer, true);

        yield return new WaitForSeconds(15f);

        Physics2D.IgnoreLayerCollision(ballLayer, alienLayer, false);

        if (ballController != null)
        {
            ballController.SetAppearance(false);
        }

        pierceActive = false;
    }

    IEnumerator ShieldPowerUpRoutine()
    {
        if (playerController != null)
        {
            playerController.ActivateShield(20f, Color.blue, Color.red);
        }
        yield return null;
    }

    IEnumerator FreezePowerUpRoutine()
    {
        if (currentSwarm != null)
        {
            currentSwarm.StopSwarm(); // Stop movement and firing
        }

        foreach (ForceFieldEffect wall in forceFieldWalls)
        {
            if (wall != null) wall.SetTemporaryColor(Color.cyan);
        }

        yield return new WaitForSeconds(15f); // Duration of the power-up
        
        foreach (ForceFieldEffect wall in forceFieldWalls)
        {
            if (wall != null) wall.RevertToOriginalColor();
        }
        
        if (currentSwarm != null)
        {
            currentSwarm.ResumeSwarm(); // Resume movement and firing
        }
    }
}

