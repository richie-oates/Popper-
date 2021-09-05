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
}
