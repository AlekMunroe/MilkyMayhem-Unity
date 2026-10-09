//Checklist of things the GameManager should handle:
//1. Keep track of the state and progress of checkpoints
//2. Keep track of the player's score.

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

    public int RemainingCheckpoints => TotalCheckpoints - clearedCheckpoints.Count;

    public event Action<int> ScoreChanged;
    public event Action<int, int> CheckpointProgressChanged;
    public event Action RoundCompleted;

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
        FindCheckpoints();
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
        Debug.Log("GameManager: Game complete");

        RoundCompleted?.Invoke();

        Debug.LogWarning("GameManager: Game complete logic not implemented");
    }
}
