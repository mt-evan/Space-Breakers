using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    // How fast the player moves
    public float moveSpeed = 10f;

    // The boundaries the player cannot pass
    public float leftBoundary = -16.5f;
    public float rightBoundary = 16.5f;

    // Respawn wait time
    public float respawnTime = 5f;

    private Rigidbody2D rb;
    private float moveInput;
    private bool isAlive = true;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Only allow input from the user if the player is alive
        if (isAlive)
        {
            // Get the input for left/right arrow keys
            // Value will be -1 for left and 1 for right and 0 for no input
            moveInput = Input.GetAxis("Horizontal");
        }
        

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    // FixedUpdate is called on a fixed timer and is best for physics calculations
    void FixedUpdate()
    {
        if (isAlive)
        {
            // Apply velocity based on the user's input
            rb.velocity = new Vector2(moveInput * moveSpeed, 0f);

            // Clamp the player's position to stay within the boundaries
            Vector2 clampedPosition = transform.position;
            clampedPosition.x = Mathf.Clamp(clampedPosition.x, leftBoundary, rightBoundary);

            transform.position = clampedPosition;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if player collided with a projectile
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            Destroy(collision.gameObject);

            if (isAlive)
            {
                Die();
            }
        }
    }

    //private void OnCollisionEnter2D(Collision2D collision)
    //{
    //    // Check if the collision is with the projectile
    //    if (collision.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
    //    {
    //        Destroy(collision.gameObject); // Destroy the projectile

    //        // Only have the player die if it is not already in the respawning process
    //        if (isAlive)
    //        {
    //            Die();
    //        }
    //    }
    //}

    void Die()
    {
        isAlive = false;
        // Stop movement related to the player immediately
        rb.velocity = Vector2.zero;
        moveInput = 0f;

        // Hide the player
        GetComponent<SpriteRenderer>().enabled = false;
        GetComponent<BoxCollider2D>().enabled = false;

        // Start the respawning
        StartCoroutine(RespawnPlayer());
    }

    IEnumerator RespawnPlayer()
    {
        // Wait for respawn time
        yield return new WaitForSeconds(respawnTime);

        // Respawn at center of the screen
        transform.position = new Vector2(0, transform.position.y);

        // Make the player appear
        GetComponent<SpriteRenderer>().enabled = true;
        GetComponent<BoxCollider2D>().enabled = true;

        // Set the flag to allow user input to affect movement again
        isAlive = true;
    }
}
