using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ButtonWithConfirm : MonoBehaviour
{
    [SerializeField] GameObject confirmBox;
    [SerializeField] GameObject parentObject;

    void Awake()
    {
        HideConfirmBox();
        confirmBox.transform.SetParent(parentObject.transform);
    }
    public void ShowConfirmBox()
    {
        confirmBox.SetActive(true);
    }

    public void HideConfirmBox()
    {
        confirmBox.SetActive(false);
    }
}
