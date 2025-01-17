using UnityEngine;
using UnityEngine.UI;

public class UnityToAndroidBridge : MonoBehaviour
{
    private AndroidJavaObject serviceBBDD;
    //public Text messageText;

    void Start()
    {
        // messageText = GameObject.Find("MessageText").GetComponent<Text>();
    }

    // Method to close the Unity application
    public void CloseUnityApp()
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
            currentActivity.Call("exitUnity");
        }
    }

    //PETICIONS
    // EXEMPLE HOLA
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

    // EXEMPLE CALLBACK
    public void OnServerResponse(string response)
    {
        // Handle the server response here, for example display it in a UI
        Debug.Log("Received response from server: " + response);
        //messageText.text = response;
    }

    public void SendSaveGame(string gamestring)
    {
        // Create an AndroidJavaClass object that references the ServiceBBDD class
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            // Get the current activity (UnityHostActivity)
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Call the sendStateToServer method exposed by UnityHostActivity
            currentActivity.Call("sendSaveGame", gamestring);
        }
    }

    public void RequestGame()
    {
        // Create an AndroidJavaClass object that references the ServiceBBDD class
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            // Get the current activity (UnityHostActivity)
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Call the sendStateToServer method exposed by UnityHostActivity
            currentActivity.Call("requestGame");
        }
    }

    public void SendAddCobre(int cobre)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            // Get the current activity (UnityHostActivity)
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Call the sendStateToServer method exposed by UnityHostActivity
            currentActivity.Call("sendAddCobre", cobre);
        }
    }
}

