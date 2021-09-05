using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonMovement : BubbleMovement
{
    // Start is called before the first frame update
    protected override void OnEnable()
    {
        base.OnEnable();
        vertSpeed = 2.0f;
    }
}
