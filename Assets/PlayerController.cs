using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    [Header("Movement")]
    public float moveSpeed = 10f;
    public float leftBoundary = -16.5f;
    public float rightBoundary = 16.5f;
    public BallController ballController;
    public GameObject shieldPrefab;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isAlive = true;

    // shield state management
    private bool isShielded = false;
    private GameObject shieldInstance;
    private Coroutine shieldCoroutine;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isAlive)
        {
            moveInput = Input.GetAxis("Horizontal");
        }
    }

    void FixedUpdate()
    {
        if (isAlive)
        {
            rb.velocity = new Vector2(moveInput * moveSpeed, 0f);
            Vector2 constrainedPosition = transform.position;
            constrainedPosition.x = Mathf.Clamp(constrainedPosition.x, leftBoundary, rightBoundary);
            transform.position = constrainedPosition;
        }
    }

    public bool IsShieldActive()
    {
        return isShielded;
    }

    public void ActivateShield(float duraction, Color normalColor, Color warningColor)
    {
        // if a shield is already active, stop it so reset the timer
        if (shieldCoroutine != null)
        {
            StopCoroutine(shieldCoroutine);
        }
        shieldCoroutine = StartCoroutine(ShieldRoutine(duraction, normalColor, warningColor));
    }

    public void DeactivateShield()
    {
        if (shieldCoroutine != null) {
            StopCoroutine(shieldCoroutine);
            shieldCoroutine = null;
        } 
        if (shieldInstance != null)
        {
            shieldInstance.SetActive(false);
        }
        isShielded = false;
    }

    private IEnumerator ShieldRoutine(float duration, Color normalCOlor, Color warningColor)
    {
        isShielded = true;

        // Make a shield if it does not exist
        if (shieldInstance == null)
        {
            shieldInstance = Instantiate(shieldPrefab, transform.position, Quaternion.identity, transform);
        }
        shieldInstance.SetActive(true);
        SpriteRenderer shieldRenderer = shieldInstance.GetComponent<SpriteRenderer>();

        shieldRenderer.color = normalCOlor;

        if (duration > 3f)
        {
            yield return new WaitForSeconds(duration - 3f);
            shieldRenderer.color = warningColor;
        }

        // Wait for the final 3 seconds
        yield return new WaitForSeconds(Mathf.Min(duration, 3f));

        // deactivate the shield
        shieldInstance.SetActive(false);
        isShielded = false;
        shieldCoroutine = null;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            // --- UPDATED: Check its own shield status ---
            if (isShielded)
            {
                Destroy(other.gameObject);
                return;
            }

            Destroy(other.gameObject);
            if (isAlive) { Die(); }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("PowerUps"))
        {
            PowerUpController powerUp = other.GetComponent<PowerUpController>();
            if (powerUp != null && GameManager.instance != null) { GameManager.instance.ActivatePowerUp(powerUp.type); }
            Destroy(other.gameObject);
        }
    }

    void Die()
    {
        SoundManager.instance.PlayPlayerDeath();

        if (GameManager.instance != null)
        {
            GameManager.instance.SubtractScore(100);
        }

        isAlive = false;
        rb.velocity = Vector2.zero;
        moveInput = 0f;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        if (ballController != null && !ballController.IsInPlay())
        {
            ballController.ResetBallToPlayer();
        }
        StartCoroutine(RespawnPlayer());
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(5f);

        // Resets the player after death
        ResetPlayerPosition();

        ActivateShield(3f, Color.red, Color.red);

        // After a respawn, tell the ball to reset itself only if it is not currently in play.
        if (ballController != null && !ballController.IsInPlay())
        {
            ballController.ResetBallToPlayer();
        }
    }

    public void ResetPlayerPosition()
    {
        // Make sure player is visible and active
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;
        isAlive = true;

        // Move to the center starting position
        transform.position = new Vector2(0, transform.position.y);
    }

    public bool IsPlayerAlive()
    {
        return isAlive;
    }
}

