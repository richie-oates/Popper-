using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonOnClick : ObjectOnClick
{
    [SerializeField] float explosionRadius = 4.0f;
    ObjectPooler objectPooler;
    Animator animator;

    protected override void Start()
    {
        base.Start();
        objectPooler = FindObjectOfType<ObjectPooler>();
        animator = GetComponentInChildren<Animator>();
    }

    public override void OnClickOnObject()
    {
        base.OnClickOnObject();
        StartCoroutine(BalloonOnClickCoroutine());
        
    }    
    IEnumerator BalloonOnClickCoroutine()
    {
        animator.SetTrigger("pop"); // balloon pop animation
        gameObject.tag = "safe";    // change tag so it doesn't call onclick on itself again

        yield return new WaitForSeconds(0.1f);

        // Calls OnClick on all active objects within a certain distance of the popped balloon

        List<GameObject> objects = objectPooler.pooledObjects;
        foreach (GameObject pooledObject in objects)
        {
            float distance = Vector3.Distance(transform.position, (pooledObject.transform.position));
            if (!pooledObject.CompareTag("safe") && distance <= explosionRadius && pooledObject.activeInHierarchy == true)
            {
                // delay the onclick based on how far away each object is from the balloon
                yield return new WaitForSeconds(distance / 20);
                if (pooledObject.activeInHierarchy)
                    pooledObject.SendMessage("OnClickOnObject");
            }
        }
        gameObject.tag = "Balloon"; // Change the tag back so we can use it again in the object pooler
    }
}
