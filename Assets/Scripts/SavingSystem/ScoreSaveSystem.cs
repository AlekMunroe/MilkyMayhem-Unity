using UnityEngine;
using System;

/// <summary>
/// Save the final results when a round finishes
/// </summary>
public class ScoreSaveSystem : MonoBehaviour
{
    private const string SavedResultsKey = "SavedRoundResults";
    private const int MaximumSavedResults = 50;

    private bool hasSavedCurrentRound;

    void Start()
    {
        if(GameManager.Instance == null)
        {
            Debug.LogError("ScoreSaveSystem: GameManager could not be found");

            enabled = false;
            return;
        }

        GameManager.Instance.RoundCompleted += SaveCurrentRound;
    }

    /// <summary>
    /// Collect and save the data from the current round
    /// </summary>
    private void SaveCurrentRound()
    {
        if (hasSavedCurrentRound)
        {
            return;
        }

        GameManager gameManager = GameManager.Instance;

        int deliveriesCompleted = gameManager.TotalCheckpoints - gameManager.RemainingCheckpoints;
        bool routeCompleted = gameManager.TotalCheckpoints > 0 && gameManager.RemainingCheckpoints == 0;

        SavedRoundResult result = new SavedRoundResult(DateTime.Now.ToString("dd/MM/yyyy HH:mm"), gameManager.Score, deliveriesCompleted, gameManager.TotalCheckpoints, gameManager.BottlesBroken, gameManager.CompletionTime, gameManager.RouteNumber, routeCompleted);

        SavedRoundResultCollection collection = LoadResults();

        collection.results.Add(result);

        // Highest score - First
        SortResults(collection);

        // Stop the PlayerPrefs value from growing
        if(collection.results.Count > MaximumSavedResults)
        {
            collection.results.RemoveRange(MaximumSavedResults, collection.results.Count - MaximumSavedResults);
        }

        string json = JsonUtility.ToJson(collection);

        PlayerPrefs.SetString(SavedResultsKey, json);
        PlayerPrefs.Save();

        Debug.Log("ScoreSaveSystem: Round results saved");
    }

    /// <summary>
    /// Loads the saved round results
    /// </summary>
    /// <returns></returns>
    public static SavedRoundResultCollection LoadResults()
    {
        string json = PlayerPrefs.GetString(SavedResultsKey, "");

        if (string.IsNullOrEmpty(json))
        {
            return new SavedRoundResultCollection();
        }

        SavedRoundResultCollection collection = JsonUtility.FromJson<SavedRoundResultCollection>(json);

        if(collection == null || collection.results == null)
        {
            return new SavedRoundResultCollection();
        }

        SortResults(collection);

        return collection;
    }

    private static void SortResults(SavedRoundResultCollection collection)
    {
        collection.results.Sort((first, second) =>
        {
            int scoreComparison = second.score.CompareTo(first.score);

            if (scoreComparison != 0)
            {
                return scoreComparison;
            }

            return first.completionTime.CompareTo(second.completionTime);
        });
    }

    private void OnDestroy()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.RoundCompleted -= SaveCurrentRound;
        }
    }
}
