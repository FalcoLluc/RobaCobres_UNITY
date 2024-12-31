using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using System.Collections;


public class GameManager : MonoBehaviour
{
    //delay moviment enemics
    //public float turnDelay = .2f;
    public static GameManager instance = null;
    private BoardManager boardScript;


    public int playerCobrePoints = 100;



    //despres cambio quan player tingui mecanica que toca
    //[HideInInspector] public bool playersTurn = true;


    //CAMBIAR DESPRES
    private int level = 5;
    private List<Enemy> enemies;
    private bool isGameOver = false; // Nuevo estado para Game Over
    //private bool enemiesMoving;

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
        isGameOver = false;
    }

    public void GameOver()
    {
        if (isGameOver) return; // Evita llamadas duplicadas a GameOver

        isGameOver = true;
        Debug.Log("Game Over!");

        // Detener la lógica adicional si el juego ha terminado
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.StopEnemy();
        }
    }

    public void AddEnemyToList(Enemy script)
    {
        enemies.Add(script);
    }

    public void RemoveEnemy(Enemy script)
    {
        if (enemies.Contains(script))
            enemies.Remove(script);
    }
    void Update()
    {
        {
            // Puedes agregar lógica global para los enemigos aquí si es necesario.
            if (enemies.Count == 0)
                return;

            foreach (var enemy in enemies)
            {
                if (enemy != null)
                {
                    enemy.MoveEnemy();
                }
            }
        }
    }
}

