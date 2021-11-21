using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

// Inherits from Singleton and persists through scenes
// Handles game states and broadcasts when state is changed
// As with game modes
// Loads the main scene and shows a Loading screen and bar while this is happening
// Handles qutting the game/application

public class GameManager : Singleton<GameManager>
{
    private int screenWidth, screenHeight;
    private Vector2 screenBounds;
    private AsyncOperation screenLoading;
    [SerializeField] GameObject loadingScreen, loadingText;
    [SerializeField] ProgressBar loadingProgressBar;
    [SerializeField] CanvasGroup LoadingScreenAlphaCanvas;
    
    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED,
        FROZEN,
        QUIT,
        ENDGAME
    }

    public enum GameMode
    {
        ARCADE,
        CASUAL
    }

    GameMode _currentGameMode = GameMode.ARCADE;
    GameState _currentGameState = GameState.PREGAME;
    GameState returnAfterPause; // State to return to after unpausing game
    GameState returnOnCancel; // State to return to after cancelling quit game

    [System.Serializable] public class EventGameState : UnityEvent<GameState, GameState> { }
    public EventGameState OnGameStateChanged;
    
    public Vector2 ScreenBounds
    {
        get 
        { 
            screenBounds = GetScreenBounds(); 
            return screenBounds; 
        }
        private set { screenBounds = value; }
    }
    public GameState CurrentGameState
    {
        get { return _currentGameState; }
        private set { _currentGameState = value; }
    }

    public GameMode CurrentGameMode
    {
        get { return _currentGameMode; }
        private set { _currentGameMode = value; }
    }

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject); // Game Manager will persist after the main scene has loaded

        // Set the target frame rate dependent on device
#if UNITY_WEBGL || UNITY_ANDROID
        Application.targetFrameRate = -1;
#endif
        screenBounds = GetScreenBounds();

        // Activate loading screen
        loadingScreen.gameObject.SetActive(true);
    }

    public Vector2 GetScreenBounds()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        // Get the screenbounds from the main camera
        return Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    private void Start()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameAsync());
    }

    // Fades in the Loading Screen
    // Loads the main game scene
    public IEnumerator LoadGameAsync()
    {
        yield return new WaitForEndOfFrame();
        loadingText.SetActive(true);
        var seq = LeanTween.sequence();
        seq.append(LeanTween.alphaCanvas(LoadingScreenAlphaCanvas, 1, 0.5f));
        seq.append(() =>
        {
            screenLoading = SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN, LoadSceneMode.Additive);
            StartCoroutine(GetSceneLoadProgress());
        });
    }

    // Updates the loading bar as the main scene is loading
    // Fades out the loading scene when the main scene has finished loading
    // Starts the music once th emain scene is visible
    public IEnumerator GetSceneLoadProgress()
    {
        
        while(!screenLoading.isDone)
        {
            loadingProgressBar.current = Mathf.RoundToInt(screenLoading.progress * 100);
            
            yield return null; 
        }
        loadingProgressBar.current = 100;
        yield return new WaitForSeconds(0.5f);
        var seq = LeanTween.sequence();
        seq.append(LeanTween.alphaCanvas(LoadingScreenAlphaCanvas, 0, 0.5f));
        seq.append(() =>
        {
            loadingScreen.gameObject.SetActive(false);
        });

        GameObject.FindObjectOfType<MusicController>().PlayMenuMusic();
    }

    private void Update()
    {
        // Press escape / Android back button to quit game, open the quit menu, or pause menu depending on current state
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Escape pressed");
            if (_currentGameState == GameState.QUIT)
                QuitGame(true);
            else if (_currentGameState == GameState.RUNNING)
                PauseToggle();
            else
            {
                QuitGameQuery();
            }
        }

        // Check to see if screen size changed
        if (screenHeight != Screen.height || screenWidth != Screen.width)
        {
            ScreenSizeChanged();
        }
    }

    private void ScreenSizeChanged()
    {
        GetScreenBounds();
        EventBroker.CallScreenSizeChanged(screenBounds);
        Debug.Log("New Screen size: " + screenWidth + " x " + screenHeight);
    }

    public void PauseToggle()
    {
        // This makes sure we return to the correct state after unpausing the game
        switch (_currentGameState)
        {
            case GameState.RUNNING:
                returnAfterPause = _currentGameState;
                UpdateState(GameState.PAUSED);
                break;
            case GameState.FROZEN:
                returnAfterPause = _currentGameState;
                UpdateState(GameState.PAUSED);
                break;
            case GameState.PAUSED:
                UpdateState(returnAfterPause);
                break;
            default:
                break;
        }
    }

    // Changes game mode, currently between arcade and casual
    // Calls on the Eventbroker class to broadcast a message notifying of this change
    public void ChangeGameMode(GameMode gameMode)
    {
        _currentGameMode = gameMode;
        EventBroker.CallChangeGameMode(gameMode);
    }

    // Changes the game state and sends the event to any listeners
    public void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;
        OnGameStateChanged.Invoke(_currentGameState, previousGameState);
    }

    // Takes a reference to current state in case we need to return to it
    // Changes to Quit game state
    public void QuitGameQuery()
    {
        returnOnCancel = _currentGameState;
        UpdateState(GameState.QUIT);
    }

    // Quits game or returns to previous state
    public void QuitGame(bool quit)
    {
        if (quit)
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }
        else
            UpdateState(returnOnCancel);
    }
}

