using System.Collections;
using GooglePlayGames;
using GooglePlayGames.BasicApi;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collection_so")]
public class Collection_so : ScriptableObject
{
    [SerializeField] List<Collectable_so> collectable_sos;
    [SerializeField] string googlePlay_ID;

    bool isComplete;

    public void InitialiseCollection()
    {
        foreach (Collectable_so collectable in collectable_sos)
        {
            var collected = PlayerPrefs.GetInt(collectable.name) == 1;
            collectable.CanSpawn = !collected;
            collectable.HasBeenCollected = collected;
            
        }
    }

    public List<Collectable_so> RemainingCollectables()
    {
        List<Collectable_so> remaining = new List<Collectable_so>();
        foreach (Collectable_so collectable in collectable_sos)
        {
            if (collectable.HasBeenCollected == false)
            {
                remaining.Add(collectable);
            }
        }
        return remaining;
    }

    public void SetAsComplete()
    {
        PlayerPrefs.SetInt(name, 1);
        isComplete = true;
        SaveGooglePlayAchievement();
        foreach (Collectable_so collectable in collectable_sos)
        {
            collectable.HasBeenCollected = true;
        }
    }

    void SaveGooglePlayAchievement()
    {
        if (GameManager.Instance.IsConnectedToGooglePlayServices)
        {
            Social.ReportProgress(googlePlay_ID, 100, (success) =>
            {
                if (!success) Debug.LogError("Unable to log achievement");
            });
        }
        else
        {
            Debug.Log("Not signed in .. unable to log achievement");
        }
    }

    public List<Collectable_so> Collectable_sos
    {
        get { return collectable_sos; }
    }

    public bool IsComplete
    {
        get { return RemainingCollectables().Count == 0; }
    }
}
