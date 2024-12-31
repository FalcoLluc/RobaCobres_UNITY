using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Rendering;

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

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // Get the SpriteRenderer component
    }

    void Update()
    {
        if (isImmobilized)
            return;

        // Handle movement input
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 movement = new Vector2(horizontal, vertical).normalized;

        // Apply movement to Rigidbody2D
        rb.linearVelocity = movement * moveSpeed;

        // Flip the sprite based on movement direction
        if (horizontal < 0) // Moving left
        {
            spriteRenderer.flipX = true;
        }
        else if (horizontal > 0) // Moving right
        {
            spriteRenderer.flipX = false;
        }

        // Update animations
        if (movement.magnitude > 0)
        {
            //animator.SetFloat("moveX", horizontal);
            //animator.SetFloat("moveY", vertical);
            animator.SetBool("isMoving", true);
        }
        else
        {
            animator.SetBool("isMoving", false);
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Fence")
        {
            // Si el objeto es una "Fence", ejecuta la lógica para dañarla
            Wall hitFence = collision.gameObject.GetComponent<Wall>();
            if (hitFence != null)
            {
                hitFence.DamageWall(wallDamage); // Daño a la Fence
                animator.SetTrigger("playerChop"); // Activar animación de corte
            }

            // Detener el movimiento después de la colisión
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
        if (other.tag == "Furgo")
        {
            //FER COSES FURGO
            //Invoke("Restart", restartLevelDelay);
            //enabled = false;
        }
        else if (other.tag == "Tren")
        {
            isImmobilized = true;
            rb.linearVelocity = Vector2.zero;
            GameManager.instance.GameOver();
            animator.SetTrigger("playerDead");
            Debug.Log("Game Over Atropellado");
        }
        else if (other.tag == "Cobre")
        {
            cobre += pointsPerCobre;
            other.gameObject.SetActive(false);
        }
        else if (other.tag == "CobreRajola")
        {
            cobre += pointsPerCobreRajola;
            other.gameObject.SetActive(false);
        }
    }

    //Aquesta part igual sobraria pk es de RogueLike
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseCobre(int loss)
    {
        animator.SetTrigger("playerHit");
        Debug.Log("Hit.");
        cobre -= loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (cobre <= 0)
        {
            isImmobilized = true;
            rb.linearVelocity = Vector2.zero;
            GameManager.instance.GameOver();
            animator.SetTrigger("playerDead");
            Debug.Log("Game Over Paliza");
        }
    }
}



