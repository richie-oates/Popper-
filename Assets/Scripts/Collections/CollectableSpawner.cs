using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField] GameObject collectionPrefab;
    [SerializeField] Collections collections;
    Collection currentCollection;

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
                SpawnCollectables(currentCollection.RemainingCollectables);
                spawnTimer = 0;
            }
        }
    }

    void SpawnCollectables(List<GameObject> collectablesToSpawn)
    {
        foreach (GameObject collectable in collectablesToSpawn)
        {
            if (Random.Range(0, collectable.GetComponent<Collectable>().Rarity) == 0)
            {
                collectable.SetActive(true);
            }
        }

        if (currentCollection.RemainingCollectables.Count == 0)
            isSpawning = false;
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
