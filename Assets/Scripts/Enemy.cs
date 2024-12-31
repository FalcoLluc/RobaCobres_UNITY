using UnityEngine;

public class Enemy : MonoBehaviour
{
    public int playerDamage;
    public float speed = 2.0f; // velocitat del enemic
    public float attackCooldown = 1.0f; // temps d'espera dsprs d'atacar

    private Animator animator;
    private Transform target;
    private SpriteRenderer spriteRenderer;


    private Rigidbody2D rb; // s'utilitza per controlar el moviment
    private bool isAttacking = false; // Nueva variable para detener el movimiento
    private float attackTimer = 0f; // temporitzador per moure dsprs d'atacar
    private bool isBlocked = false; // s'activa si colisiona amb pared


    private bool isGameOver = false; // Estat per controlar si el joc ha acabat
    

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
       
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>(); // assigna el sprite renderer
        rb = GetComponent<Rigidbody2D>(); // Obt� Rigidbody2D
        target = GameObject.FindGameObjectWithTag("Player").transform;
        GameManager.instance.AddEnemyToList(this);
        // base.Start();
    }

    // Update is called once per frame
    void Update()
    {
        if (isAttacking)
        {
            // Temporizador para reanudar el movimiento
            attackTimer -= Time.deltaTime;
            if (attackTimer <= 0f)
            {
                isAttacking = false; // Reanudar movimiento
                isBlocked = false; // no esta bloquejat
                animator.SetBool("isBlocked", false);
            }
        }
        else if (isGameOver)
        {
            isAttacking = false; // Reanudar movimiento
            isBlocked = true;
            animator.SetBool("isBlocked", true);
            rb.linearVelocity = Vector2.zero; // Detener el movimiento físicamente
        }
        else
        {
            MoveEnemy();
        }
    }


    public void MoveEnemy()
    {
        /*if (target != null)
        {
            transform.position = Vector2.MoveTowards(transform.position, target.position, speed * Time.deltaTime);
        }*/
       if (target == null || rb == null || isBlocked) return;

        float moveSpeed = isAttacking ? speed * 0.5f : speed;
        // Movimiento hacia el jugador
        Vector2 newPosition = Vector2.MoveTowards(
            rb.position,
            target.position,
            moveSpeed * Time.deltaTime
        );

        rb.MovePosition(newPosition);
       
        // Animaci�n o direcci�n del sprite
        if (target.position.x < transform.position.x)
        {
            spriteRenderer.flipX = false;
        }
        else
        {
            spriteRenderer.flipX = true;
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
                isAttacking = true; // Detener el movimiento despu�s del ataque
                attackTimer = attackCooldown; // activa el temportzador
            }

        }
        else if (collision.gameObject.layer == LayerMask.NameToLayer("BlockingLayer"))
        {
            Debug.Log("Enemy blocked by a wall.");
            isBlocked = true; // Bloquear el movimiento temporalmente
            attackTimer = attackCooldown; // Usar cooldown para desbloquear
            animator.SetBool("isBlocked", true);
        }
    }
    private void OnDestroy()
    {
        GameManager.instance.RemoveEnemy(this); // Eliminar de la lista si es destruido
    }
    public void StopEnemy()
    {
        isGameOver = true;
        rb.linearVelocity = Vector2.zero; // Detener físicamente el movimiento
        //animator.enabled = false; // Desactivar animaciones
    }
}
