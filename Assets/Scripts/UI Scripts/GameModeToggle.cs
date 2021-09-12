using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameModeToggle : MonoBehaviour
{
    [SerializeField] GameObject arcadeModeTitle, casualModeTitle;

    GameManager.GameMode currentGameMode;

    private void Start()
    {
        currentGameMode = GameManager.Instance.CurrentGameMode;
        SetTitle();
    }

     void SetTitle()
    {
        if (currentGameMode == GameManager.GameMode.ARCADE)
        {
            casualModeTitle.SetActive(false);
            arcadeModeTitle.SetActive(true);
        }
        else
        {
            arcadeModeTitle.SetActive(false);
            casualModeTitle.SetActive(true);
        }
    }

    public void SwitchGameMode()
    {
        if (currentGameMode == GameManager.GameMode.ARCADE)
        {
            currentGameMode = GameManager.GameMode.CASUAL;
            GameManager.Instance.ChangeGameMode(GameManager.GameMode.CASUAL);
        }
        else
        {
            currentGameMode = GameManager.GameMode.ARCADE;
            GameManager.Instance.ChangeGameMode(GameManager.GameMode.ARCADE);
        }
        SetTitle();
    }
}
