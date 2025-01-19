using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int playerDamage;
    public float speed = 2.0f; // Velocidad del enemigo
    public float attackCooldown = 1.0f; // Tiempo de espera después de atacar

    private Animator animator;
    private Transform target;
    private SpriteRenderer spriteRenderer;
    private Rigidbody2D rb; // Para controlar el movimiento
    private bool isAttacking = false; // Para detener el movimiento
    private float attackTimer = 0f; // Temporizador para reanudar el movimiento
    private bool isBlocked = false; // Para saber si está bloqueado por un obstáculo
    private bool isGameOver = false; // Para saber si el juego ha terminado

    public float detectionDistance = 5f; // Distancia para detectar obstáculos
    public LayerMask wallLayer;


    void Start()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        rb = GetComponent<Rigidbody2D>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        GameManager.instance.AddEnemyToList(this);
        //rb.sharedMaterial.friction = 0f;
    }

    void Update()
    {
        if (isAttacking)
        {
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false;
                isBlocked = false;
                //animator.SetBool("isBlocked", false);
            }
        }
        else if (isGameOver)
        {
            isAttacking = false;
            isBlocked = true;
            //animator.SetBool("isBlocked", true);
            rb.linearVelocity = Vector2.zero; // Detener el movimiento
        }
        else
        {
            MoveEnemy();
        }
    }

    public void MoveEnemy()
    {
        if (target == null || rb == null || isBlocked || GameManager.instance.GetSetupState()) return;

        // Raycast para detectar si hay un obstáculo entre el enemigo y el jugador
        RaycastHit2D hit = Physics2D.Raycast(transform.position, (target.position - transform.position).normalized, detectionDistance, wallLayer);
        if (hit.collider != null)
        {
            AvoidObstacle(); // Si detecta un obstáculo, evadirlo
            animator.SetBool("isMoving", false);
        }
        else
        {
            float moveSpeed = isAttacking ? speed * 0.5f : speed;

            // Movimiento hacia el jugador
            Vector2 newPosition = Vector2.MoveTowards(rb.position, target.position, moveSpeed * Time.deltaTime);
            rb.MovePosition(newPosition);
            animator.SetBool("isMoving", true);
        }

        // Animación o dirección del sprite
        if (target.position.x < transform.position.x)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Player hitPlayer = collision.gameObject.GetComponent<Player>();
            if (hitPlayer != null)
            {
                animator.SetTrigger("enemyAttack");
                hitPlayer.LoseCobre(playerDamage);
                isAttacking = true;
                attackTimer = attackCooldown;
            }
        }
        /*else if (collision.gameObject.layer == LayerMask.NameToLayer("BlockingLayer"))
        {
            isBlocked = true; // Bloquear el movimiento temporalmente
            attackTimer = attackCooldown; // Usar cooldown para desbloquear
            animator.SetBool("isBlocked", true);
        }*/
    }

    private void OnDestroy()
    {
        GameManager.instance.RemoveEnemy(this);
    }

    public void StopEnemy()
    {
        isGameOver = true;
        rb.linearVelocity = Vector2.zero; // Detener físicamente el movimiento
        rb.bodyType = RigidbodyType2D.Kinematic;  // Evitar que las físicas afecten al enemigo
    }

    // Método modificado para evadir obstáculos
    private void AvoidObstacle()
    {
        // Direcciones de 90 grados (izquierda, derecha, arriba, abajo)
        Vector2 directionToPlayer = (target.position - transform.position).normalized;
        Vector2 rightDirection = new Vector2(directionToPlayer.y, -directionToPlayer.x); // +90 grados
        Vector2 leftDirection = new Vector2(-directionToPlayer.y, directionToPlayer.x); // -90 grados
        Vector2 upDirection = directionToPlayer; // 0 grados (arriba)
        Vector2 downDirection = new Vector2(-directionToPlayer.x, -directionToPlayer.y); // 180 grados (abajo)

        // Raycasts a las 4 direcciones
        RaycastHit2D hitRight = Physics2D.Raycast(transform.position, rightDirection, detectionDistance, wallLayer);
        RaycastHit2D hitLeft = Physics2D.Raycast(transform.position, leftDirection, detectionDistance, wallLayer);
        RaycastHit2D hitUp = Physics2D.Raycast(transform.position, upDirection, detectionDistance, wallLayer);
        RaycastHit2D hitDown = Physics2D.Raycast(transform.position, downDirection, detectionDistance, wallLayer);

        // visualizamos raycasts
        Debug.DrawRay(transform.position, (target.position - transform.position).normalized * detectionDistance, Color.red);
        Debug.DrawRay(transform.position, rightDirection * detectionDistance, Color.green);
        Debug.DrawRay(transform.position, leftDirection * detectionDistance, Color.blue);
        Debug.DrawRay(transform.position, upDirection * detectionDistance, Color.yellow);
        Debug.DrawRay(transform.position, downDirection * detectionDistance, Color.magenta);


        // Comprobamos si las direcciones tienen obstáculos
        bool isBlockedRight = hitRight.collider != null;
        bool isBlockedLeft = hitLeft.collider != null;
        bool isBlockedUp = hitUp.collider != null;
        bool isBlockedDown = hitDown.collider != null;

        // Si todas las direcciones están bloqueadas, no nos movemos
        if (isBlockedRight && isBlockedLeft && isBlockedUp && isBlockedDown)
        {
            rb.linearVelocity = Vector2.zero; // Detener el movimiento
            //animator.SetBool("isBlocked", true); // Activar animación de bloqueo
        }
        else
        {
            // Elegir la dirección sin obstáculo
            if (!isBlockedRight)
            {
                rb.linearVelocity = rightDirection * speed; // Mover hacia la derecha
                //animator.SetBool("isBlocked", false);
            }
            else if (!isBlockedLeft)
            {
                rb.linearVelocity = leftDirection * speed; // Mover hacia la izquierda
                //animator.SetBool("isBlocked", false);
            }
            else if (!isBlockedUp)
            {
                rb.linearVelocity = upDirection * speed * 10; // Mover hacia arriba
                //animator.SetBool("isBlocked", false);
            }
            else if (!isBlockedDown)
            {
                rb.linearVelocity = downDirection * speed * 10; // Mover hacia abajo
                //animator.SetBool("isBlocked", false);
            }
        }
    }
}

