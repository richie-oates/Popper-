using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Stores the screen bounds as a Vector2 in a scriptable object so it's accessible to other scripts
// Checks for screen size changes and updates screen bounds if necessary
public class ScreenBoundsUpdater : MonoBehaviour
{
    private int screenWidth, screenHeight;
    
    public Vector2Variable screenBounds;
    

    private void Awake()
    {
        screenBounds.Value = GetScreenBounds();
    }

    private void Update()
    {
        // Check to see if screen size changed
        if (screenHeight != Screen.height || screenWidth != Screen.width)
        {
            screenBounds.Value = GetScreenBounds();
        }
    }

    private Vector2 GetScreenBounds()
    {
        screenWidth = Screen.width;
        screenHeight = Screen.height;
        // Get the screenbounds from the main camera
        return Camera.main.ScreenToWorldPoint(new Vector3(screenWidth, screenHeight, Camera.main.transform.position.z));
    }
}
