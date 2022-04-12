using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParabolaMovement : MonoBehaviour
{
	[Tooltip("Horizontal speed, in units/sec")]
	public float minSpeed, maxSpeed;

	[Tooltip("The range the height of the arc can be as ratio of screen height")]
	public float minArcHeight, maxArcHeight;

	[SerializeField] bool remainUpright = false;

	float arcHeight, speed;
	Vector3 startPos, targetPos, facingPos, headingDirection;
	[SerializeField] Vector2Variable screenBounds;
	Transform tran;
	bool movementFrozen;

    void Start()
    {
		GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
		RandomiseStartVariables();
		tran = transform;
	}

	void OnEnable()
	{
		RandomiseStartVariables();
	}

    void Update()
    {
		if (movementFrozen) return;
		Move();
    }

	void Move()
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
		if (remainUpright)
        {
			headingDirection = (facingPos - transform.position).normalized;
			Quaternion headingChange = Quaternion.FromToRotation(tran.up, headingDirection);
			tran.localRotation *= headingChange;
        }

		// Do something when we reach the target
		if (nextPos == targetPos) Arrived();
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
		float startY = -(screenBounds.Value.y + transform.localScale.y / 2);
		float startX = Random.Range(-1.5f * screenBounds.Value.x, 1.5f * screenBounds.Value.x);
		return new Vector3(startX, startY, 0);
	}


	Vector3 GetRandomTargetPos(Vector3 startPosition)
	{
		float targetX = Random.Range(0, screenBounds.Value.x);
		if (startPosition.x > 0) targetX *= -1;
		return new Vector3(targetX, -(screenBounds.Value.y + transform.localScale.y / 2), 0);
	}

	float GetRandomArcHeight()
	{
		return Random.Range(2 * screenBounds.Value.y * minArcHeight, 2 * screenBounds.Value.y * maxArcHeight);
	}

	void Arrived()
	{
		gameObject.GetComponent<Collectable>().GetCollectable_So.CanSpawn = true;
		Destroy(gameObject);
	}
}