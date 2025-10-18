using UnityEngine;

public class BallController : MonoBehaviour
{
    public float ballSpeed = 9f;
    public PlayerController playerController;
    public float ballYOffset = 0.5f;
    public float lossBoundary = -20f;

    private Rigidbody2D rb;
    private bool inPlay = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (!inPlay)
        {
            if (playerController != null && playerController.IsPlayerAlive())
            {
                spriteRenderer.enabled = true;
                transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y + ballYOffset, 0);

                if (Input.GetButtonDown("Jump"))
                {
                    inPlay = true;
                    rb.velocity = Vector2.up * ballSpeed;
                }
            }
            else
            {
                spriteRenderer.enabled = false;
            }
        }
        else
        {
            if (transform.position.y < lossBoundary)
            {
                inPlay = false;
                rb.velocity = Vector2.zero;
            }
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Projectiles"))
        {
            Destroy(other.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Vector2 playerPosition = collision.transform.position;
            Vector2 ballPosition = transform.position;
            float playerWidth = collision.collider.bounds.size.x;
            float contactPoint = (ballPosition.x - playerPosition.x) / playerWidth;
            Vector2 newDirection = new Vector2(contactPoint, 1).normalized;
            rb.velocity = newDirection * ballSpeed;
        }
        else if (collision.gameObject.CompareTag("Alien"))
        {
            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(10);
            }
            Destroy(collision.gameObject);
        }
    }

    public void ResetBallToPlayer()
    {
        inPlay = false;
        rb.velocity = Vector2.zero;
    }
}

