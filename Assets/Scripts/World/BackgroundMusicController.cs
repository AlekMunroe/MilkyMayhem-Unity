using System.Collections.Generic;
using UnityEngine;

public class BackgroundMusicController : MonoBehaviour
{
    [Header("Background Music")]
    [SerializeField] private GameObject milkStoreBackgroundMusic;
    [SerializeField] private GameObject roundBackgroundMusic;
    [SerializeField] private GameObject endRoundBackgroundMusic;
    [SerializeField] private GameObject pauseMenuBackgroundMusic;

    [Header("Low Time Warning")]
    [SerializeField] private AudioSource lowTimeWarningAudioSource;
    [SerializeField] private AudioSource singleBeepAudioSource;
    [SerializeField, Range(1f, 2f)] private float lowTimeMusicPitch = 1.15f;

    private float defaultRoundMusicPitch;

    private AudioSource milkStoreAudioSource;
    private AudioSource roundAudioSource;
    private AudioSource endRoundAudioSource;
    private AudioSource pauseMenuAudioSource;

    private AudioSource currentBackgroundMusic;

    private readonly HashSet<AudioSource> startedMusic = new HashSet<AudioSource>();

    private void Start()
    {
        if(GameManager.Instance == null)
        {
            Debug.LogError("BackgroundMusicController: GameManager not found");

            enabled = false;
            return;
        }

        if(WorldController.Instance == null)
        {
            Debug.LogError("BackgroundMusicController: WorldController not found");

            enabled = false;
            return;
        }

        if(lowTimeWarningAudioSource == null || singleBeepAudioSource == null)
        {
            Debug.LogError("BackgroundMusicController: a countdown AudioSource is not assigned");

            enabled = false;
            return;
        }

        milkStoreAudioSource = GetAudioSource(milkStoreBackgroundMusic);
        roundAudioSource = GetAudioSource(roundBackgroundMusic);
        endRoundAudioSource = GetAudioSource(endRoundBackgroundMusic);
        pauseMenuAudioSource = GetAudioSource(pauseMenuBackgroundMusic);

        defaultRoundMusicPitch = roundAudioSource.pitch;

        if(milkStoreAudioSource == null || roundAudioSource == null || endRoundAudioSource == null || pauseMenuAudioSource == null)
        {
            Debug.LogError("BackroundMusicController: Missing audio source");

            enabled = false;
            return;
        }

        GameManager.Instance.RoundStarted += HandleRoundStarted;
        GameManager.Instance.RoundCompleted += HandleRoundCompleted;
        WorldController.Instance.PauseChanged += HandlePauseChanged;
        GameManager.Instance.LowTimeWarningStarted += HandleLowTimeWarning;
        GameManager.Instance.FinalCountdownSecond += HandleFinalCountdownSecond;

        SwitchBackgroundMusic(milkStoreAudioSource);
    }

    private AudioSource GetAudioSource(GameObject musicObject)
    {
        if(musicObject == null)
        {
            Debug.LogError("BackgroundMusicController: A music object is missing");

            return null;
        }

        AudioSource audioSource = musicObject.GetComponent<AudioSource>();

        if(audioSource == null)
        {
            Debug.LogError(musicObject.name + " has no AudioSource");

            return null;
        }

        return audioSource;
    }

    private void HandleRoundStarted()
    {
        roundAudioSource.pitch = defaultRoundMusicPitch;

        SwitchBackgroundMusic(roundAudioSource);
    }

    private void HandleRoundCompleted()
    {
        roundAudioSource.pitch = defaultRoundMusicPitch;

        SwitchBackgroundMusic(endRoundAudioSource);
    }

    private void HandlePauseChanged(bool isPaused)
    {
        if (isPaused)
        {
            if(currentBackgroundMusic != null)
            {
                currentBackgroundMusic.Pause();
            }

            // The pause music always plays from the start
            pauseMenuAudioSource.Stop();
            pauseMenuAudioSource.Play();

            return;
        }

        pauseMenuAudioSource.Stop();

        ResumeMusic(currentBackgroundMusic);
    }

    private void HandleLowTimeWarning()
    {
        roundAudioSource.pitch = lowTimeMusicPitch;

        lowTimeWarningAudioSource.Stop();
        lowTimeWarningAudioSource.Play();
    }

    private void HandleFinalCountdownSecond(int secondsRemaining)
    {
        singleBeepAudioSource.Stop();
        singleBeepAudioSource.Play();
    }

    private void SwitchBackgroundMusic(AudioSource selectedMusic)
    {
        if(selectedMusic == null)
        {
            return;
        }

        if(currentBackgroundMusic != null && currentBackgroundMusic != selectedMusic)
        {
            currentBackgroundMusic.Pause();
        }

        currentBackgroundMusic = selectedMusic;

        ResumeMusic(currentBackgroundMusic);
    }

    private void ResumeMusic(AudioSource music)
    {
        if(music == null)
        {
            return;
        }

        if (startedMusic.Add(music))
        {
            music.Play();
        }
        else
        {
            music.UnPause();
        }
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.RoundStarted -= HandleRoundStarted;
            GameManager.Instance.RoundCompleted -= HandleRoundCompleted;
            GameManager.Instance.LowTimeWarningStarted -= HandleLowTimeWarning;
            GameManager.Instance.FinalCountdownSecond -= HandleFinalCountdownSecond;
        }

        if(WorldController.Instance != null)
        {
            WorldController.Instance.PauseChanged -= HandlePauseChanged;
        }
    }
}
