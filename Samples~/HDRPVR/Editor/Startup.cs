using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

[InitializeOnLoad]
public class Startup {

	//Only Load Once
    private static bool loaded = false;

    //On Startup Load the HD Render Pipeline
    static Startup() {
        if (!loaded) {
            Debug.Log("Loading High Definiton Render Pipeline Package from Startup script...");
            Client.Add("com.unity.render-pipelines.high-definition");
            loaded = true;
        }
    }
}
