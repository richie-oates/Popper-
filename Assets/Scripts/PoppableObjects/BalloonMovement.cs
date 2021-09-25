using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonMovement : BubbleMovement
{
    protected override void OnEnable()
    {
        base.OnEnable();
        vertSpeed = Random.Range(1.5f, 2.5f);
    }
}
