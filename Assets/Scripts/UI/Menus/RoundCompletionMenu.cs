using UnityEngine;
using UnityEngine.SceneManagement;

public class RoundCompletionMenu : MonoBehaviour
{
    [SerializeField] private int mainMenuSceneIndex;

    /// <summary>
    /// Loads the current scene
    /// </summary>
    public void RestartGame()
    {
        Scene currentScene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.buildIndex);
    }

    /// <summary>
    /// Load the main menu based on the mainMenuSceneIndex
    /// </summary>
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
}
