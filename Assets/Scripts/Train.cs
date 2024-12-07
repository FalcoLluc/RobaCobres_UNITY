using UnityEngine;

public class Train : MonoBehaviour
{
    public float speed = 3f;  // Speed of the train
    private Vector3 moveDirection = Vector3.right;  // Direction the train is moving
    private Vector3 startPosition;  // Store the train's starting position

    private Rigidbody2D rb2d;  // Reference to Rigidbody2D for detecting collisions

    // Start method to set initial conditions
    void Start()
    {
        startPosition = transform.position;  // Store the starting position of the train
        rb2d = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
    }

    // FixedUpdate method to move the train
    void FixedUpdate()
    {
        // Move the train at the specified speed using Rigidbody2D
        rb2d.MovePosition(rb2d.position + (Vector2)moveDirection * speed * Time.fixedDeltaTime);
    }

    // Handle collisions with other objects using OnCollisionEnter2D
    void OnCollisionEnter2D(Collision2D collision)
    {
        // Check if the collided object has the "Finish" tag
        if (collision.gameObject.CompareTag("Bordes"))
        {
            // If it hits the finish, reset the train's position
            ResetTrainPosition();
        }
    }

    // Reset the train to its starting position
    void ResetTrainPosition()
    {
        transform.position = startPosition;  // Set the train's position back to the origin
        rb2d.linearVelocity = Vector2.zero;  // Reset velocity to prevent movement after reset
        Debug.Log("Train position reset to start.");
    }
}













