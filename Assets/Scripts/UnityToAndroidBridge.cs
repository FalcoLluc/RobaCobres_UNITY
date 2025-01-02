using UnityEngine;
using UnityEngine.UI;

public class UnityToAndroidBridge : MonoBehaviour
{
    private AndroidJavaObject serviceBBDD;
    public Text messageText;

    void Start()
    {
        messageText = GameObject.Find("MessageText").GetComponent<Text>();
    }

    // This method will be called to send the current state of items to Android
    public void SendItemsStateToServer(string itemsStateText)
    {
        // Create an AndroidJavaClass object that references the ServiceBBDD class
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            // Get the current activity (UnityHostActivity)
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Call the sendStateToServer method exposed by UnityHostActivity
            currentActivity.Call("sendStateToServer", itemsStateText);
        }
    }

    // This method is called by Android when the callback is received with the response from the server
    public void OnServerResponse(string response)
    {
        // Handle the server response here, for example display it in a UI
        Debug.Log("Received response from server: " + response);
        messageText.text = response;
    }
}

