using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockOnClick : ObjectOnClick
{
    [SerializeField] int timeToFreeze = 5;
    [SerializeField] AudioClip[] breakSounds;
    [SerializeField] AudioClip[] boingSounds;
    [SerializeField] AudioClip ticking;
    Coroutine freezeTimeCoroutine;

    ClockMovement clockMovement;

    protected override void Start()
    {
        clockMovement = GetComponent<ClockMovement>();
        base.Start();
    }
    
    protected override void OnClickOnObject()
    {
        base.OnClickOnObject();
        if (breakSounds.Length > 0)
            audioSource.PlayOneShot(breakSounds[Random.Range(0, breakSounds.Length)]);
        if (boingSounds.Length > 0)
            audioSource.PlayOneShot(boingSounds[Random.Range(0, boingSounds.Length)]);
        if ((GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING
            || GameManager.Instance.CurrentGameState == GameManager.GameState.FROZEN) && !PlayerScore.Instance.gameOver)
        {
            print("ClockOnClick: checking for other freezetimecoroutines");
            ClockOnClick[] clocks = FindObjectsOfType<ClockOnClick>();
            foreach (ClockOnClick clock in clocks)
            {
                if (clock.freezeTimeCoroutine != null)
                {
                    clock.StopCoroutine(clock.freezeTimeCoroutine);
                    clock.audioSource.Stop();
                } 
            }
            freezeTimeCoroutine = StartCoroutine(FreezeTimeForSeconds(timeToFreeze));
        }
        clockMovement.BreakClock();
    }

    private IEnumerator FreezeTimeForSeconds(int seconds)
    {
        GameManager.Instance.UpdateState(GameManager.GameState.FROZEN);
        audioSource.PlayOneShot(ticking);
        yield return new WaitForSeconds(seconds);
        audioSource.Stop();
        while (_currentState != GameManager.GameState.FROZEN)
            yield return null;
        GameManager.Instance.UpdateState(GameManager.GameState.RUNNING);
    }

    public override void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _currentState = currentState;
        if (currentState == GameManager.GameState.PAUSED && previousState == GameManager.GameState.FROZEN)
        {
            audioSource.Pause();
        }
        if (currentState == GameManager.GameState.FROZEN && previousState == GameManager.GameState.PAUSED)
            audioSource.UnPause();
        
        if (currentState == GameManager.GameState.ENDGAME)
        {
            if (freezeTimeCoroutine != null)
                StopCoroutine(freezeTimeCoroutine);
        }
    }
}
