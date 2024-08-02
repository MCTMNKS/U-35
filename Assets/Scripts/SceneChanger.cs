using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Analytics;  // Import the Unity Analytics namespace
using System.Collections.Generic;

public class SceneChanger : MonoBehaviour
{
    // This allows you to set the scene name in the Unity editor.
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the character.
        if (other.gameObject.CompareTag("ExampleCharacter"))
        {
            LogSceneCompletion(sceneToLoad);
            LoadScene(sceneToLoad);
        }
    }

    private void LoadScene(string sceneName)
    {
        // Check if the scene is valid before trying to load it.
        if (string.IsNullOrEmpty(sceneName) || SceneUtility.GetBuildIndexByScenePath(sceneName) == -1)
        {
            Debug.LogError("Scene " + sceneName + " cannot be loaded. Check the scene name and make sure it's added to the build settings.");
            return;
        }

        SceneManager.LoadScene(sceneName);
    }

    private void LogSceneCompletion(string sceneName)
    {
        // Log a custom analytics event for scene completion
        Analytics.CustomEvent("scene_finished", new Dictionary<string, object>
        {
            { "scene_name", sceneName },
            { "timestamp", System.DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss") }
        });

        Debug.Log("Logged scene completion for: " + sceneName);
    }
}

