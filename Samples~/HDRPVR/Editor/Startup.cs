using UnityEngine;
using UnityEditor;
using UnityEditor.PackageManager;

[InitializeOnLoad]
public class Startup {

    private static bool loaded = false;

    static Startup() {
        if (!loaded) {
            Debug.Log("Loading High Definiton Render Pipeline Package from Startup script...");
            Client.Add("com.unity.render-pipelines.high-definition");
            loaded = true;
        }
    }
}
