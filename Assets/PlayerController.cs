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
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            Destroy(other.gameObject);
            if (isAlive)
            {
                Die();
            }
        }
    }

    void Die()
    {
        isAlive = false;
        rb.velocity = Vector2.zero;
        moveInput = 0f;
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;
        StartCoroutine(RespawnPlayer());
    }

    IEnumerator RespawnPlayer()
    {
        yield return new WaitForSeconds(5f);

        // Resets the player after death
        ResetPlayerPosition();

        // After a respawn, tell the ball to reset itself.
        if (ballController != null)
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

