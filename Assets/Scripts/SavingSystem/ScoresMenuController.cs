using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ScoresMenuController : MonoBehaviour
{
    [Header("Score Table")]
    [SerializeField] private Transform scoreRowsContainer;
    [SerializeField] private GameObject tableHeaders;
    [SerializeField] private GameObject scoreRowPrefab;
    [SerializeField] private GameObject noScoresMessage;
    [SerializeField, Min(1)] private int maximumDisplayedScores = 13;

    private readonly List<GameObject> displayedRows = new List<GameObject>();

    private void OnEnable()
    {
        RefreshScores();
    }

    public void RefreshScores()
    {
        ClearDisplayedRows();

        if(scoreRowsContainer == null || scoreRowPrefab == null)
        {
            Debug.LogError("ScoresMenuController: A reference is not assigned");

            return;
        }

        SavedRoundResultCollection collection = ScoreSaveSystem.LoadResults();

        bool hasSavedResults = collection.results.Count > 0;

        if(noScoresMessage != null)
        {
            noScoresMessage.SetActive(!hasSavedResults);
        }

        if(scoreRowsContainer != null)
        {
            scoreRowsContainer.gameObject.SetActive(hasSavedResults);
        }

        if(tableHeaders != null)
        {
            tableHeaders.SetActive(hasSavedResults);
        }

        int displayedResultCount = Mathf.Min(collection.results.Count, maximumDisplayedScores);

        for(int i = 0; i < displayedResultCount; i++)
        {
            CreateScoreRow(collection.results[i]);
        }
    }

    private void CreateScoreRow(SavedRoundResult result)
    {
        GameObject rowObject = Instantiate(scoreRowPrefab, scoreRowsContainer);

        displayedRows.Add(rowObject);

        Transform row = rowObject.transform;

        SetRowText(row, "DateText", result.date);
        SetRowText(row, "ScoreText", result.score.ToString());
        SetRowText(row, "DeliveriesText", result.deliveriesCompleted + "/" + result.totalDeliveries);
        SetRowText(row, "BrokenText", result.bottlesBroken.ToString());
        SetRowText(row, "TimeText", FormatTime(result.completionTime));
        SetRowText(row, "RouteText", result.routeNumber.ToString());
    }

    private void SetRowText(Transform row, string objectName, string value)
    {
        Transform textObject = row.Find(objectName);

        if (textObject == null)
        {
            Debug.LogError("ScoresMenuController: " + objectName + " could not be found in the Score Row prefab", row);

            return;
        }

        TMP_Text text = textObject.GetComponent<TMP_Text>();

        if(text == null)
        {
            Debug.LogError("ScoresMenuController: " + objectName + " doesnt contain a TMP_Text component", textObject);

            return;
        }

        text.text = value;
    }

    /// <summary>
    /// Format seconds into MM:SS
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    private string FormatTime(float time)
    {
        int displayedTime = Mathf.FloorToInt(time);
        int minutes = displayedTime / 60;
        int seconds = displayedTime % 60;

        return minutes.ToString("00") + ":" + seconds.ToString("00");
    }

    /// <summary>
    /// Removes the old existing rows
    /// </summary>
    private void ClearDisplayedRows()
    {
        foreach(GameObject row in displayedRows)
        {
            if(row != null)
            {
                row.SetActive(false);
                Destroy(row);
            }
        }

        displayedRows.Clear();
    }
}
