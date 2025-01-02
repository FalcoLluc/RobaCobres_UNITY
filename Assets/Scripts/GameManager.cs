using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using System.Collections;
using UnityEngine.UI;


public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;

    //delay moviment enemics
    //public float turnDelay = .2f;
    public static GameManager instance = null;
    private BoardManager boardScript;


    public int playerCobrePoints = 100;



    //despres cambio quan player tingui mecanica que toca
    //[HideInInspector] public bool playersTurn = true;


    //CAMBIAR DESPRES
    private Text levelText;
    private GameObject levelImage;
    private int level = 1;
    private List<Enemy> enemies;
    private bool isGameOver = false; // Nuevo estado para Game Over
    //private bool enemiesMoving;
    private bool doingSetup;

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


    private void OnLevelWasLoaded(int index)
    {
        level++;
        InitGame();
    }


    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        //CANVIAR QUAN TINGUEM DIFERENTS NIVELLS
        //levelText.text="Day "+level;
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.SetupScene(level);
        isGameOver = false;
    }

    public void GameOver()
    {
        if (isGameOver) return; // Evita llamadas duplicadas a GameOver

        isGameOver = true;
        Debug.Log("Game Over!");

        // Detener la l�gica adicional si el juego ha terminado
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.StopEnemy();
        }

        levelText.text = "Game Over";
        levelImage.SetActive(true);
        enabled = false;
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
            // Puedes agregar l�gica global para los enemigos aqu� si es necesario.
            if (enemies.Count == 0 || doingSetup)
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

