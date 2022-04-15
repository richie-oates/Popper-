using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionsUI : MonoBehaviour
{
    [SerializeField] GameObject collectionPanelPrefab;
    [SerializeField] Collections collections;

    void OnEnable()
    {
        DestroyAllChildren();

        var currentCollection = collections.CurrentCollection;
        if (currentCollection != null && !currentCollection.IsComplete)
        {
            GameObject go = Instantiate(collectionPanelPrefab);
            go.transform.SetParent(transform);
            CollectionPanel currentPanel = go.GetComponent<CollectionPanel>();

            AddSprites(currentPanel, currentCollection, true);
            currentPanel.transform.localScale = Vector3.one;
        }

        foreach (Collection_so collection in collections.CompleteCollections)
        {
            GameObject newGO = Instantiate(collectionPanelPrefab);
            newGO.transform.SetParent(transform);
            CollectionPanel thisPanel = newGO.GetComponent<CollectionPanel>();

            AddSprites(thisPanel, collection, false);
            thisPanel.transform.localScale = Vector3.one;
        }

    }

    void AddSprites(CollectionPanel panel, Collection_so collection, bool incompleteCollection)
    {
        panel.SetText(collection.name);
        Sprite[] sprites = new Sprite[collection.Collectable_sos.Count];
        for (int i = 0; i < sprites.Length; i++)
        {
            var collectable = collection.Collectable_sos[i];
            sprites[i] = collectable.SpriteThis;
        }

        if (incompleteCollection)
        {
            bool[] shadows = new bool[sprites.Length];
            for (int i = 0; i < shadows.Length; i++)
            {
                var collectable = collection.Collectable_sos[i];
                shadows[i] = !collectable.HasBeenCollected;
            }

            panel.SetImages(sprites, shadows);
        }
        else
        {
            panel.SetImages(sprites);
        }
    }

    public void PlayServicesButton()
    {
        if (GameManager.Instance.IsConnectedToGooglePlayServices)
        {
            DisplayGooglePlayAchievements();

        }
        else
        {
            GameManager.Instance.SignInToGooglePlayServices();
        }
    }
    void DisplayGooglePlayAchievements()
    {
        Social.ShowAchievementsUI();
    }

    void DestroyAllChildren()
    {
        foreach (Transform child in transform)
            Destroy(child.gameObject);
    }
}
