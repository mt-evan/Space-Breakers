using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienSwarmController : MonoBehaviour
{
    public GameObject alienPrefab; // Prefab of the alien to spawn
    public int aliensPerRow = 8;
    public float padding = 1.5f; // Padding for space between aliens
    public float moveSpeed = 2.0f;
    public float stepDownAmount = 0.5f;
    public float leftLimit = -16.5f;
    public float rightLimit = 16.5f;

    private bool movingRight = true;
    private List<Transform> alienTransforms = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        SpawnAliens();
    }

    // Update is called once per frame
    void Update()
    {
        // All aliens have been eliminated
        if (alienTransforms.Count == 0)
        {
            return;
        }

        MoveSwarm();
    }

    void SpawnAliens()
    {
        // Calculate the total width of the row of aliens
        float totalWidth = (aliensPerRow - 1) * padding;
        // Calculate the starting x position to center the row
        float startingX = -totalWidth / 2;

        // Spawns each of the aliens per row
        for (int i = 0; i <aliensPerRow; i++)
        {
            float xPosition = startingX + i * padding;
            Vector3 spawnPosition = new Vector3(xPosition, 10, 0); // Note that this is hardcoded for y level
            GameObject alien = Instantiate(alienPrefab, spawnPosition, Quaternion.identity, transform);
            alienTransforms.Add(alien.transform);
        }
    }

    void MoveSwarm()
    {
        // Figure out direction of movement and amount of movement
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Check if the aliens need to change direciton
        bool changeDirection = false;
        foreach (Transform alien in alienTransforms)
        {
            if (movingRight && alien.position.x >= rightLimit)
            {
                changeDirection = true;
                break;
            }
            else if (!movingRight && alien.position.x <= leftLimit)
            {
                changeDirection = true;
                break;
            }
        }

        if (changeDirection)
        {
            movingRight = !movingRight; // Change direction
            // Move downwards a bit
            Vector3 currentPosition = transform.position;
            currentPosition.y -= stepDownAmount;
            transform.position = currentPosition;
        }
    }
}
