using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    private int wallHitCounter = 0;
    [Header("Settings")]
    public float ballSpeed = 9f;
    public PlayerController playerController;
    public float ballYOffset = 0.5f;
    public float lossBoundary = -20f;
    public int maxConsecutiveWallHits = 4;
    public float ballRespawnDelay = 2f;

    public LayerMask alienLayer;
    private float ballRadius;

    [Header("Sprites")]
    public Sprite defaultSprite;
    public Sprite pierceSprite;

    private Rigidbody2D rb;
    private CircleCollider2D circleCollider;
    private bool inPlay = false;
    private SpriteRenderer spriteRenderer;
    private bool isResetting = false;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        circleCollider = GetComponent<CircleCollider2D>();
        ballRadius = circleCollider.radius;
    }

    void Update()
    {
        if (!inPlay)
        {
            if (playerController != null && playerController.IsPlayerAlive() && !isResetting)
            {
                // Re-enable physics and visuals
                rb.isKinematic = false;
                circleCollider.enabled = true;
                spriteRenderer.enabled = true;

                transform.position = new Vector3(playerController.transform.position.x, playerController.transform.position.y + ballYOffset, 0);

                if (Input.GetButtonDown("Jump"))
                {
                    inPlay = true;
                    rb.isKinematic = false;
                    circleCollider.enabled = true;
                    rb.velocity = Vector2.up * ballSpeed;
                }
            }
            else
            {
                // Keep physics/visuals disabled
                spriteRenderer.enabled = false;
                if (rb != null && !rb.isKinematic) rb.isKinematic = true;
                if (circleCollider != null && circleCollider.enabled) circleCollider.enabled = false;
            }
        }
        else // Ball is in play
        {
            if (transform.position.y < lossBoundary && !isResetting)
            {
                isResetting = true;
                rb.velocity = Vector2.zero;
                spriteRenderer.enabled = false;
                // Disable physics components on loss
                rb.isKinematic = true;
                circleCollider.enabled = false;
                StartCoroutine(RespawnBallAfterDelay());
            }

            if (GameManager.instance != null && GameManager.instance.IsPierceActive())
            {
                CheckForPierceableAliens();
            }
        }
    }

    IEnumerator RespawnBallAfterDelay()
    {
        if (GameManager.instance != null) GameManager.instance.SubtractScore(10);
        yield return new WaitForSeconds(ballRespawnDelay);
        ResetBallToPlayer();
        isResetting = false;
    }


    public void ResetBallToPlayer()
    {
        inPlay = false;
        wallHitCounter = 0;

        // Stop movement and disable physics/collider
        if (rb != null)
        {
            rb.velocity = Vector2.zero;
            rb.isKinematic = true;
        }
        if (circleCollider != null)
        {
            circleCollider.enabled = false;
        }
        if (spriteRenderer != null)
        {
            spriteRenderer.enabled = false;
        }
        // transform.position = new Vector3(0, 100, 0);
    }

    void CheckForPierceableAliens()
    {
        Collider2D[] aliensToDestroy = Physics2D.OverlapCircleAll(transform.position, ballRadius, alienLayer);
        foreach (Collider2D alien in aliensToDestroy)
        {
            if (SoundManager.instance != null) SoundManager.instance.PlayAlienHit();
            if (GameManager.instance != null) GameManager.instance.AddScore(10);
            Destroy(alien.gameObject);
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Don't process collisions if resetting
        if (isResetting) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            wallHitCounter = 0;
            if (inPlay && SoundManager.instance != null) SoundManager.instance.PlayPlayerBounce();
            Vector2 playerPosition = collision.transform.position;
            Vector2 ballPosition = transform.position;
            float playerWidth = collision.collider.bounds.size.x;

            // Safety check for player width
            if (Mathf.Approximately(playerWidth, 0f))
            {
                Debug.LogWarning("Player width is zero during collision. Bouncing straight up.");
                rb.velocity = Vector2.up * ballSpeed;
                return;
            }

            float contactPoint = (ballPosition.x - playerPosition.x) / playerWidth;
            Vector2 newDirection = new Vector2(contactPoint, 1).normalized;

            // Safety check for NaN direction
            if (float.IsNaN(newDirection.x) || float.IsNaN(newDirection.y))
            {
                Debug.LogWarning("Calculated NaN bounce direction. Bouncing straight up.");
                rb.velocity = Vector2.up * ballSpeed;
                return;
            }

            rb.velocity = newDirection * ballSpeed;
        }
        else if (collision.gameObject.CompareTag("Alien"))
        {
            wallHitCounter = 0;
            if (SoundManager.instance != null) SoundManager.instance.PlayAlienHit();
            if (GameManager.instance != null) GameManager.instance.AddScore(10);
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.CompareTag("Wall"))
        {
            if (SoundManager.instance != null) SoundManager.instance.PlayWallBounce();
            wallHitCounter++;
            if (wallHitCounter >= maxConsecutiveWallHits)
            {
                if (GameManager.instance != null) GameManager.instance.SubtractScore(10);
                isResetting = true;
                // Disable physics components on wall reset trigger
                rb.velocity = Vector2.zero;
                rb.isKinematic = true;
                circleCollider.enabled = false;
                spriteRenderer.enabled = false;
                StartCoroutine(RespawnBallAfterDelay());
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

