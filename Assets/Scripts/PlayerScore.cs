using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PlayerScore : Singleton<PlayerScore>
{
    int score = 0, highScore = 0, highCombo = 0, bubblesLost, bubblesHit, combo = 0, comboLevel = 0, multiplier = 1;
    int maxBubblesLost = 8, timeForMultiplier = 5;
    [SerializeField] TextMeshProUGUI scoreText, highScoreText, bubbleLostText, comboText, comboLevelText, multiplierText;
    [SerializeField] Slider dangerLevelSlider, comboLevelSlider;
    int dangerLevel, currentMisses;

    GameManager.GameState _currentState;

    ObjectSpawner objectSpawner;
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

    private void Start()
    {
        // Add listener for GameState changes
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        // Listener for GameMode Changes
        EventBroker.ChangeGameMode += OnChangeGameMode;

        // Add listeners for missed bubbles and hit objects
        EventBroker.BubbleLost += OnBubbleLost;
        EventBroker.HitObject += OnHitObject;
        EventBroker.MissedEverything += OnMissedEverything;

        objectSpawner = FindObjectOfType<ObjectSpawner>();

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
        bubblesLost = 0;
        bubblesHit = 0;
        multiplier = 1;
        dangerLevel = 0;
        currentMisses = 0;
        dangerLevelSlider.value = 0;
        dangerLevelSlider.maxValue = maxBubblesLost;
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
        if (currentState == GameManager.GameState.ENDGAME)
        {
            //Save high score to player prefs
            PlayerPrefs.SetInt("highscore", highScore);
            PlayerPrefs.SetInt("highCombo", highCombo);

        }
    }

    public void ResetCombo()
    {
        combo = 0;
        comboLevel = 1;
        comboLevelText.text = "Combo";
        comboLevelSlider.value = 0; 
        comboText.text = "Combo: " + combo;
    }

    // Updates combo, score and high score and displays them
    public void UpdateScore(int points, GameObject destroyedObject)
    {
        if (!gameOver && (_currentState == GameManager.GameState.RUNNING
            || _currentState == GameManager.GameState.FROZEN))
        {
            // every 3 bubbles hit reduces danger level
            bubblesHit++;
            if (bubblesHit >= 3)
            {
                bubblesHit = 0;
                StartCoroutine(RemoveBubbleFromCount(0));
            }
            // calculate points just scored and add them to the total score
            int extraPoints = points * comboLevel * multiplier;
            score += extraPoints;
            if (destroyedObject != null)
            { 
                // Floating text to show points scored
                GameManager.Instance.ShowText("+" + (extraPoints).ToString(), 50, new Color(0.1137f, 0.1215f, 0.898f), destroyedObject.transform.position, Vector3.up * 10, 0.5f); 
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
                highCombo = combo;
                // ToDo: Display message on screen, play sound
            }
            comboLevelSlider.value = combo % 10;
            if (combo > 1 && combo % 10 == 0)
            {
                comboLevel = combo;
                comboLevelText.text = "X" + combo + " Combo";
            }

            comboText.text = "Combo: " + combo; // For debugging

            if (objectHit.CompareTag("Bird"))
            {
                StartCoroutine(MultiplierTimer(timeForMultiplier));
            }
        }
    }

    // Combo multiplier gets doubled for a certain time
    IEnumerator MultiplierTimer(float time)
    {
        multiplier *= 2;
        multiplierText.text = "x" + multiplier;
        multiplierText.gameObject.SetActive(true);
        yield return new WaitForSeconds(time);
        multiplier /= 2;
        multiplierText.text = "x" + multiplier;
        if (multiplier <= 1)
        {
            multiplierText.gameObject.SetActive(false);

        }
    }

    // Keeps a tally of bubbles which go off the screen in the last x seconds
    // Changes the danger level based on this amount
    // Calls game over if the max is reached
    public void OnBubbleLost()
    {
        if (arcadeMode && !gameOver)
        {
            bubblesLost++;
            bubbleLostText.text = "Bubbles Lost: " + bubblesLost;
            DangerLevelIncrease();
            StartCoroutine(RemoveBubbleFromCount(5));
        }
    }

    public void OnMissedEverything()
    {
        if (arcadeMode && !gameOver)
        {
            currentMisses++;
            DangerLevelIncrease();
            StartCoroutine(RemoveMissFromCount(5));
        }
    }

    public void DangerLevelIncrease()
    {
        dangerLevelSlider.maxValue = maxBubblesLost;
        dangerLevel = bubblesLost + currentMisses;
        dangerLevelSlider.value = dangerLevel;

        if (dangerLevel >=  maxBubblesLost && !gameOver)
        {
            gameOver = true;
            StartCoroutine(GameOverSequence());
        }
    }

    // Triggers a game over sequence where hundreds of bubbles are spawned at once
    IEnumerator GameOverSequence()
    {
        objectSpawner.TriggerGameOver(0.01f, 0.5f);
        yield return new WaitForSeconds(3.0f);
        GameManager.Instance.UpdateState(GameManager.GameState.ENDGAME);
    }

    IEnumerator RemoveBubbleFromCount(float delay)
    {
        yield return new WaitForSeconds(delay);
        if(!gameOver)
        {
            bubblesLost--;
            if (bubblesLost < 0) bubblesLost = 0;
            bubbleLostText.text = "Bubbles Lost: " + bubblesLost;
            dangerLevelSlider.value = bubblesLost + currentMisses;
        }
    }
    IEnumerator RemoveMissFromCount(float delay)
    {
        yield return new WaitForSeconds(delay);
        if (!gameOver)
        {
            currentMisses--;
            if (currentMisses < 0) currentMisses = 0;
            dangerLevelSlider.value = bubblesLost + currentMisses;
        }
    }

    public void OnChangeGameMode(GameManager.GameMode currentGameMode)
    {
        arcadeMode = currentGameMode == GameManager.GameMode.ARCADE;
    }
}
