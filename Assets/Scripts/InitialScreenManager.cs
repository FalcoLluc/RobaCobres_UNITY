using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class InitialScreenManager : MonoBehaviour
{
    public GameObject initialImage;
    public Button startButton; // Botón para comenzar desde el nivel 1
    public Button continueButton; // Botón para continuar desde el nivel guardado

    void Start()
    {
        // Asignar las funciones a los botones
        startButton.onClick.AddListener(StartNewGame);
        continueButton.onClick.AddListener(ContinueGame);
        //initialImage.SetActive(true);
    }

    // Comenzar un nuevo juego desde el nivel 1
    void StartNewGame()
    {
        GameManager.instance.startLevel1();
        initialImage.SetActive(false);
    }

    // Continuar desde el último nivel guardado
    void ContinueGame()
    {
        GameManager.instance.continueGame();
        initialImage.SetActive(false);
    }
}

