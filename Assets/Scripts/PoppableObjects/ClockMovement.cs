using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClockMovement : MonoBehaviour
{
	[Tooltip("Horizontal speed, in units/sec")]
	public float minSpeed, maxSpeed;

	[Tooltip("The range the height of the arc can be as ratio of screen height")]
	public float minArcHeight, maxArcHeight;

	float arcHeight, speed;
	[SerializeField] float fallSpeed = 10;
	Vector3 startPos, targetPos, facingPos, headingDirection;
	Vector2 screenBounds;
	Transform spriteTransform;
	[SerializeField]  SpriteRenderer clockSpriteRenderer;
	[SerializeField] Sprite clockSprite;
	[SerializeField] Sprite clockHitSprite;
	bool movementFrozen;

	private bool broken;

	void Start()
	{
		GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
		spriteTransform = GetComponentInChildren<Transform>();
		
		screenBounds = GameManager.Instance.ScreenBounds;
		RandomiseStartVariables();		
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

	private void OnEnable()
    {
		clockSpriteRenderer.sprite = clockSprite;
		broken = false;
		RandomiseStartVariables();
	}

	void Update()
	{
		if (movementFrozen) return;
		if (!broken)
		{
			ParabolaMovement();
		}
		else
		{
			FallingMovement();
			
		}
	}

	void ParabolaMovement()
    {
		// Compute the next position, with arc added in
		float x0 = startPos.x;
		float x1 = targetPos.x;
		float dist = x1 - x0;
		float nextX = Mathf.MoveTowards(transform.position.x, x1, speed * Time.deltaTime);
		float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
		float arc = arcHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
		Vector3 nextPos = new Vector3(nextX, baseY + arc, transform.position.z);

		// Move to next position
		transform.position = nextPos;

		// Rotate to face facingPos
		headingDirection = (facingPos - transform.position).normalized;
		Quaternion headingChange = Quaternion.FromToRotation(spriteTransform.up, headingDirection);
		spriteTransform.localRotation *= headingChange;

		// Do something when we reach the target
		if (nextPos == targetPos) Arrived();
	}

	void FallingMovement()
    {
		transform.Translate(Vector3.down * fallSpeed * Time.deltaTime);
		if (transform.position.y < -screenBounds.y - 5.0f)
		{
			gameObject.SetActive(false);
		}
	}

	void RandomiseStartVariables()
    {
		// Get random start position
		startPos = GetRandomStartPos();
		transform.position = startPos;
		// Get target position based on start position
		targetPos = GetRandomTargetPos(startPos);
		// Get random arc height
		arcHeight = GetRandomArcHeight();
		// Get facingPos, a point above the peak of the arc
		facingPos = new Vector3((startPos.x - targetPos.x) / 2, arcHeight * 3.0f, 0); 
		// Set random speed
		speed = Random.Range(minSpeed, maxSpeed) * Mathf.Abs(startPos.x - targetPos.x) * 0.1f;
	}

	Vector3 GetRandomStartPos()
    {
		float startY = -(screenBounds.y + transform.localScale.y / 2);
		float startX = Random.Range(-1.5f * screenBounds.x, 1.5f * screenBounds.x);
		return new Vector3(startX, startY, 0);
    }


	Vector3 GetRandomTargetPos(Vector3 startPosition)
    {
		float targetX = Random.Range(0, screenBounds.x);
		if (startPosition.x > 0) targetX *= -1;
		return new Vector3(targetX, -(screenBounds.y + transform.localScale.y / 2), 0);
    }

	float GetRandomArcHeight()
    {
		return Random.Range(2 * screenBounds.y * minArcHeight, 2 * screenBounds.y * maxArcHeight);
    }

	void Arrived()
	{
		gameObject.SetActive(false);
	}

	public void BreakClock()
    {
		Debug.Log("ClockMovement: broken");
		clockSpriteRenderer.sprite = clockHitSprite;
		broken = true;
    }
}
