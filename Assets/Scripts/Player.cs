using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;
using UnityEngine.EventSystems; // Needed for touch events
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public int wallDamage = 1;
    public int pointsPerCobre = 10;
    public int pointsPerCobreRajola = 20;
    public float restartLevelDelay = 1f;

    private int cobre;
    public float moveSpeed = 5f;
    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer spriteRenderer; // To control sprite flipping
    private bool isImmobilized = false;
    public float joystickRadius = 50f;       // The maximum movement distance of the joystick handle

    private Vector2 joystickInput;

    public AudioClip moveSound1;
    public AudioClip moveSound2;
    public AudioClip gameOverSound;

    public Text cobreText;
    // Joystick Variables
    public Image joystickBackground;  // Change this to Image
    public Image joystickHandle;      // Change this to Image

    void Start()
    {
        cobreText = GameObject.Find("CobreText").GetComponent<Text>();
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
        cobreText.text = "Cobre: " + cobre;

        joystickBackground = GameObject.Find("JoystickBackground").GetComponent<Image>();
        joystickHandle = GameObject.Find("JoystickHandle").GetComponent<Image>();
    }

    void Update()
    {
        if (isImmobilized)
        {
            rb.linearVelocity = Vector2.zero; // Ensure no residual movement
            return;
        }

#if UNITY_EDITOR || UNITY_STANDALONE || UNITY_WEBPLAYER
        // Handle keyboard or joystick input for testing in the editor
        float horizontal = (int)Input.GetAxisRaw("Horizontal");
        float vertical = (int)Input.GetAxisRaw("Vertical");

        // Enforce movement on a single axis (horizontal or vertical)
        if (horizontal != 0)
        {
            vertical = 0; // Prevent diagonal movement
        }

        Vector2 movement = new Vector2(horizontal, vertical).normalized; // Normalize movement to avoid faster diagonal movement
#else
        // Handle touch input for Android (or other mobile platforms)
        HandleJoystickInput();
        Vector2 movement = joystickInput.normalized; // Normalize joystick input to avoid faster diagonal movement
#endif

        // Apply movement to Rigidbody2D
        rb.linearVelocity = movement * moveSpeed;

        // Flip the sprite based on movement direction
        if (movement.x < 0) // Moving left
        {
            spriteRenderer.flipX = true;
        }
        else if (movement.x > 0) // Moving right
        {
            spriteRenderer.flipX = false;
        }

        // Update animations
        if (movement.magnitude > 0)
        {
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    // Function to handle the joystick input for mobile platforms
    private void HandleJoystickInput()
    {
        if (Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);

            // Get the touch position in the canvas space (UI space)
            Vector2 touchPosition = touch.position; // touch.position is in screen space, we will keep it in screen space

            if (touch.phase == TouchPhase.Began)
            {
                Debug.Log("Touch began at: " + touchPosition);

                // Make sure the joystick background doesn't move, only the handle should move
                joystickBackground.rectTransform.position = touchPosition;
                joystickHandle.rectTransform.position = touchPosition;
            }

            if (touch.phase == TouchPhase.Moved || touch.phase == TouchPhase.Stationary)
            {
                // Calculate the movement direction in screen space
                Vector2 direction = touchPosition - (Vector2)joystickBackground.rectTransform.position;
                float distance = Mathf.Clamp(direction.magnitude, 0f, joystickRadius); // Limit the movement to joystick radius

                // Move the joystick handle, but clamp it within the joystick radius
                joystickHandle.rectTransform.position = (Vector2)joystickBackground.rectTransform.position + direction.normalized * distance;

                // Store the joystick input (normalized direction)
                joystickInput = direction.normalized;

                Debug.Log("Joystick direction: " + joystickInput);
            }

            if (touch.phase == TouchPhase.Ended)
            {
                Debug.Log("Touch ended.");

                // Reset joystick handle position when touch ends
                joystickHandle.rectTransform.position = joystickBackground.rectTransform.position;
                joystickInput = Vector2.zero;
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Fence"))
        {
            // If the object is a "Fence", execute logic to damage it
            Wall hitFence = collision.gameObject.GetComponent<Wall>();
            if (hitFence != null)
            {
                hitFence.DamageWall(wallDamage); // Damage the Fence
                animator.SetTrigger("playerChop"); // Activate chop animation
            }

            // Stop movement after collision
            rb.linearVelocity = Vector2.zero;
        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("BlockingLayer"))
        {
            Debug.Log("Collided with an object in BlockingLayer.");
            rb.linearVelocity = Vector2.zero; // Stop player when hitting a wall
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Furgo"))
        {
            // Handle Furgo interaction
        }
        else if (other.CompareTag("Tren"))
        {
            cobre = 0;
            Debug.Log("Game Over Atropellado");
            CheckIfGameOver();
        }
        else if (other.CompareTag("Cobre"))
        {
            cobre += pointsPerCobre;
            other.gameObject.SetActive(false);
            cobreText.text = "Cobre: " + cobre;
        }
        else if (other.CompareTag("CobreRajola"))
        {
            cobre += pointsPerCobreRajola;
            other.gameObject.SetActive(false);
        }
    }

    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseCobre(int loss)
    {
        animator.SetTrigger("playerHit");
        cobre -= loss;
        cobreText.text = "Cobre: " + cobre;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (cobre <= 0)
        {
            SoundManager.instance.PlaySingle(gameOverSound);
            SoundManager.instance.musicSource.Stop();
            isImmobilized = true;
            rb.linearVelocity = Vector2.zero;
            rb.bodyType = RigidbodyType2D.Kinematic;
            GameManager.instance.GameOver();
            animator.SetTrigger("playerDead");
            Debug.Log("Game Over Paliza");
        }
    }
}

