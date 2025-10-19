using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class AlienSwarmController : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject alienPrefab;
    public GameObject projectilePrefab;
    public List<GameObject> powerUpPrefabs;
    public int aliensPerRow = 8;
    public int numberOfRows = 4;
    public float padding = 1.5f;
    public float rowSpacing = 1.5f;
    public float startingYPosition = 10f;
    public float gameOverYPosition = -8.0f;

    [Header("Level 1 Difficulty Settings")]
    public float baseMoveSpeed = 1.5f;
    public float baseProjectileSpeed = 5f;
    public float baseMinFireDelay = 4.0f;
    public float baseMaxFireDelay = 20.0f;
    public float stepDownAmount = 0.5f;

    [Header("Movement Boundaries")]
    public float leftBoundary = -16.5f;
    public float rightBoundary = 16.5f;

    private float moveSpeed;
    private float projectileSpeed;
    private float minFireDelay;
    private float maxFireDelay;
    private float alienWidth;
    private bool movingRight = true;
    private List<Transform> alienTransforms = new List<Transform>();
    private bool isStopped = false;

    public void InitializeLevel(int level)
    {
        moveSpeed = baseMoveSpeed + (level - 1) * 0.2f;
        projectileSpeed = baseProjectileSpeed + (level - 1) * 0.3f;
        minFireDelay = Mathf.Max(0.5f, baseMinFireDelay - (level - 1) * 0.25f);
        maxFireDelay = Mathf.Max(2.0f, baseMaxFireDelay - (level - 1) * 0.5f);

        alienWidth = alienPrefab.GetComponent<BoxCollider2D>().bounds.size.x;
        SpawnAliens();
    }

    void Update()
    {
        // If the swarm is stopped by something like the Freeze power-up, do nothing
        if (isStopped) return;
        MoveSwarm();
    }

    void SpawnAliens()
    {
        for (int j = 0; j < numberOfRows; j++)
        {
            for (int i = 0; i < aliensPerRow; i++)
            {
                float totalWidth = (aliensPerRow - 1) * padding;
                float startX = -totalWidth / 2;
                float yPos = startingYPosition - (j * rowSpacing);
                float xPos = startX + i * padding;
                Vector3 spawnPosition = new Vector3(xPos, yPos, 0);
                GameObject alien = Instantiate(alienPrefab, spawnPosition, Quaternion.identity, transform);
                alien.layer = LayerMask.NameToLayer("Aliens");
                alienTransforms.Add(alien.transform);

                AlienController alienController = alien.GetComponent<AlienController>();
                if (alienController != null)
                {
                    alienController.powerUpPrefabs = this.powerUpPrefabs;

                    alienController.projectilePrefab = this.projectilePrefab;
                    alienController.projectileSpeed = this.projectileSpeed;
                    alienController.minFireDelay = this.minFireDelay;
                    alienController.maxFireDelay = this.maxFireDelay;
                }
            }
        }
    }

    void MoveSwarm()
    {
        alienTransforms.RemoveAll(item => item == null);
        if (alienTransforms.Count == 0)
        {
            if (!isStopped)
            {
                isStopped = true;
                GameManager.instance.WaveCleared();
                gameObject.SetActive(false);
            }
            return;
        }

        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        transform.position += direction * moveSpeed * Time.deltaTime;
        float currentLeftmost = float.MaxValue;
        float currentRightmost = float.MinValue;
        float currentLowest = float.MaxValue;

        foreach (Transform alien in alienTransforms)
        {
            if (alien.position.x < currentLeftmost)
                currentLeftmost = alien.position.x;
            if (alien.position.x > currentRightmost)
                currentRightmost = alien.position.x;
            if (alien.position.y < currentLowest)
                currentLowest = alien.position.y;
        }

        bool changeDirection = false;
        if (movingRight && (currentRightmost + (alienWidth / 2)) >= rightBoundary)
            changeDirection = true;
        else if (!movingRight && (currentLeftmost - (alienWidth / 2)) <= leftBoundary)
            changeDirection = true;

        if (changeDirection)
        {
            movingRight = !movingRight;
            Vector3 currentPosition = transform.position;
            currentPosition.y -= stepDownAmount;
            transform.position = currentPosition;
        }

        if (currentLowest <= gameOverYPosition)
        {
            GameManager.instance.GameOver();
        }
    }

    public void StopSwarm()
    {
        isStopped = true;
        foreach (Transform alien in alienTransforms)
        {
            if (alien != null)
            {
                alien.GetComponent<AlienController>().StopFiring();
            }
        }
    }

    public void ResumeSwarm()
    {
        isStopped = false;
        foreach (Transform alien in alienTransforms)
        {
            if (alien != null)
            {
                alien.GetComponent<AlienController>().ResumeFiring();
            }
        }
    }
}
