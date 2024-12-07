using UnityEngine;
using System;
using System.Collections.Generic;
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

    // Definimos las variables del tablero
    public GameObject furgo;
    public GameObject[] tileTypes; // Los tipos de tiles (piso, pared, etc.)
    public GameObject[] cobreTiles; // Lugar donde estará el cobre
    public GameObject defaultTile;
    public GameObject[] enemyTiles; // Enemigos

    public GameObject player;

    private List<string> lines = new List<string>(); // Lista de líneas leídas desde el archivo
    private int rows; // Número de filas (cálculo dinámico)
    private int columns; // Número de columnas (cálculo dinámico)

    private Transform boardHolder; // Contenedor del tablero
    private List<Vector3> gridPositions = new List<Vector3>(); // Arreglo de posiciones de tiles

    // Método para cargar el archivo de texto y crear el tablero con formas no cuadradas
    void LoadBoardFromFile(string filePath)
    {
        lines.Clear();
        string[] allLines = File.ReadAllLines(filePath);

        // Almacenamos las líneas del archivo en la lista
        lines.AddRange(allLines);

        // Calculamos el número de filas y columnas (la longitud de cada fila puede ser diferente)
        rows = lines.Count;
        columns = lines[0].Length;

        // Si las demás líneas tienen diferente longitud, ajustamos las columnas al máximo
        foreach (var line in lines)
        {
            if (line.Length > columns)
            {
                columns = line.Length;
            }
        }

        // Invertir el orden de las filas para corregir la orientación de la Y (para que el origen esté en la parte superior)
        lines.Reverse();
    }

    // Método para convertir una letra en un índice de tile
    int GetTileIndexFromLetter(char tileChar)
    {
        // Convertimos la letra a mayúscula y verificamos que sea una letra válida
        tileChar = Char.ToUpper(tileChar);

        if (tileChar >= 'A' && tileChar <= 'Z')
        {
            return tileChar - 'A'; // Convierte la letra en un índice (A=0, B=1, ..., Z=25)
        }
        else
        {
            // Si el carácter no es una letra válida, retornamos un valor por defecto
            return -1; // Este índice puede corresponder a un tile de error o vacío
        }
    }

    void BoardSetup()
    {
        LoadBoardFromFile("Assets/MapaLayout/BoardLayout.txt"); // Cargamos el archivo .txt
        boardHolder = new GameObject("Board").transform;

        gridPositions.Clear();

        // Iteramos sobre las filas y columnas
        for (int y = 0; y < rows; y++) // Primero iteramos sobre las filas
        {
            for (int x = 0; x < columns; x++) // Después sobre las columnas
            {
                GameObject toInstantiate;
                char tileChar;

                // Comprobamos si la fila actual tiene suficientes columnas
                if (x < lines[y].Length)
                {
                    // Si hay suficientes columnas, obtenemos el carácter en la posición (x, y)
                    tileChar = lines[y][x];
                }
                else
                {
                    // Si no hay suficiente longitud en la fila, asignamos un carácter por defecto
                    tileChar = ' '; //No es valid, retornara -1 i li asignarem el default
                }

                // Convertimos el carácter en un índice de tile
                int tileIndex = GetTileIndexFromLetter(tileChar);

                if (tileIndex >= 0 && tileIndex < tileTypes.Length)
                {
                    toInstantiate = tileTypes[tileIndex]; // Instanciamos el tile correspondiente
                }
                else
                {
                    // Si el índice no es válido, usamos un tile vacío o por defecto
                    toInstantiate = defaultTile; // O cualquier tile por defecto
                }

                // Instanciamos el objeto en la posición (x, y)
                GameObject instance = Instantiate(toInstantiate, new Vector3(x, y, 0f), Quaternion.identity);
                instance.transform.SetParent(boardHolder);

                //VULL QUE NOMES HI HAGI A GRID POSITIONS els que no siguin Blocking, és a dir, Default
                if (instance.layer == LayerMask.NameToLayer("Default"))
                {
                    gridPositions.Add(new Vector3(x, y, 0f)); // Only add to gridPositions if it's on the "Default" layer
                }
            }
        }
    }


    // Método para crear objetos en posiciones aleatorias (extra)
    Vector3 RandomPosition()
    {
        int randomIndex = UnityEngine.Random.Range(0, gridPositions.Count);
        Vector3 randomPosition = gridPositions[randomIndex];
        gridPositions.RemoveAt(randomIndex);
        return randomPosition;
    }

    // Método para distribuir objetos aleatorios en el tablero
    void LayoutObjectAtRandom(GameObject[] tileArray, int minimum, int maximum)
    {
        int objectCount = UnityEngine.Random.Range(minimum, maximum + 1);
        for (int i = 0; i < objectCount; i++)
        {
            Vector3 randomPosition = RandomPosition();
            GameObject tileChoice = tileArray[UnityEngine.Random.Range(0, tileArray.Length)];
            Instantiate(tileChoice, randomPosition, Quaternion.identity);
        }
    }

    // Método que configura la escena y coloca enemigos y otros objetos
    public void SetupScene(int level)
    {
        BoardSetup(); // Configura el tablero
        int enemyCount = (int)Mathf.Log(level, 2f);
        LayoutObjectAtRandom(enemyTiles, enemyCount, enemyCount); // Coloca enemigos en el tablero

        //ES UNA PROVA NOMES
        LayoutObjectAtRandom(cobreTiles, 4, 4);

        //PLAYER:
        Vector3 randomPosition = RandomPosition();
        Instantiate(player, randomPosition, Quaternion.identity);

        //posariem la furgo on toca;
        randomPosition = RandomPosition();
        Instantiate(furgo, randomPosition, Quaternion.identity);

    }
}



