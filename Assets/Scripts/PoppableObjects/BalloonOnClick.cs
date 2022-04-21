using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonOnClick : ObjectOnClick
{
    [SerializeField] float explosionRadius = 4.0f;
    Animator animator;
    [SerializeField] LayerMask poppableLayer;

    protected override void Start()
    {
        base.Start();
        animator = GetComponentInChildren<Animator>();
    }

    public override void OnClickOnObject()
    {
        base.OnClickOnObject();
        animator.SetTrigger("pop"); // balloon pop animation
        gameObject.tag = "safe";    // change tag so it doesn't call onclick on itself again

        Collider2D[] objectsInBlastRadius = Physics2D.OverlapCircleAll(transform.position, explosionRadius, poppableLayer);
        foreach (Collider2D collider in objectsInBlastRadius)
        {
            float distance = Vector3.Distance(transform.position, (collider.transform.position )) - collider.bounds.extents.x;
            if (!collider.CompareTag("safe"))
            {
                // delay the onclick based on how far away each object is from the balloon
                StartCoroutine(WaitDontDoItYet(distance, collider.gameObject));
            }
        }
        gameObject.tag = "Balloon"; // Change the tag back so we can use it again in the object pooler
    }

    IEnumerator WaitDontDoItYet(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time / 20);
        if (obj.activeInHierarchy)
            obj.SendMessage("OnClickOnObject");
    }
}