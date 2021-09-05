using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BirdOnClick : ObjectOnClick
{
    BirdMovement birdMovement;
    protected bool dead;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        birdMovement = GetComponent<BirdMovement>();
    }
    private void OnEnable()
    {
        dead = false;
    }

    protected override void OnClickOnObject()
    {
        if (!dead)
        {
            dead = true;
            base.OnClickOnObject();
            birdMovement.BirdHit();
        }       
    }
}
