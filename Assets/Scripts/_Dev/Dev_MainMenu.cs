using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Dev_MainMenu : MonoBehaviour
{
    public string[] devSceneNames;
    void Start()
    {
        foreach (string devSceneName in devSceneNames)
        {
            if (!IsSceneInBuildSettings(devSceneName))
            {
                Debug.LogWarning("Dev_MainMenu: Scene " + devSceneName + " Does not exist. The button will not work.");
            }
        }
    }

    /// <summary>
    /// Check if a required scene is setup in build settings
    /// </summary>
    /// <param name="sceneName"></param>
    /// <returns></returns>
    bool IsSceneInBuildSettings(string sceneName)
    {
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string scenePath = SceneUtility.GetScenePathByBuildIndex(i);
            string extractedPath = Path.GetFileNameWithoutExtension(scenePath);

            if (extractedPath == sceneName)
            {
                return true;
            }
        }

        return false;
    }
    
    /// <summary>
    /// Load a specified scene by its name
    /// </summary>
    /// <param name="sceneName"></param>
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
}
