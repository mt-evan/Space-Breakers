using UnityEngine;

public class BallController : MonoBehaviour
{
    // The speed the ball gets launched at
    public float ballSpeed = 7f;

    // A reference to the player's Transform component
    public Transform playerTransform;

    // How high above the player the ball should sit
    public float ballYOffset = 0.5f;

    // The Y position where the ball is considered lost
    public float lossBoundary = -15f;

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
        if (!inPlay)
        {
            // If the ball is NOT in play, make it follow the player
            transform.position = new Vector3(playerTransform.position.x, playerTransform.position.y + ballYOffset, 0);

            // Check if the player wants to launch the ball
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
                // If it has, reset it
                inPlay = false;
                rb.velocity = Vector2.zero; // Stop all movement
            }
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
            float playerWidth = collision.collider.bounds.size.x;
            float contactPoint = (ballPosition.x - playerPosition.x) / playerWidth;
            Vector2 newDirction = new Vector2(contactPoint, 1).normalized;
            rb.velocity = newDirction * ballSpeed;
        }
    }
}

