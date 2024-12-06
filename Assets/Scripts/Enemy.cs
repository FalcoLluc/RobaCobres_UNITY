using UnityEngine;

public class Enemy : MovingObject
{
    public int playerDamage;

    private Animator animator;
    private Transform target;
    private bool skipMove;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected override void Start()
    {
        animator = GetComponent<Animator>();
        target = GameObject.FindGameObjectWithTag("Player").transform;
        base.Start();
    }

    // Update is called once per frame
    void Update()
    {

    }

    protected override void AttemptMove<T>(int xDir, int yDir)
    {
        if (skipMove)
        {
            skipMove = false;
            return;
        }

        base.AttemptMove<T>(xDir, yDir);

        skipMove = true;
    }

    public void MoveEnemy()
    {
        int xDir = 0;
        int yDir = 0;
        bool horizontal = false;
        bool vertical = false;

        if (Mathf.Abs(target.position.x - transform.position.x) > float.Epsilon)
        {
            horizontal = true;
        }
        if (Mathf.Abs(target.position.y - transform.position.y) > float.Epsilon)
        {
            vertical = true;
        }

        if (vertical)
        {
            xDir = 0;
            yDir = target.position.y > transform.position.y ? 1 : -1;
            if (CanMove(xDir, yDir))
            {
                AttemptMove<Player>(xDir, yDir);
                return;
            }
        }

        if (horizontal)
        {
            xDir = target.position.x > transform.position.x ? 1 : -1;
            yDir = 0;

            // Flips sprite following horizontal direction
            if (xDir < 0)
            {
                //SI VOLEM QUE LANIMACIO GIRI
                //spriteRenderer.flipX = false;
            }
            else
            {
                //spriteRenderer.flipX = true;
            }


            if (CanMove(xDir, yDir))
            {
                AttemptMove<Player>(xDir, yDir);
                return;
            }

        }


    }
    protected override void OnCantMove<T>(T component)
    {
        Player hitPlayer = component as Player;
        animator.SetTrigger("enemyAttack");
        //SoundManager.instance.RandomizeSfx(enemyAttack1, enemyAttack2);
        hitPlayer.LoseFood(playerDamage);
    }
}
