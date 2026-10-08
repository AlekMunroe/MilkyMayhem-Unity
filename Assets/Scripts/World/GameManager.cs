//Checklist of things the GameManager should handle:
//1. Keep track of the state and progress of checkpoints
//2. Keep track of the player's score.

using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Container")]
    [SerializeField] private GameObject checkpointContainer;

    private List<GameObject> checkpointList = new List<GameObject>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("Duplicate GameManager Instance destroyed");
            
            Destroy(gameObject);
            return;
        }
        Instance = this;
    }

    private void Start()
    {
        GetCheckPoints();
    }

    private void GetCheckPoints()
    {
        foreach (GameObject child in checkpointContainer.transform)
        {
            if (child.CompareTag("Checkpoint"))
            {
                checkpointList.Add(child);
            }
        }
    }

    private void CheckPointProgress()
    {
        //Check if the player has reached a checkpoint and update the progress accordingly.
    }

    private void UpdateCheckPointProgress()
    {
        //Update the progress of the checkpoints based on the player's actions.
    }
}
