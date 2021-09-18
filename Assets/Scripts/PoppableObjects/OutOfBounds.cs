using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Disables objects which go off the screen
// Broadcasts an event if the object is a bubble
// Listens for screen size changed events
public class OutOfBounds : MonoBehaviour
{
    protected Vector3 screenBounds;
    protected float halfObjectHeight, halfObjectWidth;

    // Start is called before the first frame update
    protected virtual void Start()
    {
        EventBroker.ScreenSizeChanged += OnScreenSizeChanged;
    }

    private void OnEnable()
    {
        // Get reference to ScreenBounds
        screenBounds = GameManager.Instance.ScreenBounds;
        halfObjectHeight = GetComponent<Collider2D>().bounds.size.y / 2;
        halfObjectWidth = GetComponent<Collider2D>().bounds.size.x / 2;
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (Mathf.Abs(transform.position.x) > screenBounds.x + halfObjectWidth + 1.0f ||
            Mathf.Abs(transform.position.y) > screenBounds.y + halfObjectHeight + 1.0f)
        {
            if (GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING && gameObject.CompareTag("Bubble"))
            {
                EventBroker.CallBubbleLost();
            }
            gameObject.SetActive(false);
        }
    }

    public void OnScreenSizeChanged(Vector3 newScreenBounds)
    {
        screenBounds = newScreenBounds;
    }
}
