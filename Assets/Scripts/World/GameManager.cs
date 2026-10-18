using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Container")]
    [SerializeField] private GameObject checkpointContainer;

    private readonly List<MilkCheckpoint> checkpointList = new List<MilkCheckpoint>();
    private readonly HashSet<MilkCheckpoint> clearedCheckpoints = new HashSet<MilkCheckpoint>();

    public int Score { get; private set; }
    public int TotalCheckpoints => checkpointList.Count;
    public IReadOnlyList<MilkCheckpoint> Checkpoints => checkpointList;

    public int RemainingCheckpoints => TotalCheckpoints - clearedCheckpoints.Count;

    public event Action<int> ScoreChanged;
    public event Action<int, int> CheckpointProgressChanged;
    public event Action RoundCompleted;

    [Header("Round Timer")]
    [SerializeField] private float roundDuration = 120f;
    private bool isRoundRunning;
    public float TimeRemaining { get; private set; }
    public event Action<float> TimeRemainingChanged;

    [Header("Round Completion")]
    [SerializeField] private GameObject RoundCompleteUI;

    [Header("Player Data")]
    public int BottlesBroken { get; private set; }
    public float CompletionTime => roundDuration - TimeRemaining;

    private void Awake()
    {
        if(Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GameManager instance destroyed");

            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    private void Start()
    {
        if(RoundCompleteUI == null)
        {
            Debug.LogError("GameManager: RoundCompleteUI is not assigned");

            enabled = false;
            return;
        }

        RoundCompleteUI.SetActive(false);

        FindCheckpoints();
        StartRoundTimer();
    }

    void Update()
    {
        UpdateRoundTimer();
    }

    /// <summary>
    /// Find every MilkCheckpoint
    /// </summary>
    private void FindCheckpoints()
    {
        checkpointList.Clear();
        clearedCheckpoints.Clear();

        if(checkpointContainer == null)
        {
            Debug.LogError("GameManager: Checkpoint Container is not assigned");

            return;
        }

        MilkCheckpoint[] checkpoints = checkpointContainer.GetComponentsInChildren<MilkCheckpoint>(true);

        checkpointList.AddRange(checkpoints);

        CheckpointProgressChanged?.Invoke(RemainingCheckpoints, TotalCheckpoints);

        if(TotalCheckpoints == 0)
        {
            Debug.LogWarning("GameManager: No MilkCheckpoints were found");
        }
    }

    public bool RegisterCheckpointCleared(MilkCheckpoint checkpoint, int scoreAmount)
    {
        if(checkpoint == null || !checkpointList.Contains(checkpoint))
        {
            Debug.LogWarning("GameManager: An unregistered checkpoint tried to complete");

            return false;
        }

        if (!clearedCheckpoints.Add(checkpoint))
        {
            return false;
        }

        Score += Mathf.Max(0, scoreAmount);
        ScoreChanged?.Invoke(Score);

        CheckpointProgressChanged?.Invoke(RemainingCheckpoints, TotalCheckpoints);

        if(RemainingCheckpoints == 0)
        {
            CompleteRound();
        }

        return true;
    }

    private void CompleteRound()
    {
        isRoundRunning = false;

        RoundCompleted?.Invoke();

        if (RoundCompleteUI != null)
        {
            RoundCompleteUI.SetActive(true);
        }

        if (WorldController.Instance != null)
        {
            WorldController.Instance.ReleaseMouse();
        }
    }

    /// <summary>
    /// Starts the round timer when called.
    /// A future update may be to call this from elsewhere to include controlled starting
    /// </summary>
    private void StartRoundTimer()
    {
        TimeRemaining = roundDuration;
        isRoundRunning = true;

        TimeRemainingChanged?.Invoke(TimeRemaining);
    }

    /// <summary>
    /// Counts down the time left of the current round
    /// </summary>
    private void UpdateRoundTimer()
    {
        if (!isRoundRunning)
        {
            return;
        }

        TimeRemaining -= Time.deltaTime;
        TimeRemaining = Mathf.Max(TimeRemaining, 0f);

        TimeRemainingChanged?.Invoke(TimeRemaining);

        if (TimeRemaining <= 0f)
        {
            isRoundRunning = false;

            CompleteRound();
        }
    }

    public void CountBrokenBottles()
    {
        BottlesBroken++;
    }
}
