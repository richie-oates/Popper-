using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Needs renaming
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
    public Vector2Variable screenBounds;

    float halfObjectHeight;
    float halfObjectWidth;

    private void OnEnable()
    {
        transform.position = DetermineStartPosition();
    }

    
    public Vector2 DetermineStartPosition()
    {
        halfObjectHeight = GetComponent<Collider2D>().bounds.size.y / 2;
        halfObjectWidth = GetComponent<Collider2D>().bounds.size.x / 2;

        if (spawnPosition == SpawnPosition.TOP)
        {
            return new Vector3(RandomX(), halfObjectHeight + screenBounds.Value.y, 0);
        }
        if (spawnPosition == SpawnPosition.BOTTOM)
        {
            return new Vector3(RandomX(), -halfObjectHeight - screenBounds.Value.y, 0);
        }
        if (spawnPosition == SpawnPosition.SIDES)
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
            return new Vector3(-(halfObjectWidth + screenBounds.Value.x), RandomY(), 0);
        }
        else
        { 
            return new Vector3((halfObjectWidth + screenBounds.Value.x), RandomY(), 0);
        }
    }

    private void OnDisable()
    {
        if (spawnPosition == SpawnPosition.LEFT || spawnPosition == SpawnPosition.RIGHT)
            spawnPosition = SpawnPosition.SIDES;
    }

    float RandomX()
    {
        return Random.Range(-screenBounds.Value.x + halfObjectWidth, screenBounds.Value.x - halfObjectWidth);
    }

    float RandomY()
    {
        return Random.Range(-screenBounds.Value.y + halfObjectHeight, screenBounds.Value.y - halfObjectHeight);
    }
}
