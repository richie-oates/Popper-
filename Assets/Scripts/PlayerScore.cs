using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// TODO: Needs refactoring and splitting into smaller scripts
//
public class PlayerScore : Singleton<PlayerScore>
{
    int score = 0, highScore = 0, highCombo = 0, combo = 0, comboLevel = 0, multiplier = 1;
    int maxDangerLevel = 8, warningLevel = 6, timeForMultiplier = 5, dangerLevelPeriod = 5;
    [SerializeField] TextMeshProUGUI scoreText, highScoreText, bubbleLostText, comboText, comboLevelText, multiplierText;
    [SerializeField] Slider dangerLevelSlider, comboLevelSlider;
    [SerializeField] GameObject dangerWarningText;
    int objectsHit, dangerLevel;

    GameManager.GameState _currentState;

    public bool gameOver;
    private bool arcadeMode = true;

    public int Score
    {
        get { return score; }
    }

    public int HighScore
    {
        get { return highScore; }
    }

    protected override void Awake()
    {
        base.Awake();
        EventBroker.ClearRecords += OnClearRecords;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();
        EventBroker.ClearRecords -= OnClearRecords;

    }

    private void Start()
    {
        // Add listener for GameState changes
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        // Listener for GameMode Changes
        EventBroker.ChangeGameMode += OnChangeGameMode;

        // Add listeners for lost objects, hit objects and misses
        EventBroker.ObjectLost += OnObjectLost;
        EventBroker.HitObject += OnHitObject;
        EventBroker.MissedEverything += OnMissedEverything;

        InitialiseVariables();

        // Get high score and combo from player prefs and display it
        highScore = PlayerPrefs.GetInt("highscore", highScore);
        highScoreText.text = "High Score: " + highScore; // Currently only used for debugging
        highCombo = PlayerPrefs.GetInt("highCombo", highCombo);
    }

    void InitialiseVariables()
    {
        score = 0;
        scoreText.text = String.Format("{0:#,###0}", score);
        ResetCombo();
        objectsHit = 0;
        multiplier = 1;
        dangerLevel = 0;
        dangerLevelSlider.value = 0;
        dangerLevelSlider.maxValue = maxDangerLevel;
        dangerWarningText.SetActive(false);
        gameOver = false;
        UpdateScore(0, null);
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _currentState = currentState;
        if (_currentState == GameManager.GameState.PREGAME && (previousState == GameManager.GameState.ENDGAME || previousState == GameManager.GameState.PAUSED))
        {
            InitialiseVariables();
        }
        if (arcadeMode && currentState == GameManager.GameState.ENDGAME)
        {
            //Save high score to player prefs
            PlayerPrefs.SetInt("highscore", highScore);
            PlayerPrefs.SetInt("highCombo", highCombo);

            SaveHighScoreToGooglePlayLeaderboard();
        }
    }

    void OnClearRecords()
    {
        highCombo = 0;
        highScore = 0;
        PlayerPrefs.SetInt("highscore", highScore);
        PlayerPrefs.SetInt("highCombo", highCombo);
    }

    private void SaveHighScoreToGooglePlayLeaderboard()
    {
        if (GameManager.Instance.IsConnectedToGooglePlayServices)
        {
            Debug.Log("Reporting score...");
            Social.ReportScore(highScore, GPGSIds.leaderboard_high_score, (success) =>
            {
                if (!success) Debug.LogError("Unable to post highscore");
            });
        }
        else
        {
            Debug.Log("Not signed in .. unable to report score");
        }
    }

    public void ResetCombo()
    {
        combo = 0;
        comboLevel = 1;
        comboLevelText.text = "Combo";
        comboLevelSlider.value = 0; 
        comboText.text = "Combo: " + combo; // DebugHUD
    }

    // Updates combo, score and high score and displays them
    public void UpdateScore(int points, GameObject destroyedObject)
    {
        if (!gameOver && (_currentState == GameManager.GameState.RUNNING
            || _currentState == GameManager.GameState.FROZEN))
        {
            // calculate points just scored and add them to the total score
            int extraPoints = points * comboLevel * multiplier;
            score += extraPoints;
            if (destroyedObject != null)
            { 
                // Floating text to show points scored
                UIManager.Instance.ShowText("+" + (extraPoints).ToString(), 50, new Color(0.1137f, 0.1215f, 0.898f), destroyedObject.transform.position, Vector3.up * 10, 0.5f); 
            }
            //Display total score
            scoreText.text = String.Format("{0:#,###0}", score);

            //Check for high score
            if (arcadeMode && score > highScore)
            {
                highScore = score;
                highScoreText.text = "High Score: " + highScore;
            }
        }
    }

    public void OnHitObject(GameObject objectHit)
    {
        if ( _currentState == GameManager.GameState.RUNNING
            || _currentState == GameManager.GameState.FROZEN)
        {
            // comboLevel goes up for every 10 on the combo
            combo++;
            if (combo > highCombo)
            {
                highCombo = combo; // ToDo: Display message on screen, play sound
            }
            comboLevelSlider.value = combo % 10;
            if (combo > 1 && combo % 10 == 0)
            {
                EventBroker.CallComboLevelUp();
                comboLevel = combo;
                comboLevelText.text = "X" + combo + " Combo";
            }

            comboText.text = "Combo: " + combo; // For DebugHUD

            if (objectHit.CompareTag("Bird"))
            {
                StartCoroutine(MultiplierTimer(timeForMultiplier));
            }
            // every 4 bubbles hit reduces danger level
            objectsHit++;
            if (objectsHit % 3 == 0)
            {
                DangerLevelUpdate(-1);
            }
        }
    }

    // Combo multiplier gets doubled for a certain time
    IEnumerator MultiplierTimer(float time)
    {
        // double the multiplier
        multiplier *= 2;
        // display text
        multiplierText.text = "x" + multiplier;
        multiplierText.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        // reduce the multiplier
        multiplier /= 2;
        // adjust text
        multiplierText.text = "x" + multiplier;
        // multiplier can stack if we hit multiple birds so we need to check it's back to normal before hiding the text again
        if (multiplier <= 1)
        {
            multiplierText.gameObject.SetActive(false);
        }
    }

    public void OnObjectLost(OutOfBoundsEventArgs args)
    {
        if (_currentState == GameManager.GameState.PREGAME || gameOver) return;
        if (args.objectTag == "Bubble" && arcadeMode)
        {
            DangerLevelUpdate(1);
            StartCoroutine(ChangeDangerLevelAfterDelay(-1, dangerLevelPeriod));
        }
    }

    public void OnMissedEverything()
    {
        if (_currentState == GameManager.GameState.PREGAME || gameOver) return;
        ResetCombo();
        if (arcadeMode)
        {
            DangerLevelUpdate(1);
            StartCoroutine(ChangeDangerLevelAfterDelay(-1, dangerLevelPeriod));
        }
    }

    public void DangerLevelUpdate(int changeAmount)
    {
        dangerLevel += changeAmount;
        if (dangerLevel < 0) dangerLevel = 0;

        if (gameOver || _currentState == GameManager.GameState.PREGAME) return;

        // Updates the danger level slider
        dangerLevelSlider.maxValue = maxDangerLevel;
        dangerLevelSlider.value = dangerLevel;

        // Hides warning text at certain level
        if (dangerLevel < warningLevel)
        {
            dangerWarningText.SetActive(false);
        }
        // Displays warning text at certain level
        else if (dangerLevel >= warningLevel)
        {
            dangerWarningText.SetActive(true);
            // Triggers game over
            if (dangerLevel >= maxDangerLevel)
            {
                StartCoroutine(GameOverSequence());
            }
        }
        
    }

    // Triggers a game over sequence where hundreds of bubbles are spawned at once
    // Then we update game state to show the game over menu screen
    IEnumerator GameOverSequence()
    {
        gameOver = true;
        EventBroker.CallGameOverTriggered();
        yield return new WaitForSeconds(2.0f);
        GameManager.Instance.UpdateState(GameManager.GameState.ENDGAME);
    }

    IEnumerator ChangeDangerLevelAfterDelay(int changeAmount, float delay)
    {
        yield return new WaitForSeconds(delay);
        if(!gameOver)
        {
            DangerLevelUpdate(changeAmount);
        }
    }

    public void OnChangeGameMode(GameManager.GameMode currentGameMode)
    {
        arcadeMode = currentGameMode == GameManager.GameMode.ARCADE;
    }
}
