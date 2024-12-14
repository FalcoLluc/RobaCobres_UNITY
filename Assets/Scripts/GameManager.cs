using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using System.Collections;


public class GameManager : MonoBehaviour
{
    //delay moviment enemics
    public float turnDelay = .2f;
    public static GameManager instance = null;
    private BoardManager boardScript;
    public int playerCobrePoints = 100;



    //despres cambio quan player tingui mecanica que toca
    [HideInInspector] public bool playersTurn = true;


    //CAMBIAR DESPRES
    private int level = 5;
    private List<Enemy> enemies;
    private bool enemiesMoving;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();
        InitGame();
    }

    void InitGame()
    {
        enemies.Clear();
        boardScript.SetupScene(level);
    }

    public void GameOver()
    {
        playersTurn = false;
        enabled = false;
    }

    void Update()
    {
        if (enemiesMoving)
            return;
        StartCoroutine(MoveEnemies());
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    IEnumerator MoveEnemies()
    {
        enemiesMoving = true;
        yield return new WaitForSeconds(turnDelay);
        if (enemies.Count == 0)
        {
            yield return new WaitForSeconds(turnDelay);
        }

        for (int i = 0; i < enemies.Count; i++)
        {
            enemies[i].MoveEnemy();
            yield return new WaitForSeconds(enemies[i].moveTime);
        }
        enemiesMoving = false;
    }

}

