using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetSpawnPosition : MonoBehaviour
{
    public enum SpawnPosition
    {
        TOP,
        BOTTOM,
        SIDES,
        LEFT,
        RIGHT
    }

    [SerializeField] SpawnPosition spawnPosition;
    Vector3 screenBounds;
    float halfObjectHeight;
    float halfObjectWidth;

    // Start is called before the first frame update
    void Start()
    {
        // Get reference to ScreenBounds
        screenBounds = GameManager.Instance.ScreenBounds;
        halfObjectHeight = GetComponent<Collider2D>().bounds.size.y / 2;
        halfObjectWidth = GetComponent<Collider2D>().bounds.size.x / 2;
    }

    private void OnEnable()
    {
        // Get reference to ScreenBounds
        screenBounds = GameManager.Instance.ScreenBounds;
        halfObjectHeight = GetComponent<Collider2D>().bounds.size.y / 2;
        halfObjectWidth = GetComponent<Collider2D>().bounds.size.x / 2;
        transform.position = GetStartPos();
    }

    // Returns position with a random x position and y position just above the screen bounds
    Vector2 GetStartPos()
    {
        if (spawnPosition == SpawnPosition.TOP)
        {
            return new Vector3(Random.Range(-screenBounds.x + halfObjectWidth, screenBounds.x - halfObjectWidth), halfObjectHeight + screenBounds.y, 0);
        }
        else if (spawnPosition == SpawnPosition.BOTTOM)
        {
            return new Vector3(Random.Range(-screenBounds.x + halfObjectWidth, screenBounds.x - halfObjectWidth), -halfObjectHeight - screenBounds.y, 0);
        }
        else if (spawnPosition == SpawnPosition.SIDES)
        {
            if (Random.Range(0, 2) == 1)
            {
                spawnPosition = SpawnPosition.LEFT;
            }
            else
            {
                spawnPosition = SpawnPosition.RIGHT;
            }
        }
        if (spawnPosition == SpawnPosition.LEFT)
        {
            return new Vector3(-(halfObjectWidth + screenBounds.x), Random.Range(-screenBounds.y + halfObjectHeight, screenBounds.y - halfObjectHeight), 0);
        }
        else
        {
            return new Vector3((halfObjectWidth + screenBounds.x), Random.Range(-screenBounds.y + halfObjectHeight, screenBounds.y - halfObjectHeight), 0);
        }
    }

}
