using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverManager : MonoBehaviour
{
    public GameObject gameOverMenuPanel;  // Reference to the Pause Menu panel

    // Start is called before the first frame update
    void Start()
    {
        // Ensure the pause menu is hidden at the start
        if (gameOverMenuPanel != null)
        {
            gameOverMenuPanel.SetActive(false);
        }
    }


    public void ShowPanel()
    {
        // Display the pause menu panel
        SoundManager.instance.musicSource.Stop();
        gameOverMenuPanel.SetActive(true);

    }

    public void RestartGame()
    {
        Debug.Log("Restart Level");
        SoundManager.instance.musicSource.Play();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        // You can add your own logic here to save the game's state
    }

    // Method to be called when the Exit Game button is pressed
    public void ExitGame()
    {
        // Call Android's method to exit Unity
        UnityToAndroidBridge unityToAndroidBridge = FindFirstObjectByType<UnityToAndroidBridge>();
        if (unityToAndroidBridge != null)
        {
            unityToAndroidBridge.CloseUnityApp();
        }
    }
}
