using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine.InputSystem;

/// <summary>
/// Open manin menu, open options, open scores, open credits, start the game, quit the gmae, close secondary menus
/// </summary>
public class MainMenuController : MonoBehaviour
{
    [Header("Menu Panels")]
    [SerializeField] private GameObject optionsMenuPanel;
    [SerializeField] private GameObject scoresMenuPanel;
    [SerializeField] private GameObject creditsMenuPanel;
    [SerializeField] private GameObject quitConfirmationPanel;

    [Header("Scenes")]
    [SerializeField] private string gameplaySceneName = "MainGame";

    [Header("Build Information")]
    [SerializeField] private TMP_Text gameVersionText;

    [Header("Developmenr")]
    [SerializeField] private GameObject devMenuButton;
    [SerializeField] private GameObject devMenuPanel;

    void Start()
    {
        CloseAllSecondaryPanels();

        UpdateGameVersionText();

        ResetGameSpeed();

        devMenuButton.SetActive(false);

#if UNITY_EDITOR || DEVELOPMENT_BUILD
        DEV_EnableDevButton();
#endif
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if(keyboard == null)
        {
            Debug.LogError("MainMenuController: Cannot find keyboard.");

            return;
        }

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            CloseAllSecondaryPanels();
        }
    }

    public void OpenOptionsMenu()
    {
        OpenPanel(optionsMenuPanel);
    }

    public void OpenScoresMenu()
    {
        OpenPanel(scoresMenuPanel);
    }

    public void OpenCreditsMenu()
    {
        OpenPanel(creditsMenuPanel);
    }

    public void OpenQuitConfirmation()
    {
        OpenPanel(quitConfirmationPanel);
    }

    public void CancelQuit()
    {
        SetPanelActive(quitConfirmationPanel, false);
    }

    public void StartGame()
    {
        if (!Application.CanStreamedLevelBeLoaded(gameplaySceneName))
        {
            Debug.LogError("MainMenuController: The gameplay scene cannot load.");

            return;
        }

        SceneManager.LoadScene(gameplaySceneName);
    }

    public void ConfirmQuit()
    {
        Debug.Log("MainMenuController: Quitting game");

        Application.Quit();
    }

    public void CloseAllSecondaryPanels()
    {
        SetPanelActive(optionsMenuPanel, false);
        SetPanelActive(scoresMenuPanel, false);
        SetPanelActive(creditsMenuPanel, false);
        SetPanelActive(quitConfirmationPanel, false);
    }

    private void OpenPanel(GameObject panel)
    {
        if(panel == null)
        {
            Debug.LogError("MainMenuController: The panel is not assigned.");

            return;
        }

        CloseAllSecondaryPanels();
        panel.SetActive(true);
    }

    private void SetPanelActive(GameObject panel, bool isActive)
    {
        if(panel != null)
        {
            panel.SetActive(isActive);
        }
    }

    private void UpdateGameVersionText()
    {
        if(gameVersionText == null)
        {
            Debug.LogError("MainMenuController: Game version text is not assigned");

            return;
        }

        gameVersionText.text = "v" + Application.version;

        if (Debug.isDebugBuild)
        {
            gameVersionText.text += "\nDevelopment Build";
        }
    }

    public void DEV_OpenDevMenu()
    {
        CloseAllSecondaryPanels();

        devMenuPanel.SetActive(true);
    }

    private void DEV_EnableDevButton()
    {
        devMenuButton.SetActive(true);
    }

    public void DEV_ClearAllPlayerprefs()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();

        Application.Quit();
    }

    /// <summary>
    /// Reset the game time to the default value
    /// </summary>
    void ResetGameSpeed()
    {
        Time.timeScale = 1f;
    }
}
