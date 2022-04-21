using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UIManager : Singleton<UIManager>
{
    [SerializeField] GameObject dangerMeter, HUD, StartMenu, PauseMenu, GameOverMenu, SettingsMenu, QuitMenu, RecordsMenu, BackgroundBlur, TutorialMenu;
    AudioSource audioSource;
    [SerializeField] AudioClip[] buttonClickSounds;
    [SerializeField] Animator startMenuAnimator, comboLevelUpTextAnimator;
    GameManager.GameState _currentState;
    [SerializeField] float delayShowStartMenuTime = 5;
    [SerializeField] TextMeshProUGUI scoreText, highScoreText, accuracyText;
    public FloatingTextManager floatingTextManager;

    [SerializeField] PlayerStats_so playerStats_so;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        EventBroker.ComboLevelUp += OnComboLevelUp;
        // Add listener for GameState changes
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        // Show Start Menu at start of game after short delay
        StartCoroutine(DelayShowStartMenu());
        // Add listener for Game Mode changes
        EventBroker.ChangeGameMode += OnChangeGameMode;
    }

    IEnumerator DelayShowStartMenu()
    {
        yield return new WaitForSeconds(delayShowStartMenuTime);
        StartMenu.SetActive(true);
        BackgroundBlur.SetActive(true);
    }

    public void StartGame()
    {
        GameManager.Instance.UpdateState(GameManager.GameState.RUNNING);
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _currentState = currentState;

        if (currentState == GameManager.GameState.RUNNING)
        {
            BackgroundBlur.SetActive(false);
            HUD.SetActive(true);
            //PauseMenu.SetActive(false);
            GameOverMenu.SetActive(false);
        }
        if (currentState == GameManager.GameState.FROZEN)
        {
            BackgroundBlur.SetActive(false);
            PauseMenu.SetActive(false);
            GameOverMenu.SetActive(false);
        }
        if (currentState == GameManager.GameState.PAUSED)
        {
            HUD.SetActive(false);
            BackgroundBlur.SetActive(true);
            StartMenu.SetActive(false);
            PauseMenu.SetActive(true);
            GameOverMenu.SetActive(false);
        }
        if (currentState == GameManager.GameState.ENDGAME && previousState == GameManager.GameState.RUNNING)
        {
            HUD.SetActive(false);
            BackgroundBlur.SetActive(true);
            StartMenu.SetActive(false);
            PauseMenu.SetActive(false);
            GameOverMenu.SetActive(true);
            scoreText.text = String.Format("{0:#,###0}", PlayerScore.Instance.Score);
            highScoreText.text = String.Format("{0:#,###0}", PlayerScore.Instance.HighScore);
            accuracyText.text = String.Format("{0:0}", playerStats_so.AccuracyThisTurn) + " %";
        }
        if (currentState == GameManager.GameState.PREGAME)
        {
            HUD.SetActive(false);
            BackgroundBlur.SetActive(true);
            //StartMenu.SetActive(true);
            PauseMenu.SetActive(false);
            // GameOverMenu.SetActive(false);
            if (previousState == GameManager.GameState.QUIT)
                StartMenu.SetActive(true);
        }
        if (currentState == GameManager.GameState.QUIT)
        {
            HUD.SetActive(false);
            BackgroundBlur.SetActive(true);
            StartMenu.SetActive(false);
            PauseMenu.SetActive(false);
            GameOverMenu.SetActive(false);
            QuitMenu.SetActive(true);
            SettingsMenu.SetActive(false);
            RecordsMenu.SetActive(false);
            TutorialMenu.SetActive(false);
        }
    }

    public void PlayButtonSound()
    {
        audioSource.PlayOneShot(buttonClickSounds[UnityEngine.Random.Range(0, buttonClickSounds.Length)]);
    }

    public void OpenMenu(GameObject menu)
    {
        menu.SetActive(true);
    }

    public void UnpauseGame()
    {
        GameManager.Instance.PauseToggle();
    }

    public void BackToPregame()
    {
        GameManager.Instance.UpdateState(GameManager.GameState.PREGAME);
    }

    public void OnChangeGameMode(GameManager.GameMode gameMode)
    {
        dangerMeter.SetActive(gameMode == GameManager.GameMode.ARCADE);
    }

    public void OnComboLevelUp()
    {
        comboLevelUpTextAnimator.SetTrigger("ShowText");
    }

    // Floating Text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }

    public void ConfirmQuit(bool quit)
    {
        QuitMenu.SetActive(false);
        GameManager.Instance.QuitGame(quit);
    }
}
