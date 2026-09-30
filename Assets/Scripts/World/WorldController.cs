using UnityEngine;
using UnityEngine.InputSystem;

public class WorldController : MonoBehaviour
{
    public static WorldController Instance { get; private set; }

    public static bool isGamePaused { get; private set; }

    private CameraController cameraController;

    [Header("UI")] 
    [SerializeField] private GameObject pauseMenuUI;

    void Awake()
    {
        // Setup instance
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate WorldController Instance destroyed");
            
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    void Start()
    {
        cameraController = PlayerController.Instance.GetCameraController();
        
        LockMouse();
    }

    void Update()
    {
        Keyboard keyboard = Keyboard.current;

        if (keyboard.escapeKey.wasPressedThisFrame)
        {
            UpdatePause();
        }
    }

    /// <summary>
    /// Setting if the game is paused
    /// </summary>
    public void UpdatePause()
    {
        if (isGamePaused)
        {
            isGamePaused = false;
            LockMouse();
            cameraController.enabled = true;
            PlayerController.Instance.UpdatePause(false);
            pauseMenuUI.SetActive(false);
            
            return;
        }
        
        isGamePaused = true;
        ReleaseMouse();
        cameraController.enabled = false;
        PlayerController.Instance.UpdatePause(true);
        pauseMenuUI.SetActive(true);
    }

    /// <summary>
    /// Enable the cursor
    /// </summary>
    void ReleaseMouse()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    /// <summary>
    /// Disable the cursor
    /// </summary>
    void LockMouse()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    /// <summary>
    /// Set a custom time speed for the entire game
    /// </summary>
    /// <param name="timeScale">Game speed</param>
    void SetGameSpeed(float timeScale)
    {
        Time.timeScale = timeScale;
    }

    /// <summary>
    /// Reset the game time to the default value
    /// </summary>
    void ResetGameSpeed()
    {
        Time.timeScale = 1f;
    }

    /// <summary>
    /// Activate the game's pause menu
    /// </summary>
    /// <param name="toggle">True/False</param>
    void TogglePauseMenu(bool toggle)
    {
        
    }
}
