using UnityEngine;
using UnityEngine.Rendering;

public class Player : MovingObject
{
    public int wallDamage = 1;
    public int pointsPerCobre = 10;
    public int pointsPerCobreRajola = 20;
    public float restartLevelDelay = 1f;

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
        if (!GameManager.instance.playersTurn) return;

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
    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        //food--;

        base.AttemptMove<T>(xDir, yDir);
        RaycastHit2D hit;

        CheckIfGameOver();

        GameManager.instance.playersTurn = false;
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Furgo")
        {
            //FER COSES FURGO
            //Invoke("Restart", restartLevelDelay);
            //enabled = false;
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
        Application.LoadLevel(Application.loadedLevel);
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


