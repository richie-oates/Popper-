using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collections : MonoBehaviour
{
    [SerializeField] List<Collection_so> allCollections;
    List<Collection_so> completeCollections;

    Collection_so currentCollection;

    void Awake()
    {
        completeCollections = new List<Collection_so>();

        InitialiseCollections();

        EventBroker.ClearCollections += OnClearCollections;
    }

    void OnDestroy()
    {
        EventBroker.ClearCollections -= OnClearCollections;

    }

    void InitialiseCollections()
    {
        completeCollections.Clear();

        foreach (Collection_so collection in allCollections)
        {
            if (PlayerPrefs.GetInt(collection.name) == 1)
            {
                collection.SetAsComplete();
                completeCollections.Add(collection);
            }
            else
            {
                currentCollection = collection;
                currentCollection.InitialiseCollection();
                return;
            }
        }
    }

    public void RefreshCollections()
    {
        completeCollections.Clear();

        foreach (Collection_so collection in allCollections)
        {
            if (collection.IsComplete)
            {
                completeCollections.Add(collection);
                PlayerPrefs.SetInt(collection.name, 1);
            }
            else
            {
                currentCollection = collection;
                return;
            }
        }
    }

    public void ResetCollections()
    {
        foreach (Collection_so collection_so in allCollections)
        {
            collection_so.ResetCollection();
            completeCollections.Clear();
            currentCollection = allCollections[0];
        }
    }

    void OnClearCollections()
    {
        ResetCollections();
    }

    public List<Collection_so> CompleteCollections
    {
        get
        {
            RefreshCollections();
            return completeCollections;
        }
    }

    public Collection_so CurrentCollection
    {
        get 
        {
            return currentCollection; 
        }
    }
}
