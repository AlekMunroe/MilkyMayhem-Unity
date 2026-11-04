using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    [SerializeField] private int mainMenuSceneIndex;

    [Header("Panels")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private GameObject confirmMainMenuPanel;

    public void ReturnToGameButton()
    {
        WorldController.Instance.UpdatePause();
    }

    private void OnEnable()
    {
        ToggleConfirmReturnUI(false);
    }

    /// <summary>
    /// Toggle the "Are you sure you want to return to main menu" panel
    /// </summary>
    /// <param name="toggle">Enable the confirmReturnUI panel</param>
    public void ToggleConfirmReturnUI(bool toggle)
    {
        mainPanel.SetActive(!toggle);
        confirmMainMenuPanel.SetActive(toggle);
    }
    
    /// <summary>
    /// Load the main menu based on the mainMenuSceneIndex
    /// </summary>
    public void LoadMainMenuScene()
    {
        SceneManager.LoadScene(mainMenuSceneIndex);
    }
}
