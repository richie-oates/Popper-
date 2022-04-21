using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;
using UnityEngine.SocialPlatforms;

public class RecordsMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText, highComboText, bubblesText, accuracyText;
    [SerializeField] PlayerStats_so playerStats_so;

    int highScore, highCombo;


    private void OnEnable()
    {
        highScore = PlayerPrefs.GetInt("highscore", highScore);
        highScoreText.text = String.Format("{0:#,###0}", highScore);

        highCombo = PlayerPrefs.GetInt("highCombo", highCombo);
        highComboText.text = String.Format("{0:#,###0}", highCombo);
        
        bubblesText.text = String.Format("{0:#,###0}", playerStats_so.BubblesHitTotal);
        accuracyText.text = String.Format("{0:0}", playerStats_so.AccuracyTotal) + " %";
    }

    /*private void GetGPGSScores()
    {
        Social.LoadScores(GPGSIds.leaderboard_high_score, scores =>
        {
            if (scores.Length > 0)
            {
                leaderScore.text = String.Format("{0:#,###0}", scores[0].value);
                leaderName.text = GetProfileName(scores);
            }
            else
            {
                leaderName.text = "No scores loaded";
            }
        });
    }*/

    /*void DoLeaderboard()
    {
        leaderboard = Social.CreateLeaderboard();
        leaderboard.id = GPGSIds.leaderboard_high_score;
        leaderboard.LoadScores(result => GetLeaderboardScore());
    }

    private void GetLeaderboardScore()
    {
            if (leaderboard.scores.Length > 0)
            {
                Debug.Log("Got " + leaderboard.scores.Length + " scores");
                leaderName.text = GetProfileName(leaderboard.scores);
                leaderScore.text = String.Format("{0:#,###0}", leaderboard.scores[0].value);
            }
            else
            {
                leaderName.text = "No scores loaded";
                Debug.Log("No scores loaded");
            }
    }*/

    /*string GetProfileName(IScore[] scores)
    {
        Debug.Log("Getting user profiles");
        string profileName = "No data";
        string[] userIDs = new string[0];
        if (scores.Length < 1 )
        {
            return profileName; 
        }
        userIDs[0] = scores[0].userID;
        Social.LoadUsers(userIDs, profiles =>
        {
            if (profiles.Length > 0)
                profileName = profiles[0].userName;
        });
        Debug.Log("Profile Name: " + profileName);
        return profileName;
    }*/

    public void PlayServicesButton()
    {
        if (GameManager.Instance.IsConnectedToGooglePlayServices)
        {
            DisplayGooglePlayLeaderboard();
            
        }
        else
        {
            GameManager.Instance.SignInToGooglePlayServices();
        }
    }
    public void DisplayGooglePlayLeaderboard()
    {
        Social.ShowLeaderboardUI();
    }
}
