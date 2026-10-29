using UnityEngine;
using System.Collections.Generic;
using System;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Container")]
    [SerializeField] private GameObject checkpointContainer;
    [SerializeField] private CheckpointRoute[] checkpointRoutePrefabs;

    private CheckpointRoute activeCheckpointRoute;

    public int RouteNumber { get; private set; }

    private readonly List<MilkCheckpoint> checkpointList = new List<MilkCheckpoint>();
    private readonly HashSet<MilkCheckpoint> clearedCheckpoints = new HashSet<MilkCheckpoint>();

    public int Score { get; private set; }
    public int TotalCheckpoints => checkpointList.Count;
    public IReadOnlyList<MilkCheckpoint> Checkpoints => checkpointList;

    public int RemainingCheckpoints => TotalCheckpoints - clearedCheckpoints.Count;

    public event Action<int> ScoreChanged;
    public event Action<int, int> CheckpointProgressChanged;
    public event Action RoundCompleted;

    public bool IsRoundRunning => isRoundRunning;
    public event Action RoundStarted;

    [Header("Round Timer")]
    [SerializeField] private float roundDuration = 120f;
    [SerializeField, Min(0f)] private float lowTimeWarningThreshold = 30f;
    private bool isRoundRunning;
    public float TimeRemaining { get; private set; }

    private bool lowTimeWarningTriggered;
    public float LowTimeWarningThreshold => lowTimeWarningThreshold;

    public event Action<float> TimeRemainingChanged;
    public event Action LowTimeWarningStarted;
    public event Action<int> FinalCountdownSecond;

    private int lastCountdownSecond = -1;

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

        TimeRemaining = roundDuration;
        isRoundRunning = false;
        TimeRemainingChanged?.Invoke(TimeRemaining);
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

        if(activeCheckpointRoute == null)
        {
            Debug.LogError("GameManager: No checkpoint route found");

            return;
        }

        MilkCheckpoint[] checkpoints = activeCheckpointRoute.GetComponentsInChildren<MilkCheckpoint>(true);

        checkpointList.AddRange(checkpoints);

        CheckpointProgressChanged?.Invoke(RemainingCheckpoints, TotalCheckpoints);

        if(TotalCheckpoints == 0)
        {
            Debug.LogWarning("GameManager: No MilkCheckpoints were found");
        }
    }

    private bool CreateRandomCheckpointRoute()
    {
        if(checkpointContainer == null)
        {
            Debug.LogError("GameManager: Checkpoint container is missing");

            return false;
        }

        if(checkpointRoutePrefabs == null || checkpointRoutePrefabs.Length == 0)
        {
            Debug.LogError("GameManager: No checkpoint route prefabs assigned");

            return false;
        }

        int routeIndex = UnityEngine.Random.Range(0, checkpointRoutePrefabs.Length);

        CheckpointRoute selectedRoute = checkpointRoutePrefabs[routeIndex];

        if(selectedRoute == null)
        {
            Debug.LogError("GameManager: The selected checkpoint route is null");

            return false;
        }

        activeCheckpointRoute = Instantiate(selectedRoute, checkpointContainer.transform);

        activeCheckpointRoute.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        activeCheckpointRoute.transform.localScale = Vector3.one;

        RouteNumber = activeCheckpointRoute.RouteNumber;

        return true;
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
    public void StartRound()
    {
        if (isRoundRunning)
        {
            return;
        }

        if (!CreateRandomCheckpointRoute())
        {
            return;
        }

        FindCheckpoints();

        if(TotalCheckpoints == 0)
        {
            return;
        }

        TimeRemaining = roundDuration;
        lowTimeWarningTriggered = false;
        lastCountdownSecond = -1;
        isRoundRunning = true;

        RoundStarted?.Invoke();
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

        if(!lowTimeWarningTriggered && TimeRemaining <= lowTimeWarningThreshold)
        {
            lowTimeWarningTriggered = true;
            LowTimeWarningStarted?.Invoke();
        }

        int displayedSecond = Mathf.CeilToInt(TimeRemaining);

        if(displayedSecond > 0 && displayedSecond <= 5 && displayedSecond != lastCountdownSecond)
        {
            lastCountdownSecond = displayedSecond;
            FinalCountdownSecond?.Invoke(displayedSecond);
        }

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
