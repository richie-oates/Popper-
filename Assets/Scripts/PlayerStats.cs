using System;
using UnityEngine;
using GooglePlayGames;
using UnityEngine.SocialPlatforms;

public class PlayerStats : MonoBehaviour
{
    [SerializeField] PlayerStats_so playerStats_so;
    [SerializeField] string accuracyLeaderboardID;
    [SerializeField] string bubbleLeaderboardID;
    [SerializeField] string gamesPlayedEventID;

    void Awake()
    {
        playerStats_so.LoadTotalCounts();

        EventBroker.HitObject += OnObjectHit;
        EventBroker.MissedEverything += OnMissedEverything;
        EventBroker.ClearRecords += OnClearRecords;       
    }

    void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    void OnDestroy()
    {
        EventBroker.HitObject -= OnObjectHit;
        EventBroker.MissedEverything -= OnMissedEverything;
        EventBroker.ClearRecords -= OnClearRecords;
    }

    void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        switch(currentState)
        {
            case GameManager.GameState.RUNNING :
                if (previousState == GameManager.GameState.PREGAME)
                        playerStats_so.ResetTurnCounts();
                playerStats_so.AddGamesPlayed();
                SubmitEvent(gamesPlayedEventID);
                break;

            case GameManager.GameState.PREGAME :
                playerStats_so.SaveTotalCounts();
                SaveStatsToGooglePlayLeaderboard();
                break;

            case GameManager.GameState.QUIT :
                playerStats_so.SaveTotalCounts();
                SaveStatsToGooglePlayLeaderboard();
                break;

            case GameManager.GameState.ENDGAME :
                playerStats_so.SaveTotalCounts();
                SaveStatsToGooglePlayLeaderboard();
                break;

            default:
                break;
        }
    }

    void OnObjectHit(GameObject obj)
    {
        playerStats_so.AddObjectHit();

        if (obj.CompareTag("Bubble"))
        {
            playerStats_so.AddBubbleHit();
        }
    }

    void OnMissedEverything()
    {
        playerStats_so.AddMiss();
    }

    void OnClearRecords()
    {
        playerStats_so.ResetTotalCounts();
    }

    void SaveStatsToGooglePlayLeaderboard()
    {
        if (GameManager.Instance.IsConnectedToGooglePlayServices)
        {
            /*Debug.Log("Reporting accuracy...");
            Social.ReportScore(Convert.ToInt64(playerStats_so.AccuracyTotal), accuracyLeaderboardID, (success) =>
            {
                if (!success) Debug.LogError("Unable to post accuracy");
            });*/
            Debug.Log("Reporting bubbles...");
            Social.ReportScore(playerStats_so.BubblesHitTotal, bubbleLeaderboardID, (success) =>
            {
                if (!success) Debug.LogError("Unable to post bubbles");
            });
        }
        else
        {
            Debug.Log("Not signed in .. unable to report stats");
        }
    }

    public void SubmitEvent(String eventId)
    {
        try
        {
            PlayGamesPlatform.Instance.Events.IncrementEvent(eventId, 1);
        }
        catch
        {
            print("Failed to post event to GPGS");
        }
    }
}
