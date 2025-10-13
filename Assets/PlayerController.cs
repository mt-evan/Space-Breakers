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

    private Rigidbody2D rb;
    private float moveInput;


    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        // Get the input for left/right arrow keys
        // Value will be -1 for left and 1 for right and 0 for no input
        moveInput = Input.GetAxis("Horizontal");

        rb.velocity = new Vector2(moveInput * moveSpeed, rb.velocity.y);
    }

    // FixedUpdate is called on a fixed timer and is best for physics calculations
    void FixedUpdate()
    {
        // Apply velocity based on the user's input
        rb.velocity = new Vector2(moveInput * moveSpeed, 0f);

        // Clamp the player's position to stay within the boundaries
        Vector2 clampedPosition = transform.position;
        clampedPosition.x = Mathf.Clamp(clampedPosition.x, leftBoundary, rightBoundary);
        
        transform.position = clampedPosition;
    }
}
