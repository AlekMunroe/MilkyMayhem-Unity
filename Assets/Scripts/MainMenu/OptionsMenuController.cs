using UnityEngine;

/// <summary>
/// Open display, open audio, open camera, open controls, open gameplay, close options, return to the main menu
/// </summary>
public class OptionsMenuController : MonoBehaviour
{
    [Header("Options Panels")]
    [SerializeField] private GameObject displayPanel;
    [SerializeField] private GameObject audioPanel;
    [SerializeField] private GameObject cameraPanel;
    [SerializeField] private GameObject controlsPanel;
    [SerializeField] private GameObject gameplayPanel;

    private void OnEnable()
    {
        OpenDisplayPanel();
    }

    public void OpenDisplayPanel()
    {
        OpenPanel(displayPanel);
    }

    public void OpenAudioPanel()
    {
        OpenPanel(audioPanel);
    }

    public void OpenCameraPanel()
    {
        OpenPanel(cameraPanel);
    }

    public void OpenControlsPanel()
    {
        OpenPanel(controlsPanel);
    }

    public void OpenGameplayPanel()
    {
        OpenPanel(gameplayPanel);
    }

    private void OpenPanel(GameObject panel)
    {
        if(panel == null)
        {
            Debug.LogError("OptionsMenuController: The panel is not assigned.");

            return;
        }

        CloseAllPanels();
        panel.SetActive(true);
    }

    private void CloseAllPanels()
    {
        SetPanelActive(displayPanel, false);
        SetPanelActive(audioPanel, false);
        SetPanelActive(cameraPanel, false);
        SetPanelActive(controlsPanel, false);
        SetPanelActive(gameplayPanel, false);
    }

    private void SetPanelActive(GameObject panel, bool isActive)
    {
        if(panel != null)
        {
            panel.SetActive(isActive);
        }
    }
}
