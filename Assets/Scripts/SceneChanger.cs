using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneChanger : MonoBehaviour
{
    // This allows you to set the scene name in the Unity editor.
    public string sceneToLoad;

    private void OnTriggerEnter(Collider other)
    {
        // Check if the colliding object is the character.
        if (other.gameObject.tag == "ExampleCharacter")
        {
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
}

