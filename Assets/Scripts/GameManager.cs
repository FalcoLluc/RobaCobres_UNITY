using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public class GameManager : MonoBehaviour
{
    public float levelStartDelay = 2f;

    //delay moviment enemics
    //public float turnDelay = .2f;
    public static GameManager instance = null;
    private BoardManager boardScript;

    public int playerCobrePoints;
    public int playerCobreTotales;

    //despres cambio quan player tingui mecanica que toca
    //[HideInInspector] public bool playersTurn = true;


    //CAMBIAR DESPRES
    private Text levelText;
    private Text cobreText;
    private GameObject levelImage;
    private int level;
    private List<Enemy> enemies;
    private bool isGameOver = false; // Nuevo estado para Game Over
    //private bool enemiesMoving;

    private bool isGameWin = false;
    private bool doingSetup;

    private UnityToAndroidBridge unityToAndroidBridge;
    private bool initialScreenShown = false;
    private Dictionary<int, object[]> levelToInfoMap;

    public AudioClip R2;

    void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
        DontDestroyOnLoad(gameObject);
        enemies = new List<Enemy>();
        boardScript = GetComponent<BoardManager>();

        levelToInfoMap = new Dictionary<int, object[]>()
        {
            { 1, new object[] { "R2", new Color(0f, 0.5f, 0f) } },
            { 2, new object[] { "R4", Color.red } },
            { 3, new object[] { "R11", Color.blue } },
            { 4, new object[] { "R1", new Color(0.678f, 0.847f, 0.902f) } }, // Light blue
            { 5, new object[] { "R3", Color.red } }
        };
    }

    void Start()
    {
        //cobreText = GameObject.Find("CobreText").GetComponent<Text>();
        // Find the UnityToAndroidBridge script on an existing GameObject
        unityToAndroidBridge = FindFirstObjectByType<UnityToAndroidBridge>();
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


    private void OnLevelWasLoaded(int index)
    {
        //level++;
        InitGame();
    }


    private void HideLevelImage()
    {
        levelImage.SetActive(false);
        doingSetup = false;
    }

    public bool GetSetupState()
    {
        return doingSetup;
    }

    void InitGame()
    {
        doingSetup = true;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        cobreText = GameObject.Find("CobreText").GetComponent<Text>();
        //CANVIAR QUAN TINGUEM DIFERENTS NIVELLS
        levelText.text = "Rodalies " + (string)levelToInfoMap[level][0];
        levelImage.GetComponent<Image>().color = (Color)levelToInfoMap[level][1];
        levelImage.SetActive(true);
        playerCobrePoints = 0;
        playerCobreTotales = 0;
        enemies.Clear();
        Invoke("HideLevelImage", levelStartDelay);
        boardScript.BoardSetup(level);
        isGameOver = false;
        isGameWin = false;
    }

    public void InitGameContinue(string txt, int _level, int _cobreActual, int _cobreTotal)
    {
        doingSetup = true;
        level = _level;
        playerCobrePoints = _cobreActual;
        playerCobreTotales = _cobreTotal;
        levelImage = GameObject.Find("LevelImage");
        levelText = GameObject.Find("LevelText").GetComponent<Text>();
        cobreText = GameObject.Find("CobreText").GetComponent<Text>();
        //CANVIAR QUAN TINGUEM DIFERENTS NIVELLS
        levelText.text = "Rodalies " + (string)levelToInfoMap[level][0];
        levelImage.GetComponent<Image>().color = (Color)levelToInfoMap[level][1];
        levelImage.SetActive(true);
        Invoke("HideLevelImage", levelStartDelay);

        enemies.Clear();
        boardScript.BoardSetupString(txt, _level);
        isGameOver = false;
        isGameWin = false;
        //playerCobrePoints = 0;
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

        //levelText.text = "Game Over";
        //levelImage.SetActive(true);
        GameOverManager gameOver = FindFirstObjectByType<GameOverManager>();
        gameOver.ShowPanel();
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

    public void GameWin()
    {
        if (isGameWin) return; // Evita llamadas duplicadas

        isGameWin = true;
        Debug.Log("¡Te has pasado el nivel!");
        // Detener la lógica adicional si el juego ha terminado
        foreach (var enemy in enemies)
        {
            if (enemy != null)
                enemy.StopEnemy();
        }
        unityToAndroidBridge.SendAddCobre(playerCobrePoints);
        playerCobreTotales += playerCobrePoints;

        if (level == 5)
        {
            Debug.Log("¡Te has pasado el juego!");
            doingSetup = true;
            //CANVIAR QUAN TINGUEM DIFERENTS NIVELLS
            levelText.text = "Has conseguido cargarte la red de Rodalies, felicidades! \n\n Cobre Total Robado: " + playerCobreTotales;
            levelImage.GetComponent<Image>().color = new Color(1f, 0.5f, 0f, 1f);
            levelImage.SetActive(true);
            unityToAndroidBridge.SendAddPuntosTotales(playerCobreTotales);
        }
        else
        {
            level++;
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
    public void saveGame()
    {
        string levelStr = boardScript.SaveItemsState();
        unityToAndroidBridge.SendSaveGame(levelStr, level, playerCobrePoints, playerCobreTotales);
    }


    //GAME
    public void startLevel1()
    {
        level = 1;
        this.initialScreenShown = true;
        SoundManager.instance.PlaySingle(R2);
        InitGame();
    }

    public void continueGame()
    {
        //FER PETCIONS BBDD
        unityToAndroidBridge.RequestGame();
        this.initialScreenShown = true;
    }

    public bool IsInitialScreenShown()
    {
        return initialScreenShown;
    }

    public void actualizarTextoCobre()
    {
        if (playerCobrePoints >= 0)
        {
            cobreText.text = "Cobre:" + playerCobrePoints;
        }
        else
        {
            cobreText.text = "Cobre:" + 0;
        }
    }
}

