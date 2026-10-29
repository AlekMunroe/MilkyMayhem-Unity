using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerUIController : MonoBehaviour
{
    [SerializeField] private PlayerController playerController;

    [Header("UI Elements")] 
    [SerializeField] private Slider sprintSlider;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private TMP_Text timeText;

    [SerializeField] private GameObject deliveryInfo;
    [SerializeField] private GameObject timer;
    [SerializeField] private GameObject compass;

    [Header("Round Completion")]
    [SerializeField] private TMP_Text roundResultsText;
    [SerializeField] private TMP_Text roundSummaryText;

    private Color defaultTimeTextColour;

    void Awake()
    {
        CheckNull(sprintSlider);
        CheckNull(scoreText);
        CheckNull(timeText);
        CheckNull(roundSummaryText);
        CheckNull(roundResultsText);
        CheckNull(playerController);
        CheckNull(deliveryInfo);
        CheckNull(timer);
        CheckNull(compass);

        sprintSlider.interactable = false;

        defaultTimeTextColour = timeText.color;
    }

    private void Start()
    {
        CheckNull(GameManager.Instance);

        GameManager.Instance.CheckpointProgressChanged += UpdateDeliveryText;
        GameManager.Instance.TimeRemainingChanged += UpdateTimeText;
        GameManager.Instance.RoundCompleted += UpdateRoundCompleteUI;
        GameManager.Instance.RoundStarted += ShowRoundUI;

        UpdateDeliveryText(GameManager.Instance.RemainingCheckpoints, GameManager.Instance.TotalCheckpoints);
        UpdateTimeText(GameManager.Instance.TimeRemaining);
        SetRoundUIActive(GameManager.Instance.IsRoundRunning);
    }

    void Update()
    {
        UpdateSprintSlider();
    }

    private void UpdateSprintSlider()
    {
        float sprintSliderValue = playerController.SprintStaminaPercent;
        
        sprintSlider.value = sprintSliderValue;
    }

    /// <summary>
    /// Displays the amount of milk deliveries completed
    /// </summary>
    private void UpdateDeliveryText(int remainingCheckpoints, int totalCheckpoints)
    {
        int completedDeliveries = totalCheckpoints - remainingCheckpoints;

        scoreText.text = completedDeliveries + " Delivered\n\n" + remainingCheckpoints + " Left";
    }

    /// <summary>
    /// Displays the amount of remaining time in the round left
    /// </summary>
    /// <param name="timeRemaining"></param>
    private void UpdateTimeText(float timeRemaining)
    {
        int displayedTime = Mathf.CeilToInt(timeRemaining);

        int minutes = displayedTime / 60;
        int seconds = displayedTime % 60;

        timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        if(timeRemaining <= GameManager.Instance.LowTimeWarningThreshold)
        {
            bool showRed = Mathf.FloorToInt(timeRemaining * 2f) % 2 == 0;

            timeText.color = showRed ? Color.red : defaultTimeTextColour;
        }
        else
        {
            timeText.color = defaultTimeTextColour;
        }
    }

    private void ShowRoundUI()
    {
        SetRoundUIActive(true);
    }

    private void SetRoundUIActive(bool active)
    {
        deliveryInfo.SetActive(active);
        timer.SetActive(active);
        compass.SetActive(active);
    }

    /// <summary>
    /// Displays the results for the completed round
    /// </summary>
    private void UpdateRoundCompleteUI()
    {
        GameManager gameManager = GameManager.Instance;

        SetRoundUIActive(false);

        bool roundWon = gameManager.RemainingCheckpoints == 0;

        if (roundWon)
        {
            roundResultsText.text = "Round complete";
        }
        else
        {
            roundResultsText.text = "Time ran out!";
        }

        int completedDeliveries = gameManager.TotalCheckpoints - gameManager.RemainingCheckpoints;

        float completionTime = gameManager.CompletionTime;

        int completionMinutes = Mathf.FloorToInt(completionTime / 60f);
        int completionSeconds = Mathf.FloorToInt(completionTime % 60f);
        string formattedCompletionTime = completionMinutes.ToString("00") + ":" + completionSeconds.ToString("00");

        roundSummaryText.text = "Completion time: " + formattedCompletionTime + "\n" + "Deliveries completed: " + completedDeliveries + "/" + gameManager.TotalCheckpoints + "\n" + "Bottles broken: " + gameManager.BottlesBroken + "\n" + "Final score: " + gameManager.Score;
    }

    private void OnDestroy()
    {
        if(GameManager.Instance == null)
        {
            return;
        }

        GameManager.Instance.CheckpointProgressChanged -= UpdateDeliveryText;
        GameManager.Instance.TimeRemainingChanged -= UpdateTimeText;
        GameManager.Instance.RoundCompleted -= UpdateRoundCompleteUI;
        GameManager.Instance.RoundStarted -= ShowRoundUI;
    }

    /// <summary>
    /// Check if a reference has been assigned and disable this when the reference is missing
    /// </summary>
    private void CheckNull(UnityEngine.Object reference)
    {
        if (reference == null)
        {
            Debug.LogError(GetType().Name + ": A reference is not assigned", this);

            enabled = false;
        }
    }
}
