using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField] Collections collections;
    [SerializeField] GameObject collectablePrefab;
    Collection_so currentCollection;

    [SerializeField] float spawnRate = 1f;

    float spawnTimer = 0;
    [SerializeField] bool isSpawning;

    void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        currentCollection = collections.CurrentCollection;
    }

    void Update()
    {
        if (isSpawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnRate)
            {
                SpawnCollectables(currentCollection.RemainingCollectables());
                spawnTimer = 0;
            }
        }
    }

    void SpawnCollectables(List<Collectable_so> collectablesToSpawn)
    {
        foreach (Collectable_so collectable in collectablesToSpawn)
        {
            if (collectable.CanSpawn && Random.Range(0, collectable.Rarity) == 0)
            {
                GameObject obj = Instantiate(collectablePrefab);
                collectable.CanSpawn = false;
                obj.GetComponent<Collectable>().SetCollectable_so(collectable);
            }
        }

        if (currentCollection.RemainingCollectables().Count == 0)
        {
            isSpawning = false;
            PlayerPrefs.SetInt(currentCollection.name, 1);
            currentCollection.SetAsComplete();
            currentCollection =  collections.CurrentCollection;
        }
    }

    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (currentState == GameManager.GameState.RUNNING)
        {
            isSpawning = true;
        }
        else 
        {
            isSpawning = false;
        }
    }
}
