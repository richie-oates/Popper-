using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockOnClick : ObjectOnClick
{
    [SerializeField] int timeToFreeze;
    ClockMovement clockMovement;
    bool broken;
    [SerializeField] AudioClip[] breakSounds;
    [SerializeField] AudioClip[] boingSounds;
    [SerializeField] AudioClip ticking;
    Coroutine freezeTimeCoroutine;

    protected override void Start()
    {
        clockMovement = GetComponent<ClockMovement>();
        base.Start();
    }
    protected override void OnEnable()
    {
        base.OnEnable();
        broken = false;
    }
    protected override void OnClickOnObject()
    {
        if (!broken)
        {
            broken = true;
            clockMovement.BreakClock();
            if (breakSounds.Length > 0)
                audioSource.PlayOneShot(breakSounds[Random.Range(0, breakSounds.Length)]);
            if (boingSounds.Length > 0)
                audioSource.PlayOneShot(boingSounds[Random.Range(0, boingSounds.Length)]);
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING && !PlayerScore.Instance.gameOver)
            {
                freezeTimeCoroutine = StartCoroutine(FreezeTimeForSeconds(timeToFreeze));
            }
        }       
    }

    private IEnumerator FreezeTimeForSeconds(int seconds)
    {
        GameManager.Instance.UpdateState(GameManager.GameState.FROZEN);
        audioSource.PlayOneShot(ticking);
        yield return new WaitForSeconds(seconds);
        audioSource.Stop();
        GameManager.Instance.UpdateState(GameManager.GameState.RUNNING);
    }

    public override void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _currentState = currentState;
        if (currentState == GameManager.GameState.PAUSED && previousState == GameManager.GameState.FROZEN)
            audioSource.Pause();
        if (currentState == GameManager.GameState.FROZEN && previousState == GameManager.GameState.PAUSED)
            audioSource.UnPause();
        if (currentState == GameManager.GameState.ENDGAME)
        {
            if (freezeTimeCoroutine != null)
                StopCoroutine(freezeTimeCoroutine);
        }
    }
}
