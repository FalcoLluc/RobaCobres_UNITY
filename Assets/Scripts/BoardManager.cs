using UnityEngine;
using System;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using System.IO;

public class BoardManager : MonoBehaviour
{
    [Serializable]
    public class Count
    {
        public int minimum;
        public int maximum;
        public Count(int min, int max)
        {
            minimum = min;
            maximum = max;
        }
    }
    public GameObject[] tileTypes;
    public int columns = 4;  // Columnas del tablero
    public int rows = 4;     // Filas del tablero

    public Count wallCount = new Count(5, 9);
    public Count foodCount = new Count(1, 5);
    public GameObject exit;
    public GameObject[] floorTiles;
    public GameObject[] foodTiles;
    public GameObject[] wallTiles;
    public GameObject[] outerWallTiles;
    public GameObject[] enemyTiles;

    private Transform boardHolder;
    private List<Vector3> gridPositions = new List<Vector3>();  // Arreglo de los diferentes tipos de tiles (piso, pared, etc.)

    //AIXO PER CARREGAR
    void Start()
    {
        // Cargar el tablero desde un archivo .txt
        SetupBoardFromFile("Assets/BoardLayout.txt");
    }

    void Update()
    {

    }

    // Método para cargar el archivo de texto que representa el tablero
    int[,] LoadBoardFromFile(string filePath)
    {
        // Lee todas las líneas del archivo .txt
        string[] lines = File.ReadAllLines(filePath);

        // Inicializamos un array 2D para representar el tablero
        int[,] board = new int[columns, rows];

        // Procesa cada línea y cada carácter
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                // Convertimos cada número del archivo a un entero que representa un tile
                board[x, y] = int.Parse(lines[y][x].ToString());
            }
        }

        return board;
    }

    // Método para configurar el tablero basado en el archivo cargado
    void BoardSetupFromData(int[,] boardData)
    {
        boardHolder = new GameObject("Board").transform;

        for (int x = 0; x < columns; x++)
        {
            for (int y = 0; y < rows; y++)
            {
                // Obtenemos el índice del tile correspondiente del archivo
                int tileIndex = boardData[x, y];

                // Instanciamos el GameObject correspondiente basado en el valor del tile
                GameObject toInstantiate = tileTypes[tileIndex];

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    // Método público que puedes llamar para inicializar el tablero
    public void SetupBoardFromFile(string filePath)
    {
        // Cargamos los datos del archivo
        int[,] boardData = LoadBoardFromFile(filePath);

        // Configuramos el tablero basado en los datos del archivo
        BoardSetupFromData(boardData);
    }

    //ORIGINAL

    void InitialiseList()
    {
        gridPositions.Clear();
        for (int x = 1; x < columns - 1; x++)
        {
            for (int y = 1; y < rows - 1; y++)
            {
                gridPositions.Add(new Vector3(x, y, 0f));
            }
        }
    }

    void BoardSetup()
    {
        boardHolder = new GameObject("Board").transform;
        for (int x = -1; x < columns + 1; x++)
        {
            for (int y = -1; y < rows + 1; y++)
            {
                GameObject toInstantiate = floorTiles[Random.Range(0, floorTiles.Length)];

                if (x == -1 || x == columns || y == -1 || y == rows)
                {
                    toInstantiate = outerWallTiles[Random.Range(0, outerWallTiles.Length)];
                }

                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity) as GameObject;
                instance.transform.SetParent(boardHolder);
            }
        }
    }

    Vector3 RandomPosition()
    {
        int randomIndex = Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    public void SetupScene(int level)
    {
        BoardSetup();
        InitialiseList();
        LayoutObjectAtRandom(wallTiles, wallCount.minimum, wallCount.maximum);
        LayoutObjectAtRandom(foodTiles, foodCount.minimum, foodCount.maximum);
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount);
        Instantiate(exit, new Vector3(columns - 1, rows - 1, 0f), Quaternion.identity);
    }

}


