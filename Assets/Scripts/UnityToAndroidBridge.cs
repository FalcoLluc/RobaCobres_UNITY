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

    public void SendAddPuntosTotales(int cobre)
    {
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            // Get the current activity (UnityHostActivity)
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Call the sendStateToServer method exposed by UnityHostActivity
            currentActivity.Call("sendAddPuntosTotales", cobre);
        }
    }

    public void SendSaveGame(string gamestring, int level, int cobreActual, int cobreTotal)
    {
        // Create an AndroidJavaClass object that references the ServiceBBDD class
        using (AndroidJavaClass unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer"))
        {
            // Get the current activity (UnityHostActivity)
            AndroidJavaObject currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");

            // Call the sendStateToServer method exposed by UnityHostActivity
            currentActivity.Call("sendSaveGame", gamestring, level, cobreActual, cobreTotal);
        }
    }

    //FALTA IMPLEMENTAR A ANDROID
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

    public void OnLevelResponse(string combinedMessage)
    {
        string[] parts = combinedMessage.Split('|');

        // Extract the levelstring and level
        string levelstring = parts[0];
        int level = int.Parse(parts[1]);
        int cobreActual = int.Parse(parts[2]);
        int cobreTotal = int.Parse(parts[3]);
        Debug.Log("Received response from server: " + levelstring + "Received Level: " + level);
        GameManager.instance.InitGameContinue(levelstring, level, cobreActual, cobreTotal);
    }

    public void OnLevelResponseNoExisting()
    {
        Debug.Log("Received response from server: Not Found");
        GameManager.instance.startLevel1();
    }
}

