using UnityEngine;

public class BallController : MonoBehaviour
{
    public float ballSpeed = 9f;
    public PlayerController playerController;
    public float ballYOffset = 0.8f;
    public float lossBoundary = -20f;
    public int maxConsecutiveWallHits = 4;
    private int wallHitCounter = 0;

    public LayerMask alienLayer;
    private float ballRadius;

    public Sprite defaultSprite; // yellow orb
    public Sprite pierceSprite; // red orb

    private Rigidbody2D rb;
    private bool inPlay = false;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        ballRadius = GetComponent<CircleCollider2D>().radius;
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
                if (GameManager.instance != null)
                {
                    GameManager.instance.SubtractScore(10);
                }
                inPlay = false;
                rb.velocity = Vector2.zero;
            }

            // If the pierce powerup is active, constantly check for aliens to destroy
            if (GameManager.instance != null && GameManager.instance.IsPierceActive())
            {
                CheckForPierceableAliens();
            }
        }
    }

    void CheckForPierceableAliens()
    {
        // Create an invisible circle around the ball and get a list of all aliens inside it.
        Collider2D[] aliensToDestroy = Physics2D.OverlapCircleAll(transform.position, ballRadius, alienLayer);

        // Loop through the list and destroy each one.
        foreach (Collider2D alien in aliensToDestroy)
        {
            SoundManager.instance.PlayAlienHit();

            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(10);
            }
            Destroy(alien.gameObject);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            wallHitCounter = 0;
            if (inPlay) SoundManager.instance.PlayPlayerBounce();

            Vector2 playerPosition = collision.transform.position;
            Vector2 ballPosition = transform.position;
            float playerWidth = collision.collider.bounds.size.x;
            float contactPoint = (ballPosition.x - playerPosition.x) / playerWidth;
            Vector2 newDirection = new Vector2(contactPoint, 1).normalized;
            rb.velocity = newDirection * ballSpeed;
        }
        else if (collision.gameObject.CompareTag("Alien"))
        {
            wallHitCounter = 0;
            SoundManager.instance.PlayAlienHit();

            if (GameManager.instance != null)
            {
                GameManager.instance.AddScore(10);
            }
            // If pierce is active, the physics engine will ignore this collision.
            // If it's not active, this code will run and destroy the alien.
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            SoundManager.instance.PlayWallBounce();

            wallHitCounter++;
            if (wallHitCounter >= maxConsecutiveWallHits)
            {
                GameManager.instance.SubtractScore(10);
                ResetBallToPlayer();
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

    public void ResetBallToPlayer()
    {
        inPlay = false;
        rb.velocity = Vector2.zero;
        wallHitCounter = 0;

        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        transform.position = new Vector3(0, 100, 0); // move off screen so ball doesn't affect the arena
    }

    public bool IsInPlay()
    {
        return inPlay;
    }

    public void SetAppearance(bool isPiercing)
    {
        if (isPiercing)
        {
            spriteRenderer.sprite = pierceSprite;
        }
        else
        {
            spriteRenderer.sprite = defaultSprite;
        }
    }
}
