using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashingScreen : MonoBehaviour
{
    Animation flashing;
    [SerializeField] SpriteRenderer spriteRenderer;
    [SerializeField] Color baseColor;
    // Start is called before the first frame update
    void Start()
    {
        flashing = GetComponent<Animation>();

        // Add listener for GameState changes
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (currentState == GameManager.GameState.FROZEN)
        {
            flashing.Play();
        }
        else if (previousState == GameManager.GameState.FROZEN)
        {
            flashing.Stop();
            spriteRenderer.color = baseColor;
        }
    }
}
