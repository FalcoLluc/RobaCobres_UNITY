using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SceneManagement;


public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerCobre = 10;
    public int pointsPerCobreRajola = 20;
    public float restartLevelDelay = 1f;

    //velocidad de movimiento del jugador:
    private float moveSpeed = 5f;

    // variable per imobilitzar si li pasa un tren per sobre
    private bool isImmobilized = false;

    private Animator animator;
    private int cobre;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        animator = GetComponent<Animator>();

        cobre = GameManager.instance.playerCobrePoints;
        base.Start();
    }

    private void OnDisable()
    {
        GameManager.instance.playerCobrePoints = cobre;
    }

    // Update is called once per frame
    void Update()
    {
        if (isImmobilized || !GameManager.instance.playersTurn) return;

        int horizontal = 0;
        int vertical = 0;

        horizontal = (int)Input.GetAxisRaw("Horizontal");
        vertical = (int)Input.GetAxisRaw("Vertical");

        if (horizontal != 0)
            vertical = 0;
        if (horizontal != 0 || vertical != 0)
        {

            AttemptMove<Wall>(horizontal, vertical);
        }
        /* Vector2 move = new Vector2(horizontal, vertical).normalized * moveSpeed * Time.deltaTime;
          transform.Translate(move);

          // Actualizar la animación de movimiento (si es necesario)
          if (move.magnitude > 0)
          {
              animator.SetFloat("moveX", horizontal);
              animator.SetFloat("moveY", vertical);
              animator.SetBool("isMoving", true);
          }
          else
          {
              animator.SetBool("isMoving", false);
          }*/
    }

    protected override bool AttemptMove<T>(int xDir, int yDir)
    {
        RaycastHit2D hit;
        bool canMove = Move(xDir, yDir, out hit);

        if (hit.transform == null)
            return canMove;

        T hitComponent = hit.transform.GetComponent<T>();

        // Si no se puede mover, se llama a OnCantMove
        if (!canMove && hitComponent != null)
        {
            OnCantMove(hitComponent);

            // Detectamos la dirección de la colisión
            if (xDir != 0 && yDir == 0)  // Colisión en el eje horizontal (izquierda/derecha)
            {
                // Si colisiona horizontalmente, bloqueamos solo el movimiento horizontal.
                return CanMove(0, yDir);  // Intentar movimiento solo en el eje Y (arriba/abajo)
            }

            if (xDir == 0 && yDir != 0)  // Colisión en el eje vertical (arriba/abajo)
            {
                // Si colisiona verticalmente, bloqueamos solo el movimiento vertical.
                return CanMove(xDir, 0);  // Intentar movimiento solo en el eje X (izquierda/derecha)
            }
        }

        return canMove;

        //food--;
        /*
                base.AttemptMove<T>(xDir, yDir);
                RaycastHit2D hit;

                CheckIfGameOver();

                GameManager.instance.playersTurn = false;*/
    }

    //hauran de tenir lo de trigger activat
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

    protected override void OnCantMove<T>(T component)
    {
        Wall hitWall = component as Wall;
        hitWall.DamageWall(wallDamage);
        animator.SetTrigger("playerChop");
    }


    //Aquesta part igual sobraria pk es de RogueLike
    private void Restart()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void LoseCobre(int loss)
    {
        animator.SetTrigger("playerHit");
        cobre -= loss;
        CheckIfGameOver();
    }

    private void CheckIfGameOver()
    {
        if (cobre <= 0)
            GameManager.instance.GameOver();
    }
}

