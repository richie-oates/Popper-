using UnityEngine;

[CreateAssetMenu(menuName = "Player stats")]
public class PlayerStats_so : ScriptableObject
{
    #region fields
    int bubblesHitTotal;
    int objectsHitTotal;
    int missesTotal;

    int bubblesHitThisTurn;
    int objectsHitThisTurn;
    int missesThisTurn;
    #endregion

    #region Methods
    float CalculateAccuracy(int hits, int misses)
    {
        return 100 * hits / (hits + misses);
    }

    public void AddBubbleHit()
    {
        bubblesHitThisTurn++;
        bubblesHitTotal++;
    }

    public void AddObjectHit()
    {
        objectsHitThisTurn++;
        objectsHitTotal++;
    }

    public void AddMiss()
    {
        missesThisTurn++;
        missesTotal++;
    }

    public void ResetTurnCounts()
    {
        bubblesHitThisTurn = 0;
        objectsHitThisTurn = 0;
        missesThisTurn = 0;
    }

    public void ResetTotalCounts()
    {
        bubblesHitTotal = 0;
        objectsHitTotal = 0;
        missesTotal = 0;

        SaveTotalCounts();
    }

    public void SaveTotalCounts()
    {
        PlayerPrefs.SetInt("bubblesHitTotal", bubblesHitTotal);
        PlayerPrefs.SetInt("objectsHitTotal", objectsHitTotal);
        PlayerPrefs.SetInt("missesTotal", missesTotal);
    }

    public void LoadTotalCounts()
    {
        bubblesHitTotal = PlayerPrefs.GetInt("bubblesHitTotal");
        objectsHitTotal = PlayerPrefs.GetInt("objectsHitTotal");
        missesTotal = PlayerPrefs.GetInt("missesTotal");
    }
    #endregion

    #region Properties
    public int BubblesHitTotal
    {
        get { return bubblesHitTotal; }
    }

    public int ObjectsHitTotal
    {
        get { return objectsHitTotal; }
    }

    public int MissesTotal
    {
        get { return missesTotal; }
    }

    public float AccuracyTotal
    {
        get { return CalculateAccuracy(objectsHitTotal, MissesTotal); }
    }

    public int BubblesHitThisTurn
    {
        get { return bubblesHitThisTurn; }
    }

    public int ObjectsHitThisTurn
    {
        get { return objectsHitThisTurn; }
    }

    public int MissesThisTurn
    {
        get { return missesThisTurn; }
    }

    public float AccuracyThisTurn
    {
        get { return CalculateAccuracy(objectsHitThisTurn, MissesThisTurn); }
    }
    #endregion
}