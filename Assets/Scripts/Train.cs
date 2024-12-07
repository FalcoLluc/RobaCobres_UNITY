using UnityEngine;

public class Train : MonoBehaviour
{
    public float minSpeed; // Minimum speed of the train
    public float maxSpeed; // Maximum speed of the train
    public float speed;
    private Vector3 moveDirection = Vector3.right;  // Direction the train is moving
    private Vector3 startPosition;  // Store the train's starting position

    private Rigidbody2D rb2d;  // Reference to Rigidbody2D for detecting collisions

    private BoxCollider2D boxCollider;
    private SpriteRenderer spriteRenderer;

    // Start method to set initial conditions
    void Start()
    {
        startPosition = transform.position;  // Store the starting position of the train
        rb2d = GetComponent<Rigidbody2D>();  // Get the Rigidbody2D component
        boxCollider = GetComponent<BoxCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // FixedUpdate method to move the train
    void FixedUpdate()
    {
        // Move the train at the specified speed using Rigidbody2D
        rb2d.MovePosition(rb2d.position + (Vector2)moveDirection * speed * Time.fixedDeltaTime);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        // Check if the collided object has the "Finish" tag
        if (other.CompareTag("Bordes"))
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

        // SPEED RANDOM
        speed = Random.Range(minSpeed, maxSpeed);

        // Load sprites from the Resources folder
        Sprite[] sprites = Resources.LoadAll<Sprite>("Trains");

        if (sprites != null && sprites.Length > 0)
        {
            // Ensure trainSprite index is within the range of loaded sprites
            int trainSpriteIndex = Random.Range(0, 5);
            spriteRenderer.sprite = sprites[trainSpriteIndex];
        }
        else
        {
            Debug.LogError("No sprites found in the 'Trains' folder.");
        }
    }
}













