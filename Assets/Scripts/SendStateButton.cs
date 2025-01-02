using UnityEngine;
using UnityEngine.UI;  // For interacting with UI components

public class SendStateButton : MonoBehaviour
{
    public Button sendStateButton; // Button reference
    private UnityToAndroidBridge unityToAndroidBridge; // Reference to the Unity-Android bridge

    void Start()
    {
        // Get the button component if not set
        if (sendStateButton == null)
        {
            sendStateButton = GetComponent<Button>();
        }

        // Find the UnityToAndroidBridge script on an existing GameObject
        unityToAndroidBridge = FindObjectOfType<UnityToAndroidBridge>();

        // Add listener to the button click event
        sendStateButton.onClick.AddListener(OnSendStateButtonClicked);
    }

    // Method to handle the button click
    private void OnSendStateButtonClicked()
    {
        if (unityToAndroidBridge != null)
        {
            // Prepare the state as a string (e.g., ".txt" content or game state)
            string itemsStateText = "HOLAA"; // Example data

            // Send the state to the Android backend through UnityToAndroidBridge
            unityToAndroidBridge.SendItemsStateToServer(itemsStateText);
        }
        else
        {
            Debug.LogError("UnityToAndroidBridge is not found!");
        }
    }
}

