using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collection : MonoBehaviour
{
    [SerializeField] List<GameObject> collectables;
    [SerializeField] string googlePlayAchievementName;

    public void InitialiseCollection()
    {
        foreach (GameObject collectableGO in collectables)
        {
            var collectable = collectableGO.GetComponent<Collectable>();
            if (PlayerPrefs.GetInt(collectable.name) == 1)
            {
                collectable.SetAsCollected();
            }
        }
    }

    public int TotalCollectables
    {
        get { return collectables.Count; }
    }

    public List<GameObject> RemainingCollectables
    {
        get
        {
            List<GameObject> remaining = new List<GameObject>();
            foreach (GameObject collectable in collectables)
            {
                if (!collectable.GetComponent<Collectable>().HasBeenCollected)
                {
                    remaining.Add(collectable);
                }
            }
            return remaining;
        }
    }

    public List<GameObject> FoundCollectables
    {
        get
        {
            List<GameObject> found = new List<GameObject>();
            foreach (GameObject collectable in collectables)
            {
                if (!collectable.GetComponent<Collectable>().HasBeenCollected)
                {
                    found.Add(collectable);
                }
            }
            return found;
        }
    }

    public void SetAsComplete()
    {
        foreach (GameObject collectable in collectables)
        {
            collectable.GetComponent<Collectable>().SetAsCollected();
        }
    }

    public List<GameObject> Collectables
    {
        get { return collectables; }
    }

    public bool IsComplete
    {
        get { return RemainingCollectables.Count == 0; }
    }
}
