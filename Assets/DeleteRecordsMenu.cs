using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeleteRecordsMenu : MonoBehaviour
{
    [SerializeField] Collections collections;

    public void ClearCollections()
    {
        EventBroker.CallClearCollections();
    }

    public void ClearRecords()
    {
        EventBroker.CallClearRecords();
    }
}
