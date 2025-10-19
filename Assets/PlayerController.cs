using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour
{
    public float moveSpeed = 10f;
    public float leftBoundary = -16.5f;
    public float rightBoundary = 16.5f;
    public BallController ballController;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isAlive = true;

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

    private void OnTriggerEnter2D(Collider2D other)
    {
        // Check if we hit a projectile
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            // Check with the GameManager if the shield is active
            if (GameManager.instance != null && GameManager.instance.IsShieldActive())
            {
                // If shield is active, just destroy the projectile and do nothing else
                Destroy(other.gameObject);
                return;
            }

            Destroy(other.gameObject);
            if (isAlive)
            {
                Die();
            }
        }
        // Check if we hit a power-up
        else if (other.gameObject.layer == LayerMask.NameToLayer("PowerUps"))
        {
            PowerUpController powerUp = other.GetComponent<PowerUpController>();
            if (powerUp != null && GameManager.instance != null)
            {
                // Tell the GameManager which power-up was collected
                GameManager.instance.ActivatePowerUp(powerUp.type);
            }
            // Destroy the power-up object after collecting it
            Destroy(other.gameObject);
        }
    }

    void Die()
    {
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

