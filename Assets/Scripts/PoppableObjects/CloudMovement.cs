using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudMovement : MonoBehaviour
{
    protected Vector2 screenBounds;
    protected Coroutine coroutine;
    [SerializeField] float minHorizontalSpeed, maxHorizontalSpeed, horizontalSpeed, newHorizontalSpeed;
    [SerializeField] float minVerticalSpeed, maxVerticalSpeed, verticalSpeed, newVerticalSpeed;
    
    // Start is called before the first frame update
    void Start()
    {
        screenBounds = GameManager.Instance.ScreenBounds;
    }

    private void OnEnable()
    {
        screenBounds = GameManager.Instance.ScreenBounds;
        SetStartSpeed();
        coroutine = StartCoroutine(ChangeDirection());
        transform.position = new Vector3(transform.position.x, transform.position.y, -1.0f);
    }

    private void SetStartSpeed()
    {
        if (transform.position.x >= screenBounds.x)
        {
            horizontalSpeed = -Random.Range(minHorizontalSpeed, maxHorizontalSpeed);
        }
        else if (transform.position.x <= screenBounds.x)
        {
            horizontalSpeed = Random.Range(minHorizontalSpeed, maxHorizontalSpeed);
        }
        else
        {
            horizontalSpeed = (Random.Range(0, 2) == 1 ? 1 : -1) * Random.Range(minHorizontalSpeed, maxHorizontalSpeed);
        }

        verticalSpeed = (Random.Range(0, 2) == 1 ? 1 : -1) * Random.Range(minVerticalSpeed, maxVerticalSpeed);        
    }

    // Update is called once per frame
    void Update()
    {
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.FROZEN && GameManager.Instance.CurrentGameState != GameManager.GameState.PAUSED)
        {
            transform.Translate(horizontalSpeed * Time.deltaTime, verticalSpeed * Time.deltaTime, 0);
        }
    }

    protected IEnumerator ChangeDirection()
    {
        yield return new WaitForSeconds(Random.Range(5, 10));
        newHorizontalSpeed = (Random.Range(0, 2) == 1 ? 1 : -1) * Random.Range(minHorizontalSpeed, maxHorizontalSpeed);
        StartCoroutine(ChangeValueOverTime(horizontalSpeed, newHorizontalSpeed, 3f));
        newVerticalSpeed = (Random.Range(0, 2) == 1 ? 1 : -1) * Random.Range(minVerticalSpeed, maxVerticalSpeed);
        StartCoroutine(ChangeValueOverTime(verticalSpeed, newVerticalSpeed, 3f));
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
