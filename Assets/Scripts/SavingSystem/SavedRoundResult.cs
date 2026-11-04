using System;
using System.Collections.Generic;

/// <summary>
/// Holds the saved results from one round
/// </summary>
[Serializable]
public class SavedRoundResult
{
    public string date;
    public int score;
    public int deliveriesCompleted;
    public int totalDeliveries;
    public int bottlesBroken;
    public float completionTime;
    public int routeNumber;
    public bool routeCompleted;

    public SavedRoundResult(string date, int score, int deliveriesCompleted, int totalDeliveries, int bottlesBroken, float completionTime, int routeNumber, bool routeCompleted)
    {
        this.date = date;
        this.score = score;
        this.deliveriesCompleted = deliveriesCompleted;
        this.totalDeliveries = totalDeliveries;
        this.bottlesBroken = bottlesBroken;
        this.completionTime = completionTime;
        this.routeNumber = routeNumber;
        this.routeCompleted = routeCompleted;
    }
}

[Serializable]
public class SavedRoundResultCollection
{
    public List<SavedRoundResult> results = new List<SavedRoundResult>();
}