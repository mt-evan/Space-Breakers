using UnityEngine;

public class BallController : MonoBehaviour
{
    public float ballSpeed = 9f;
    public Transform playerTransform;
    public float ballYOffset = 0.5f;
    public float lossBoundary = -20f; // The Y position where the ball is considered lost

    private Rigidbody2D rb;
    private bool inPlay = false; // Flag for checking if ball is still in play


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!inPlay)
        {
            // If the ball is NOT in play, make it follow the player
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + ballYOffset, 0);

            // Check if the player launches the ball
            if (Input.GetButtonDown("Jump"))
            {
                inPlay = true;
                rb.velocity = Vector2.up * ballSpeed;
            }
        }
        else
        {
            // If the ball IS in play, check if it has fallen off the screen
            if (transform.position.y < lossBoundary)
            {
                // If it has, reset it and make it not move vertically
                inPlay = false;
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Check if the ball hit a projectile
        if (collision.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            Destroy(collision.gameObject);
        }
    }

    // This is called by the physics engine when this collider hits something
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the object that ball collided with is the Player
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 playerPosition = collision.transform.position;
            Vector2 ballPosition = transform.position;

            // Calculate the ball's direction of movement depending on where on the player it hits
            float playerWidth = collision.collider.bounds.size.x;
            float contactPoint = (ballPosition.x - playerPosition.x) / playerWidth;
            Vector2 newDirection = new Vector2(contactPoint, 1).normalized;
            rb.velocity = newDirection * ballSpeed;
        }
        else if (collision.gameObject.CompareTag("Alien"))
        {
            // Find the swarm controller on the parent of the alien
            AlienSwarmController swarmController = collision.transform.GetComponent<AlienSwarmController>();
            if (swarmController != null)
            {
                // Have the controller remote the alien from its list for consistent data
                swarmController.RemoveAlien(collision.transform);
            }

            // Destroy alien if the ball hits it
            Destroy(collision.gameObject);
        }
    }
}

