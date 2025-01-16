using UnityEngine;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuPanel;  // Reference to the Pause Menu panel
    private bool isPaused = false;

    // Method to be called by the Pause Button
    public void TogglePauseMenu()
    {
        if (isPaused)
        {
            ResumeGame();
        }
        else
        {
            PauseGame();
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the pause menu is hidden at the start
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
    }

    // Method to be called when the Pause button is pressed
    public void PauseGame()
    {
        // Display the pause menu panel
        pauseMenuPanel.SetActive(true);

        // Pause Unity's time and logic
        Time.timeScale = 0f;
    }

    // Method to be called when the Resume button is pressed
    public void ResumeGame()
    {
        // Hide the pause menu panel
        pauseMenuPanel.SetActive(false);

        // Resume Unity's time and logic
        Time.timeScale = 1f;
    }

    // Method to be called when the Save Game button is pressed
    public void SaveGame()
    {
        Debug.Log("Game state saved (implement your logic here).");
        GameManager.instance.saveGame();
        // You can add your own logic here to save the game's state
    }

    // Method to be called when the Exit Game button is pressed
    public void ExitGame()
    {
        // Call Android's method to exit Unity
        UnityToAndroidBridge unityToAndroidBridge = FindObjectOfType<UnityToAndroidBridge>();
        if (unityToAndroidBridge != null)
        {
            unityToAndroidBridge.CloseUnityApp();
        }
    }

    void Update()
    {
        // Pause game when the Escape key or back button is pressed
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }
}


