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
    public GameObject trainPrefab;
    public GameObject wallPrefab;
    public GameObject[] tileTypes; // Los tipos de tiles (piso, pared, etc.)
    public GameObject[] cobreTiles; // Lugar donde estará el cobre
    public GameObject defaultTile;
    public GameObject[] enemyTiles; // Enemigos

    public GameObject player;

    private List<string> lines = new List<string>(); // Lista de líneas leídas desde el archivo
    private List<string> linesItems = new List<string>(); // Lista de líneas items, player...
    private int rows; // Número de filas (cálculo dinámico)
    private int columns; // Número de columnas (cálculo dinámico)

    private Transform boardHolder; // Contenedor del tablero
    private List<Vector3> gridPositions = new List<Vector3>(); // Arreglo de posiciones de tiles

    // Método para cargar el archivo de texto y crear el tablero con formas no cuadradas
    void LoadBoardFromFile(string filePath)
    {
        lines.Clear();
        TextAsset boardData = Resources.Load<TextAsset>(filePath);
        try
        {
            lines.AddRange(boardData.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None));
        }
        catch (Exception e)
        {
            Debug.LogError("Error al cargar el archivo de tablero: " + e.Message);
        }

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

    // Método para cargar el archivo de objetos
    void LoadItemsFromFile(string filePath)
    {
        linesItems.Clear();
        TextAsset boardData = Resources.Load<TextAsset>(filePath);
        try
        {
            linesItems.AddRange(boardData.text.Split(new string[] { "\n", "\r\n" }, StringSplitOptions.None));
        }
        catch (Exception e)
        {
            Debug.LogError("Error al cargar el archivo de objetos: " + e.Message);
        }
        linesItems.Reverse();
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

    void BoardSetup(int level)
    {
        string boardPath = $"MapaLayout/BoardLayout{level}";
        string itemsPath = $"MapaLayout/ItemsLayout{level}";
        LoadBoardFromFile(boardPath);
        LoadItemsFromFile(itemsPath);

        // Verifica si los arreglos necesarios están asignados
        if (tileTypes == null || tileTypes.Length == 0)
        {
            Debug.LogError("tileTypes no está asignado o está vacío.");
            return;
        }

        if (cobreTiles == null || cobreTiles.Length == 0)
        {
            Debug.LogError("cobreTiles no está asignado o está vacío.");
            return;
        }

        if (enemyTiles == null || enemyTiles.Length == 0)
        {
            Debug.LogError("enemyTiles no está asignado o está vacío.");
            return;
        }

        if (player == null)
        {
            Debug.LogError("El objeto player no está asignado.");
            return;
        }

        boardHolder = new GameObject("Board").transform;
        gridPositions.Clear();

        // PART TILES
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

                // Vull que només hi hagi a gridPositions els que no siguin Blocking, és a dir, Default
                if (instance.layer == LayerMask.NameToLayer("Default"))
                {
                    gridPositions.Add(new Vector3(x, y, 0f)); // Only add to gridPositions if it's on the "Default" layer
                }
            }
        }

        // PART ITEMS, ENEMICS
        for (int y = 0; y < linesItems.Count; y++) // Iteramos sobre las filas de items
        {
            for (int x = 0; x < linesItems[y].Length; x++) // Iteramos sobre las columnas
            {
                char itemChar = linesItems[y][x]; // Obtenemos el carácter en la posición (x, y)

                switch (itemChar)
                {
                    case 'P':
                        {
                            GameObject playerInstance = Instantiate(player, new Vector3(x, y, 0f), Quaternion.identity); // Instancia el jugador
                            CameraFollow cameraFollow = Camera.main.GetComponent<CameraFollow>();
                            if (cameraFollow != null)
                            {
                                cameraFollow.target = playerInstance.transform;
                            }
                            break;
                        }
                    case 'E':
                        Instantiate(enemyTiles[UnityEngine.Random.Range(0, enemyTiles.Length)], new Vector3(x, y, 0f), Quaternion.identity); // Instancia un enemigo
                        break;
                    case 'C':
                        Instantiate(cobreTiles[UnityEngine.Random.Range(0, cobreTiles.Length)], new Vector3(x, y, 0f), Quaternion.identity); // Instancia el cobre
                        break;
                    case 'F':
                        Instantiate(furgo, new Vector3(x, y, 0f), Quaternion.identity); // Instancia la furgo
                        break;
                    case 'T':
                        Instantiate(trainPrefab, new Vector3(x, y, 0f), Quaternion.identity); // Instancia el tren
                        break;
                    case 'W':
                        Instantiate(wallPrefab, new Vector3(x, y, 0f), Quaternion.identity); // Instancia el wall
                        break;
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

    //DESPRES TREURE LO DE LEVEL PK ELS CLIENTS I TOT DEPENDRA DEL TXT, no del int level, idem GameManager
    public void SetupScene(int level)
    {
        BoardSetup(level); // Configura el tablero
        /*
        //PLAYER: posariem player on toca
        Vector3 randomPosition = RandomPosition();
        // Instanciar el Player
        GameObject playerInstance = Instantiate(player, randomPosition, Quaternion.identity);




        //posariem la furgo on toca;
        randomPosition = RandomPosition();
        Instantiate(furgo, randomPosition, Quaternion.identity);

        //posariem el tren on toca
        Instantiate(trainPrefab, new Vector3(1, 5, 0), Quaternion.identity);
        */
    }

    public string SaveItemsState()
    {
        // Ruta para guardar el archivo de `ItemsLayout`
        //string itemsPath = $"MapaLayout/ItemsLayout{level}.txt";

        // Crear un array bidimensional para `ItemsLayout`
        char[,] itemsArray = new char[rows, columns];

        // Inicializar el array con el carácter por defecto ('-')
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                itemsArray[y, x] = '-'; // Fondo de los ítems
            }
        }

        // Rellenar el array de `ItemsLayout` según los ítems en el mapa
        GameObject[] allObjects = FindObjectsOfType<GameObject>();
        foreach (GameObject obj in allObjects)
        {
            Vector3 position = obj.transform.position;
            int x = Mathf.RoundToInt(position.x);
            int y = Mathf.RoundToInt(position.y);

            // Ignorar posiciones fuera del rango
            if (x < 0 || x >= columns || y < 0 || y >= rows) continue;

            // Asignar el carácter correspondiente en función de la etiqueta
            if (obj.CompareTag("Player"))
            {
                itemsArray[y, x] = 'P'; // Jugador
            }
            else if (obj.CompareTag("Enemy"))
            {
                itemsArray[y, x] = 'E'; // Enemigo
            }
            else if (obj.CompareTag("Cobre"))
            {
                itemsArray[y, x] = 'C'; // Cobre
            }
            else if (obj.CompareTag("Furgo"))
            {
                itemsArray[y, x] = 'F'; // Furgo
            }
            else if (obj.CompareTag("Train"))
            {
                itemsArray[y, x] = 'T'; // Tren
            }
            else if (obj.CompareTag("Fence"))
            {
                itemsArray[y, x] = 'W'; // Pared
            }
        }

        // Crear una lista de líneas para el archivo `ItemsLayout`
        List<string> itemsLines = new List<string>();

        // Generar las líneas de `ItemsLayout`
        for (int y = rows - 1; y >= 0; y--) // Invertir las filas para el formato correcto
        {
            string itemsLine = "";
            for (int x = 0; x < columns; x++)
            {
                itemsLine += itemsArray[y, x];
            }
            itemsLines.Add(itemsLine);
        }



        string layoutData = string.Join("\n", itemsLines);
        Debug.Log(layoutData);
        return layoutData;
        // Guardar el archivo en el directorio del proyecto
        //File.WriteAllLines(Application.dataPath + "/" + itemsPath, itemsLines);

        //Debug.Log($"Estado de los ítems guardado con éxito en: {itemsPath}");
    }
}