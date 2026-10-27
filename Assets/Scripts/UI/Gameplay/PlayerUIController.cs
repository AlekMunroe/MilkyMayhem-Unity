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

    [Header("Round Completion")]
    [SerializeField] private TMP_Text roundResultsText;
    [SerializeField] private TMP_Text roundSummaryText;

    void Awake()
    {
        CheckNull(sprintSlider);
        CheckNull(scoreText);
        CheckNull(timeText);
        CheckNull(roundSummaryText);
        CheckNull(roundResultsText);
        CheckNull(playerController);

        sprintSlider.interactable = false;
    }

    private void Start()
    {
        CheckNull(GameManager.Instance);

        GameManager.Instance.CheckpointProgressChanged += UpdateDeliveryText;
        GameManager.Instance.TimeRemainingChanged += UpdateTimeText;
        GameManager.Instance.RoundCompleted += UpdateRoundCompleteUI;

        UpdateDeliveryText(GameManager.Instance.RemainingCheckpoints, GameManager.Instance.TotalCheckpoints);
        UpdateTimeText(GameManager.Instance.TimeRemaining);
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
    }

    /// <summary>
    /// Displays the results for the completed round
    /// </summary>
    private void UpdateRoundCompleteUI()
    {
        GameManager gameManager = GameManager.Instance;

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
