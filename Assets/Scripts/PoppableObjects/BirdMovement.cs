using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdMovement : MonoBehaviour
{
    protected Vector3 screenBounds;

    [SerializeField] float minHorizontalSpeed, maxHorizontalSpeed, horizontalSpeed;
    [SerializeField] float fallingSpeed;
    public bool birdHit;
    Animator animator;
    bool movementFrozen;



    public enum DirectionFacing
    {
        LEFT,
        RIGHT
    }

    SpriteRenderer spriteRenderer;


    // Start is called before the first frame update
    void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        screenBounds = GameManager.Instance.ScreenBounds;
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetStartConditions();
    }

    private void OnEnable()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetStartConditions();
    }

        private void SetStartConditions()
    {
        animator = GetComponent<Animator>();
        birdHit = false;
        if (transform.position.x >= screenBounds.x)
        {
            horizontalSpeed = -Random.Range(minHorizontalSpeed, maxHorizontalSpeed);
            SetSpriteDirection(DirectionFacing.LEFT);
        }
        else
        {
            horizontalSpeed = Random.Range(minHorizontalSpeed, maxHorizontalSpeed);
            SetSpriteDirection(DirectionFacing.RIGHT);
        }
        
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
	{
		if (currentState == GameManager.GameState.FROZEN || currentState == GameManager.GameState.PAUSED)
		{
			movementFrozen = true;
		}
		else if (currentState == GameManager.GameState.RUNNING)
		{
			movementFrozen = false;
		}
	}

    // Update is called once per frame
    void Update()
    {
        if (!movementFrozen)
        {
            if (!birdHit)
            {
                animator.SetBool("flying", true);
                transform.Translate(horizontalSpeed * Time.deltaTime, 0, 0);
            }
            else
            {
                animator.SetBool("flying", false);
                transform.Translate(0, -fallingSpeed * Time.deltaTime, 0);
            }
        }
    }
    public void BirdHit()
    { 
        birdHit = true;
        animator.SetBool("flying", false);
    }

    void SetSpriteDirection(DirectionFacing directionToFace)
    {
        if (directionToFace == DirectionFacing.LEFT)
            spriteRenderer.flipX = true;
        else
            spriteRenderer.flipX = false;
    }
}
