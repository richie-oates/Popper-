using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeactivateParent : MonoBehaviour
{
    public void Deactivate()
    {
        transform.parent.gameObject.SetActive(false);
    }

}
