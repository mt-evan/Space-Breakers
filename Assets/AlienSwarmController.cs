using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AlienSwarmController : MonoBehaviour
{
    public GameObject alienPrefab; // Prefab of the alien to spawn
    public int aliensPerRow = 8;
    public int numberOfRows = 4;
    public float padding = 1.5f; // Padding for space between aliens
    public float rowSpacing = 1.5f; // Padding for space between rows
    public float moveSpeed = 2.0f;
    public float stepDownAmount = 0.5f;
    public float leftLimit = -16.5f;
    public float rightLimit = 16.5f;

    // Alien's width for accurate boundary checks
    private float alienWidth;

    private bool movingRight = true;
    private List<Transform> alienTransforms = new List<Transform>();

    // Start is called before the first frame update
    void Start()
    {
        alienWidth = alienPrefab.GetComponent<BoxCollider2D>().bounds.size.x;
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

        // Loop each row
        for (int j = 0; j < numberOfRows; j++)
        {
            // Calculate the y position for the row
            float yPosition = 10 - (j * rowSpacing);

            // Spawns each of the aliens per row
            for (int i = 0; i < aliensPerRow; i++)
            {
                float xPosition = startingX + i * padding;
                Vector3 spawnPosition = new Vector3(xPosition, yPosition, 0); // Note that this is hardcoded for y level
                GameObject alien = Instantiate(alienPrefab, spawnPosition, Quaternion.identity, transform);
                alienTransforms.Add(alien.transform);
            }
        }
    }

    void MoveSwarm()
    {
        // Figure out direction of movement and amount of movement
        Vector3 direction = movingRight ? Vector3.right : Vector3.left;
        transform.position += direction * moveSpeed * Time.deltaTime;

        // Find the current leftmost and rightmost aliens of the swarm
        float currentLeftmost = float.MaxValue;
        float currentRightmost = float.MinValue;

        // Loop backwards to safely remove destroyed aliens from the list
        for (int i = alienTransforms.Count - 1; i >= 0; i--)
        {
            // If the alien has been destroyed, its reference will be null
            if (alienTransforms[i] == null)
            {
                // Remove the null reference from the list
                alienTransforms.RemoveAt(i);
                continue;
            }

            // If the alien is valid, check its position for the boundary check
            if (alienTransforms[i].position.x < currentLeftmost)
            {
                currentLeftmost = alienTransforms[i].position.x;
            }
            if (alienTransforms[i].position.x > currentRightmost)
            {
                currentRightmost = alienTransforms[i].position.x;
            }
        }

        // If all aliens were destroyed in this frame, stop here
        if (alienTransforms.Count == 0) return;

        // Check the swarm's OUTER EDGE against the boundary
        bool changeDirection = false;
        if (movingRight && (currentRightmost + (alienWidth / 2)) >= rightLimit)
        {
            changeDirection = true;
        }
        else if (!movingRight && (currentLeftmost - (alienWidth / 2)) <= leftLimit)
        {
            changeDirection = true;
        }

        if (changeDirection)
        {
            movingRight = !movingRight; // Reverse direction
            Vector3 currentPosition = transform.position;
            currentPosition.y -= stepDownAmount; // Move down
            transform.position = currentPosition;
        }
    }

    // Public method for the BallController.cs file to call to report that an alien is destroyed
    public void RemoveAlien(Transform alien)
    {
        if (alienTransforms.Contains(alien))
        {
            alienTransforms.Remove(alien);
        }
    }
}
