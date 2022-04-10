using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collections : MonoBehaviour
{
    [SerializeField] GameObject[] collectionPrefabs;
    List<Collection> allCollections;
    List<Collection> completeCollections;

    Collection currentCollection;

    void Awake()
    {
        completeCollections = new List<Collection>();
        allCollections = new List<Collection>();

        InitialiseCollections();
    }

    void InitialiseCollections()
    {
        for (int i = 0; i < collectionPrefabs.Length; i++)
        {
            GameObject go = Instantiate(collectionPrefabs[i]);
            allCollections.Add(go.GetComponent<Collection>());
            foreach (Collectable child in go.GetComponentsInChildren<Collectable>())
                child.gameObject.SetActive(false);
        }

        foreach (Collection collection in allCollections)
        {
            if (PlayerPrefs.GetInt(collection.name) == 1)
                collection.SetAsComplete();
            else
            {
                currentCollection = collection;
                currentCollection.InitialiseCollection();
                return;
            }
        }
    }

    void RefreshCollections()
    {
        foreach (Collection collection in allCollections)
        {
            if (collection.IsComplete)
            {
                completeCollections.Add(collection);
            }
            else
            {
                currentCollection = collection;
                return;
            }
        }
    }

    public Collection CurrentCollection
    {
        get 
        {
            RefreshCollections();
            return currentCollection; 
        }
    }
}
