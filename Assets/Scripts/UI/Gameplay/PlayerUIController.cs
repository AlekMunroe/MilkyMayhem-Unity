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
        if (sprintSlider == null)
        {
            Debug.LogError("PlayerUIController: Sprint Slider is not assigned");

            enabled = false;
            return;
        }

        if(scoreText == null)
        {
            Debug.LogError("PlayerUIController: scoreText is not assigned");

            enabled = false;
            return;
        }
        
        if(timeText == null)
        {
            Debug.LogError("PlayerUIController: timeText is not assigned");

            enabled = false;
            return;
        }
        
        if(roundSummaryText == null)
        {
            Debug.LogError("PlayerUIController: roundSummaryText is not assigned");

            enabled = false;
            return;
        }
        
        if(roundResultsText == null)
        {
            Debug.LogError("PlayerUIController: roundResultsText is not assigned");

            enabled = false;
            return;
        }

        if (playerController == null)
        {
            Debug.LogError("PlayerUIController: Player Controller is not assigned");

            enabled = false;
            return;
        }

        sprintSlider.interactable = false;
    }

    private void Start()
    {
        if (GameManager.Instance == null)
        {
            Debug.LogError("PlayerUIController: No GameManager in the scene");

            enabled = false;
            return;
        }

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

        scoreText.text = "Milk delivered: " + completedDeliveries + "\nMilk left: " + remainingCheckpoints;
    }

    /// <summary>
    /// Displays the amount of remaining time in the round left
    /// </summary>
    /// <param name="timeRemaining"></param>
    private void UpdateTimeText(float timeRemaining)
    {
        int displayedTime = Mathf.CeilToInt(timeRemaining);

        timeText.text = displayedTime + " seconds remaining";
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
}
