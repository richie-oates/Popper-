using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleOnClick : ObjectOnClick
{
    PlayerScore playerScore;
    Bubble bubbleProperties;
    Animator animator;

    protected override void Start()
    {
        base.Start();
        playerScore = FindObjectOfType<PlayerScore>();
        animator = GetComponentInChildren<Animator>();
    }

    private void OnEnable()
    {
        bubbleProperties = GetComponent<Bubble>();
    }

    protected override void OnClickOnObject()
    {
        base.OnClickOnObject();
        animator.SetTrigger("pop");       
        playerScore.UpdateScore(bubbleProperties.Value, gameObject);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
