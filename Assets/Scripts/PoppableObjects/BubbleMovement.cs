using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleMovement : MonoBehaviour
{
    private bool clockFrozen;
    protected Vector3 screenBounds;
    protected Coroutine coroutine;
    [SerializeField] protected float vertSpeed, horizontalSpeed, newHorizontalSpeed, maxHorizontalSpeed = 0.4f;
    bool bouncing, movementFrozen = false;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        screenBounds = GameManager.Instance.ScreenBounds;
        spriteRenderer = GetComponent<SpriteRenderer>();
        // Add listener for GameState changes
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        EventBroker.ScreenSizeChanged += OnScreenSizeChanged; 
    }

    public void OnScreenSizeChanged(Vector3 newScreenBounds)
    {
        screenBounds = newScreenBounds;
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

    protected virtual void OnEnable()
    {
        horizontalSpeed = Random.Range(-maxHorizontalSpeed, maxHorizontalSpeed);
        coroutine = StartCoroutine(ChangeDirection());
    }
    public void SetVertSpeed(float speed)
    {
        vertSpeed = -speed;
    }

    // Update is called once per frame
    protected void Update()
    {
        if (!movementFrozen)
        {
            transform.Translate(horizontalSpeed * Time.deltaTime, vertSpeed * Time.deltaTime, 0);

            if (Mathf.Abs(transform.position.x) > screenBounds.x - spriteRenderer.sprite.bounds.extents.x && !bouncing)
            {
                bouncing = true;
                StopCoroutine(coroutine);
                newHorizontalSpeed = -horizontalSpeed;
                StartCoroutine(ChangeValueOverTime(horizontalSpeed, newHorizontalSpeed, 1.5f));
            }
            else if (Mathf.Abs(transform.position.x) < screenBounds.x)
            {
                bouncing = false;
            }
        }
    }

    protected IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(Random.Range(2, 10));
        newHorizontalSpeed = Random.Range(-maxHorizontalSpeed, maxHorizontalSpeed);
        ChangeValueOverTime(horizontalSpeed, newHorizontalSpeed, 2.0f);
        StartCoroutine(ChangeDirection());
    }

    protected IEnumerator ChangeValueOverTime(float fromVal, float toVal, float duration)
    {
        float counter = 0f;

        while (counter < duration)
        {
            if (Time.timeScale == 0)
                counter += Time.unscaledDeltaTime;
            else
                counter += Time.deltaTime;

            horizontalSpeed = Mathf.Lerp(fromVal, toVal, counter / duration);
            yield return null;
        }
    }
}