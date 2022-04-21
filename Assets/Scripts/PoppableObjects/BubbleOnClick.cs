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

    protected override void OnEnable()
    {
        base.OnEnable();
        bubbleProperties = GetComponent<Bubble>();
    }

    public override void OnClickOnObject()
    {
        base.OnClickOnObject();
        animator.SetTrigger("pop");       
        playerScore.UpdateScore(bubbleProperties.Value, gameObject);
    }

    // Used by the animator once the popping animation has finished
    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
