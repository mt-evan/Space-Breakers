using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallController : MonoBehaviour
{
    // The speed the ball gets launched at
    public float ballSpeed = 7f;

    private Rigidbody2D rb;

    // flag for checking if ball is still in play
    private bool inPlay = false;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Check if ball is not in play, so if player presses jump, it can launch
        if (!inPlay && Input.GetButtonDown("Jump"))
        {
            inPlay = true;
            rb.velocity = Vector2.up * ballSpeed;
        }
    }

    // This is called by the physics engine when this collider hits something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that ball collided with is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            // Get position of the player
            Vector2 playerPosition = collision.transform.position;
            // Get position of the ball
            Vector2 ballPosition = transform.position;

            // Get width of the player's ship
            float playerWidth = collision.collider.bounds.size.x;

            // Calculate where the ball hits the player ship
            // Result is a value from -0.5 for far left and 0.5 for far right
            float contactPoint = (ballPosition.x - playerPosition.x) / playerWidth;

            // Make a new direction vector
            // x is based on contactPoint, y is always up as 1
            Vector2 newDirction = new Vector2(contactPoint, 1).normalized;

            // Set the ball's new velocity
            rb.velocity = newDirction * ballSpeed;
        }
    }
}
