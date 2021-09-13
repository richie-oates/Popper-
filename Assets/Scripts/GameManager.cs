using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;


public class GameManager : Singleton<GameManager>
{
    private Vector2 screenBounds;
    private AsyncOperation screenLoading;
    public GameObject loadingScreen, startButton, loadingText;
    public ProgressBar loadingProgressBar;
    public CanvasGroup alphaCanvas;
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
    GameState returnAfterPause, returnOnCancel;

    [System.Serializable] public class EventGameState : UnityEvent<GameState, GameState> { }
    public EventGameState OnGameStateChanged;
    

    public Vector2 ScreenBounds
    {
        get { return screenBounds; }
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

#if UNITY_WEBGL || UNITY_ANDROID
        Application.targetFrameRate = -1;
#else
        // Application.targetFrameRate = 60;
#endif
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
        loadingScreen.gameObject.SetActive(true);
    }

    private void Start()
    {
        LoadGame();
    }

    public void LoadGame()
    {
        StartCoroutine(LoadGameAsync());
    }

    public IEnumerator LoadGameAsync()
    {
        yield return new WaitForEndOfFrame();
        loadingText.SetActive(true);
        var seq = LeanTween.sequence();
        seq.append(LeanTween.alphaCanvas(alphaCanvas, 1, 0.5f));
        seq.append(() =>
        {
            screenLoading = SceneManager.LoadSceneAsync((int)SceneIndexes.MAIN, LoadSceneMode.Additive);
            StartCoroutine(GetSceneLoadProgress());
        });
    }


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
        seq.append(LeanTween.alphaCanvas(alphaCanvas, 0, 0.5f));
        seq.append(() =>
        {
            loadingScreen.gameObject.SetActive(false);
        });

        GameObject.FindObjectOfType<MusicController>().PlayMenuMusic();
    }

    private void Update()
    {
        // Press escape to exit application or exit play mode in editor
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


        // Press "P" to pause the game
        if(Input.GetKeyDown(KeyCode.P))
        {
            Debug.Log("P button pressed");
            PauseToggle();
        }
    }

    public void PauseToggle()
    {
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

    public void QuitGameQuery()
    {
        returnOnCancel = _currentGameState;
        UpdateState(GameState.QUIT);
    }

    
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

    public void ChangeGameMode(GameMode gameMode)
    {
        _currentGameMode = gameMode;
        EventBroker.CallChangeGameMode(gameMode);
        Debug.Log("Game Mode changed to: " + _currentGameMode);
    }

    public void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;
        Debug.Log("GameManager: Game state changed to " + _currentGameState);

        OnGameStateChanged.Invoke(_currentGameState, previousGameState);
    }
}

