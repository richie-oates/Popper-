using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EventBroker
{
    public static event Action BubblePopped;

    public static void CallBubblePopped()
    {
        BubblePopped?.Invoke();
    }

    public static event Action BubbleLost;

    public static void CallBubbleLost()
    {
        BubbleLost?.Invoke();
    }

    public static event Action<GameObject> HitObject;

    public static void CallHitObject(GameObject objectHit)
    {
        HitObject?.Invoke(objectHit);
    }

    public static event Action MissedEverything;

    public static void CallMissedEverything()
    {
        MissedEverything?.Invoke();
    }

    public static event Action<bool> VibrationSwitch;

    public static void CallVibrationSwitch(bool vibrationOn)
    {
        VibrationSwitch?.Invoke(vibrationOn);
    }

    public static event Action<GameManager.GameMode> ChangeGameMode;

    public static void CallChangeGameMode(GameManager.GameMode currentGameMode)
    {
        ChangeGameMode?.Invoke(currentGameMode);
    }

    public static event Action GameOverTriggered;

    public static void CallGameOverTriggered()
    {
        GameOverTriggered?.Invoke();
    }

    public static event Action ComboLevelUp;

    public static void CallComboLevelUp()
    {
        ComboLevelUp?.Invoke();
    }

    public static event Action<Vector3> ScreenSizeChanged;

    public static void CallScreenSizeChanged(Vector3 newScreenBounds)
    {
        ScreenSizeChanged?.Invoke(newScreenBounds);
    }
}
