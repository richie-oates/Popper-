using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class GameManager : Singleton<GameManager>
{
    private Vector2 screenBounds;
    public enum GameState
    {
        PREGAME,
        RUNNING,
        PAUSED,
        FROZEN,
        ENDGAME
    }

    GameState returnAfterPause;

    [System.Serializable] public class EventGameState : UnityEvent<GameState, GameState> { }
    public EventGameState OnGameStateChanged;
    GameState _currentGameState = GameState.PREGAME;
    public FloatingTextManager floatingTextManager;

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

    protected override void Awake()
    {
#if UNITY_WEBGL
        Application.targetFrameRate = -1;
#else
        Application.targetFrameRate = 60;
#endif
        base.Awake();
        screenBounds = Camera.main.ScreenToWorldPoint(new Vector3(Screen.width, Screen.height, Camera.main.transform.position.z));
    }

    private void Update()
    {
        // Press escape to exit application or exit play mode in editor
        if (Input.GetKeyDown(KeyCode.Escape))
        {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
         }

        // Press enter to start the game
        if(Input.GetKeyDown(KeyCode.Return))
        {
            switch (_currentGameState)
            {
                case GameState.PREGAME:
                    UpdateState(GameState.RUNNING);
                    break;
                case GameState.ENDGAME:
                    UpdateState(GameState.PREGAME);
                    break;
                default:
                    break;
            }
        }

        // Press "P" to pause the game
        if(Input.GetKeyDown(KeyCode.P))
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
    }

    public void UpdateState(GameState state)
    {
        GameState previousGameState = _currentGameState;
        _currentGameState = state;
        Debug.Log("GameManager: Game state changed to " + _currentGameState);

        switch (_currentGameState)
        {
            case GameState.PREGAME:
                Time.timeScale = 1.0f;
                break;

            case GameState.RUNNING:
                Time.timeScale = 1.0f;
                break;

            case GameState.FROZEN:
                Time.timeScale = 1.0f;
                break;

            case GameState.PAUSED:
                Time.timeScale = 1.0f;
                break;

            case GameState.ENDGAME:
                Time.timeScale = 1.0f;
                break;

            default:
                break;
        }

        OnGameStateChanged.Invoke(_currentGameState, previousGameState);
    }

    // Floating Text
    public void ShowText(string msg, int fontSize, Color color, Vector3 position, Vector3 motion, float duration)
    {
        floatingTextManager.Show(msg, fontSize, color, position, motion, duration);
    }
}

