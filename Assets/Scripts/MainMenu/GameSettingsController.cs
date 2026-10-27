using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Audio;
using NUnit.Framework;
using System.Collections.Generic;

/// <summary>
/// Resolution, full screen VSync, frame rate limit, colume, mouse sensitivity, FOV, gameplay preferences, apply, cancel, restore, load saved settings
/// </summary>
public class GameSettingsController : MonoBehaviour
{
    [Header("Display Settings")]
    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private TMP_Dropdown displayModeDropdown;
    [SerializeField] private TMP_Dropdown frameRateDropdown;
    [SerializeField] private Toggle vSyncToggle;

    [Header("Audio Settings")]
    [SerializeField] private AudioMixer audioMixer;
    [SerializeField] private Slider masterVolumeSlider;
    [SerializeField] private Slider musicVolumeSlider;
    [SerializeField] private Slider soundEffectsVolumeSlider;

    [Header("Camera Settings")]
    [SerializeField] private Slider mouseSensitivitySlider;
    [SerializeField] private Slider fieldOfViewSlider;
    [SerializeField] private Slider cameraShakeSlider;
    [SerializeField] private Toggle invertVerticalCameraToggle;

    [Header("Gameplay Settings")]
    [SerializeField] private Toggle controlHintsToggle;
    [SerializeField] private Toggle pauseWhenUnfocusedToggle;

    private readonly List<Vector2Int> availableResolutions = new();

    private readonly int[] availableFrameRates = { 30, 60, 120, 144, -1 };

    // Strings used in PlayerPrefs
    private const string ResolutionWidthKey = "ResolutionWidth";
    private const string ResolutionHeightKey = "ResolutionHeight";
    private const string DisplayModeKey = "DisplayMode";
    private const string FrameRateKey = "FrameRate";
    private const string VSyncKey = "VSync";

    private const string MasterVolumeKey = "MasterVolume";
    private const string MusicVolumeKey = "MusicVolume";
    private const string SoundEffectsVolumeKey = "SoundEffectsVolume";

    private const string MouseSensitivityKey = "MouseSensitivity";
    private const string FieldOfViewKey = "FieldOfView";
    private const string CameraShakeKey = "CameraShake";
    private const string InvertVerticalCameraKey = "InvertVerticalCamera";

    private const string ControlHintsKey = "ControlHints";
    private const string PauseWhenUnfocusedKey = "PauseWhenUnfocused";

    // The saved values
    public static float MouseSensitivity => PlayerPrefs.GetFloat(MouseSensitivityKey, 0.1f);
    public static float FieldOfView => PlayerPrefs.GetFloat(FieldOfViewKey, 75f);
    public static float CameraShakeStrength => PlayerPrefs.GetFloat(CameraShakeKey);

    public static bool InvertVerticalCamera => PlayerPrefs.GetInt(InvertVerticalCameraKey, 0) == 1;
    public static bool ShowControlsHint => PlayerPrefs.GetInt(ControlHintsKey, 1) == 1;
    public static bool PuaseWhenUnfocused => PlayerPrefs.GetInt(PauseWhenUnfocusedKey, 1) == 1;

    void Start()
    {
        SetupDisplayModeDropdown();
        SetupResolutionDropdown();
        SetupFrameRateDropdown();

        LoadSettings();
    }

    public void ApplySettings()
    {
        ApplyCurrentSettings(true);
    }

    public void CancelChanges()
    {
        LoadSettings();
        this.GetComponent<MainMenuController>().CloseAllSecondaryPanels();
    }

    /// <summary>
    /// Restores and saves the default settings
    /// </summary>
    public void RestoreDefaults()
    {
        SelectCurrentResolution();

        displayModeDropdown.SetValueWithoutNotify(0);
        frameRateDropdown.SetValueWithoutNotify(4);
        vSyncToggle.SetIsOnWithoutNotify(true);

        masterVolumeSlider.SetValueWithoutNotify(1f);
        musicVolumeSlider.SetValueWithoutNotify(1f);
        soundEffectsVolumeSlider.SetValueWithoutNotify(1f);

        mouseSensitivitySlider.SetValueWithoutNotify(0.1f);
        fieldOfViewSlider.SetValueWithoutNotify(75f);
        cameraShakeSlider.SetValueWithoutNotify(1f);
        invertVerticalCameraToggle.SetIsOnWithoutNotify(false);

        controlHintsToggle.SetIsOnWithoutNotify(true);
        pauseWhenUnfocusedToggle.SetIsOnWithoutNotify(true);

        ApplyCurrentSettings(true);
    }

    /// <summary>
    /// Creates the display mode dropdown options
    /// </summary>
    private void SetupDisplayModeDropdown()
    {
        displayModeDropdown.ClearOptions();

        displayModeDropdown.AddOptions(new List<string> { "Borderless Fullscreen", "Exclusive Fullscreen", "Windowed" });
    }

    /// <summary>
    /// Find the screen resolutions that can be used
    /// </summary>
    private void SetupResolutionDropdown()
    {
        resolutionDropdown.ClearOptions();
        availableResolutions.Clear();

        List<string> resolutionNames = new();

        foreach(Resolution resolution in Screen.resolutions)
        {
            Vector2Int resolutionSize = new Vector2Int(resolution.width, resolution.height);

            if (availableResolutions.Contains(resolutionSize))
            {
                continue;
            }

            availableResolutions.Add(resolutionSize);

            resolutionNames.Add(resolution.width + " x " + resolution.height);
        }

        if(availableResolutions.Count == 0)
        {
            Vector2Int currentResolution = new Vector2Int(Screen.currentResolution.width, Screen.currentResolution.height);

            availableResolutions.Add(currentResolution);

            resolutionNames.Add(currentResolution.x + " x " + currentResolution.y);
        }

        resolutionDropdown.AddOptions(resolutionNames);
    }

    /// <summary>
    /// Creates the frame rate dropdown options
    /// </summary>
    private void SetupFrameRateDropdown()
    {
        frameRateDropdown.ClearOptions();

        frameRateDropdown.AddOptions(new List<string> { "30 FPS", "60 FPS", "120 FPS", "144 FPS", "Unlimited" });
    }

    /// <summary>
    /// Loads all the saved settings into the UI
    /// </summary>
    private void LoadSettings()
    {
        int savedWidth = PlayerPrefs.GetInt(ResolutionWidthKey, Screen.currentResolution.width);
        int savedHeight = PlayerPrefs.GetInt(ResolutionHeightKey, Screen.currentResolution.height);
        int resolutionIndex = FindResolutionIndex(savedWidth, savedHeight);

        resolutionDropdown.SetValueWithoutNotify(resolutionIndex);

        displayModeDropdown.SetValueWithoutNotify(PlayerPrefs.GetInt(DisplayModeKey, 0));
        frameRateDropdown.SetValueWithoutNotify(FindFrameRateIndex(PlayerPrefs.GetInt(FrameRateKey, 01)));
        vSyncToggle.SetIsOnWithoutNotify(PlayerPrefs.GetInt(VSyncKey, 1) == 1);
        masterVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(MasterVolumeKey, 1f));
        musicVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(MusicVolumeKey, 1f));
        soundEffectsVolumeSlider.SetValueWithoutNotify(PlayerPrefs.GetFloat(SoundEffectsVolumeKey, 1f));
        mouseSensitivitySlider.SetValueWithoutNotify(MouseSensitivity);
        fieldOfViewSlider.SetValueWithoutNotify(FieldOfView);
        cameraShakeSlider.SetValueWithoutNotify(CameraShakeStrength);
        invertVerticalCameraToggle.SetIsOnWithoutNotify(InvertVerticalCamera);
        controlHintsToggle.SetIsOnWithoutNotify(ShowControlsHint);
        pauseWhenUnfocusedToggle.SetIsOnWithoutNotify(PuaseWhenUnfocused);

        ApplyCurrentSettings(false);
    }

    /// <summary>
    /// Save the current settings in the menu
    /// </summary>
    /// <param name="saveSettings">Should the settings be saved</param>
    private void ApplyCurrentSettings(bool saveSettings)
    {
        int resolutionIndex = Mathf.Clamp(resolutionDropdown.value, 0, availableResolutions.Count - 1);

        Vector2Int selectedResolution = availableResolutions[resolutionIndex];

        FullScreenMode displayMode = GetDisplayMode(displayModeDropdown.value);

        Screen.SetResolution(selectedResolution.x, selectedResolution.y, displayMode);

        QualitySettings.vSyncCount = vSyncToggle.isOn ? 1 : 0;

        int frameRate = availableFrameRates[frameRateDropdown.value];

        Application.targetFrameRate = vSyncToggle.isOn ? -1 : frameRate;

        SetMixerVolume("MasterVolume", masterVolumeSlider.value);
        SetMixerVolume("MusicVolume", musicVolumeSlider.value);
        SetMixerVolume("SoundEffectsVolume", soundEffectsVolumeSlider.value);

        if (!saveSettings)
        {
            return;
        }

        PlayerPrefs.SetInt(ResolutionWidthKey, selectedResolution.x);
        PlayerPrefs.SetInt(ResolutionHeightKey, selectedResolution.y);
        PlayerPrefs.SetInt(DisplayModeKey, displayModeDropdown.value);
        PlayerPrefs.SetInt(FrameRateKey, availableFrameRates[frameRateDropdown.value]);
        PlayerPrefs.SetInt(VSyncKey, vSyncToggle.isOn ? 1 : 0);

        PlayerPrefs.SetFloat(MasterVolumeKey, masterVolumeSlider.value);
        PlayerPrefs.SetFloat(MusicVolumeKey, musicVolumeSlider.value);
        PlayerPrefs.SetFloat(SoundEffectsVolumeKey, soundEffectsVolumeSlider.value);

        PlayerPrefs.SetFloat(MouseSensitivityKey, mouseSensitivitySlider.value);
        PlayerPrefs.SetFloat(FieldOfViewKey, fieldOfViewSlider.value);
        PlayerPrefs.SetFloat(CameraShakeKey, cameraShakeSlider.value);
        PlayerPrefs.SetInt(InvertVerticalCameraKey, invertVerticalCameraToggle.isOn ? 1 : 0);

        PlayerPrefs.SetInt(ControlHintsKey, controlHintsToggle.isOn ? 1 : 0);
        PlayerPrefs.SetInt(PauseWhenUnfocusedKey, pauseWhenUnfocusedToggle.isOn ? 1 : 0);

        PlayerPrefs.Save();
    }

    /// <summary>
    /// Converts a volume between 0 and 1 into dB
    /// </summary>
    /// <param name="name"></param>
    /// <param name="volume"></param>
    private void SetMixerVolume(string name, float volume)
    {
        if(audioMixer == null)
        {
            return;
        }

        float decibels = Mathf.Log10(Mathf.Max(volume, 0.0001f)) * 20f;

        audioMixer.SetFloat(name, decibels);
    }

    /// <summary>
    /// Find the dropdown index for saved resolutions
    /// </summary>
    /// <param name="width"></param>
    /// <param name="height"></param>
    /// <returns></returns>
    private int FindResolutionIndex(int width, int height)
    {
        for (int i = 1; i < availableResolutions.Count; i++)
        {
            if (availableResolutions[i].x == width && availableResolutions[i].y == height)
            {
                return i;
            }
        }

        return FindCurrentResolutionIndex();
    }

    /// <summary>
    /// find the current screen resolution
    /// </summary>
    /// <returns></returns>
    private int FindCurrentResolutionIndex()
    {
        return FindResolutionIndexWithoutFallback(Screen.currentResolution.width, Screen.currentResolution.height);
    }

    private int FindResolutionIndexWithoutFallback(int width, int height)
    {
        for(int i = 0; i < availableResolutions.Count; i++)
        {
            if (availableResolutions[i].x == width && availableResolutions[i].y == height)
            {
                return i;
            }
        }

        return 0;
    }

    private void SelectCurrentResolution()
    {
        resolutionDropdown.SetValueWithoutNotify(FindCurrentResolutionIndex());
    }

    private int FindFrameRateIndex(int frameRate)
    {
        for (int i = 0; i < availableFrameRates.Length; i++){
            if (availableFrameRates[i] == frameRate)
            {
                return i;
            }
        }

        return availableFrameRates.Length - 1;
    }

    /// <summary>
    /// Convert the dropdown into a display mode
    /// </summary>
    /// <param name="displayModeIndex"></param>
    /// <returns></returns>
    private FullScreenMode GetDisplayMode(int displayModeIndex)
    {
        switch (displayModeIndex)
        {
            case 1:
                return FullScreenMode.ExclusiveFullScreen;
            case 2:
                return FullScreenMode.Windowed;
            default:
                return FullScreenMode.FullScreenWindow;
        }
    }
}
