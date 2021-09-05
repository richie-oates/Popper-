using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BalloonOnClick : ObjectOnClick
{
    [SerializeField] float explosionRadius = 4.0f;
    ObjectPooler objectPooler;
    Animator animator;

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
        objectPooler = FindObjectOfType<ObjectPooler>();
        animator = GetComponentInChildren<Animator>();
    }

    protected override void OnClickOnObject()
    {
            animator.SetTrigger("pop"); // balloon pop animation
            gameObject.tag = "safe";    // change tag so it doesn't call onclick on itself again
            // Calls OnClick on all active objects within a certain distance of the popped balloon
            List<GameObject> objects = objectPooler.pooledObjects;
            foreach (GameObject pooledObject in objects)
            {
                float distance = Vector3.Distance(transform.position, (pooledObject.transform.position));
                if (!pooledObject.CompareTag("safe") && distance <= explosionRadius && pooledObject.activeInHierarchy == true)
                {
                    // delay the onclick based on how far away each object is from the balloon
                    StartCoroutine(WaitDontDoItYet(distance, pooledObject));
                }
            }
        gameObject.tag = "Balloon"; // Change the tag back so we can use it again in the object pooler
        base.OnClickOnObject();
    }    

    IEnumerator WaitDontDoItYet(float time, GameObject obj)
    {
        yield return new WaitForSeconds(time / 20);
        obj.SendMessage("OnClickOnObject");
    }
}
