using UnityEngine;
using UnityEngine.UI;

public class InitialScreenManager : MonoBehaviour
{
    public GameObject initialImage;
    public Button startButton;
    public Button continueButton;

    void Start()
    {
        // Check if the initial screen has already been shown
        if (!GameManager.instance.IsInitialScreenShown())
        {
            // Assign functions to buttons
            startButton.onClick.AddListener(StartNewGame);
            continueButton.onClick.AddListener(ContinueGame);
            initialImage.SetActive(true);
        }
        else
        {
            // If the initial screen has already been shown, disable it
            initialImage.SetActive(false);
            this.enabled = false;
        }
    }

    void StartNewGame()
    {
        GameManager.instance.startLevel1();
        initialImage.SetActive(false);
        this.enabled = false;
    }

    void ContinueGame()
    {
        GameManager.instance.continueGame();
        initialImage.SetActive(false);
        this.enabled = false;
    }
}


