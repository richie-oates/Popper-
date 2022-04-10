using UnityEngine;

// Class to hold variables which we want to pass with the event
public class OutOfBoundsEventArgs 
{
    public string objectTag;
    public ScreenSides.Side screenSide;

    public OutOfBoundsEventArgs(string _objectTag, ScreenSides.Side side)
    {
        objectTag = _objectTag;
        screenSide = side;
    }
}

// Checks to see if an object leaves the bounds of the screen
// Disables objects which go out of bounds
// Broadcasts an event, passing the object tag and which side of the boundary it left from
public class OutOfBounds : MonoBehaviour
{
    [Tooltip("Drop screenBounds scriptable object here")]
    public Vector2Variable screenBounds; // screenBounds is stored as a scriptable object
    private float halfObjectHeight, halfObjectWidth;
    
    // toggle in inspector to set which sides of the screen we want to check for objects going out of bounds
    [Tooltip("Out of bounds applies to this side of the screen")]
    [SerializeField] private bool top = true, bottom = true, left = true, right = true;

    [Tooltip("How far off the screen the object must go (as a percentage of screen size) before out of bounds is triggered")]
    [SerializeField] [Range(0, 10)] private float offsetPercentage = 1.0f;
    private float offset;

    // Enum indicating which side of the screen the object went off
    private ScreenSides.Side directionObjectWentOutOfBounds;

    [SerializeField] bool DebugModeOn;

    bool trackingObjectOnScreen = false;

    private void OnEnable()
    {
        // Get object sizes
        halfObjectHeight = GetComponent<Collider2D>().bounds.size.y / 2;
        halfObjectWidth = GetComponent<Collider2D>().bounds.size.x / 2;
        offset = 1.0f + offsetPercentage / 100f;

        if (!ObjectWentOutOfBounds())
        {
            trackingObjectOnScreen = true;
        }
    }

    private void Update()
    {
        if (!trackingObjectOnScreen)
        {
            if (!ObjectWentOutOfBounds())
                trackingObjectOnScreen = true;
            else
                return;
        }
        if (ObjectWentOutOfBounds())
        {
            EventBroker.CallObjectLost(new OutOfBoundsEventArgs(gameObject.tag, directionObjectWentOutOfBounds));
            gameObject.SetActive(false);
            if (DebugModeOn)
            {
                Debug.Log("Object out of bounds: " + gameObject + ". Screen side: " + directionObjectWentOutOfBounds);
            }
        }
    }

    bool ObjectWentOutOfBounds()
    {
        if (ObjectWentOffBottom)
        {
            directionObjectWentOutOfBounds = ScreenSides.Side.BOTTOM;
            return true;
        }
        if (ObjectWentOffTop)
        {
            directionObjectWentOutOfBounds = ScreenSides.Side.TOP;
            return true;
        }
        if (ObjectWentOffLeft)
        {
            directionObjectWentOutOfBounds = ScreenSides.Side.LEFT;
            return true;
        }
        if (ObjectWentOffRight)
        {
            directionObjectWentOutOfBounds = ScreenSides.Side.RIGHT;
            return true;
        }
        return false;
    }

    bool ObjectWentOffTop => top && transform.position.y > (screenBounds.Value.y * offset + halfObjectHeight);
    bool ObjectWentOffBottom => bottom && transform.position.y < (-screenBounds.Value.y * offset - halfObjectHeight);
    bool ObjectWentOffLeft => left && transform.position.x < (-screenBounds.Value.x * offset - halfObjectWidth);
    bool ObjectWentOffRight => right && transform.position.x > (screenBounds.Value.x * offset + halfObjectWidth);
}
