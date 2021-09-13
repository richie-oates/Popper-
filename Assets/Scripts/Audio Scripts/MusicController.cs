using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MusicController : MonoBehaviour
{
    private GameManager.GameState _currentState;
    [SerializeField] AudioSource audioSource;
    [SerializeField] AudioClip[] gameMusic;
    [SerializeField] AudioClip gameOverMusic;
    [SerializeField] TextMeshProUGUI gameTrackText, menuTrackText;
    int gameMusicTrackNumber = 3, menuMusicTrackNumber = 1;
    float previousVolume;
    [SerializeField] GameObject muteButton, unMuteButton;
    bool playMenuMusicOnExitMenu;
    [SerializeField] Button backButton;


    private void Start()
    {
        EventBroker.GameOverTriggered += OnGameOverTriggered;
        // Add listener for GameState changes
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        // Get tracks from player prefs
        menuMusicTrackNumber = PlayerPrefs.GetInt("menuTrack", menuMusicTrackNumber);
        menuTrackText.text = "" + (menuMusicTrackNumber + 1);
        gameMusicTrackNumber = PlayerPrefs.GetInt("gameTrack", gameMusicTrackNumber);
        gameTrackText.text = "" + (gameMusicTrackNumber + 1);
        // Play menu music
        // PlayMenuMusic();
        backButton.onClick.AddListener(OnExitMenu);
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _currentState = currentState;

        switch (_currentState)
        {
            case GameManager.GameState.RUNNING:
                if ( previousState == GameManager.GameState.PREGAME)
                {
                    audioSource.clip = gameMusic[gameMusicTrackNumber];
                    audioSource.Play();
                }
                else 
                {
                    if (audioSource.clip == gameMusic[gameMusicTrackNumber])
                        audioSource.UnPause();
                    else
                    {
                        audioSource.clip = gameMusic[gameMusicTrackNumber];
                        audioSource.Play();
                    }
                }
                    
                break;

            case GameManager.GameState.PAUSED:
                audioSource.Pause();
                break;

            case GameManager.GameState.FROZEN:
                audioSource.Pause();
                break;

            case GameManager.GameState.PREGAME:
                PlayMenuMusic();
                break;

            case GameManager.GameState.ENDGAME:
                // PlayMenuMusic();
                break;

            default:
                audioSource.Play();
                break;
        }
    }

    public void OnGameOverTriggered()
    {
        audioSource.clip = gameOverMusic;
        audioSource.Play();
    }

    public void OnExitMenu()
    {
        // Save track numbers in player prefs
        PlayerPrefs.SetInt("menuTrack", menuMusicTrackNumber);
        PlayerPrefs.SetInt("gameTrack", gameMusicTrackNumber);
        
        // Restart menu music if we were demoing in game music
        if (playMenuMusicOnExitMenu)
            PlayMenuMusic();
    }

    public void PlayMenuMusic()
    {
        audioSource.clip = gameMusic[menuMusicTrackNumber];
        audioSource.Play();
    }

    public void MuteMusic()
    {
        previousVolume = audioSource.volume;
        audioSource.volume = 0;
        muteButton.SetActive(false);
        unMuteButton.SetActive(true);
    }

    public void UnmuteMusic()
    {
        audioSource.volume = previousVolume != 0 ? previousVolume : 1;
        unMuteButton.SetActive(false);
        muteButton.SetActive(true);
    }

    public void changeMenuMusic(bool positive)
    {
        playMenuMusicOnExitMenu = false;
        menuMusicTrackNumber = IncrementTrack(positive, gameMusic.Length, menuMusicTrackNumber);
        menuTrackText.text = "" + (menuMusicTrackNumber + 1);
        PlayMenuMusic();
    }

    public void changeGameMusic(bool positive)
    {
        playMenuMusicOnExitMenu = true;
        gameMusicTrackNumber = IncrementTrack(positive, gameMusic.Length, gameMusicTrackNumber);
        gameTrackText.text = "" + (gameMusicTrackNumber + 1);
        audioSource.clip = gameMusic[gameMusicTrackNumber];
        audioSource.Play();
    }

    public int IncrementTrack(bool positive, int max, int trackNumber)
    {
        if (positive)
        {
            trackNumber++;
            if (trackNumber >= max)
                trackNumber = 0;
        }
        else
        {
            trackNumber--;
            if (trackNumber < 0)
                trackNumber = max - 1;
        }

        return trackNumber;
    }
}
