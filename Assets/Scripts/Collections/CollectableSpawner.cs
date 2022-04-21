using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawner : MonoBehaviour
{
    [SerializeField] Collections collections;
    [SerializeField] GameObject collectablePrefab;

    [Tooltip("Time(seconds) between spawn attempts")]
    [SerializeField] float spawnRate = 1f;

    float spawnTimer = 0;
    [SerializeField] bool isSpawning;

    void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        ResetCollectablesChanceToSpawn();
    }

    void Update()
    {
        if (isSpawning)
        {
            spawnTimer += Time.deltaTime;
            if (spawnTimer >= spawnRate)
            {
                if (collections.CurrentCollection != null)
                {
                    SpawnCollectables(collections.CurrentCollection.RemainingCollectables());
                    spawnTimer = 0;
                }
            }
        }
    }

    void ResetCollectablesChanceToSpawn()
    {
        if(collections.CurrentCollection != null)
        {
            foreach(Collectable_so collectable_So in collections.CurrentCollection.RemainingCollectables())
            {
                collectable_So.ResetChanceToSpawn();
            }
        }
    }

    void SpawnCollectables(List<Collectable_so> collectablesToSpawn)
    {
        foreach (Collectable_so collectable in collectablesToSpawn)
        {
            if (collectable.CanSpawn && ObjectSpawner.waveCounter >= collectable.MinimumLevelForSpawning)
            {
                if (Random.Range(0, collectable.ChanceToSpawn) == collectable.ChanceToSpawn - 1)
                {
                    GameObject obj = Instantiate(collectablePrefab);
                    collectable.CanSpawn = false;
                    obj.GetComponent<Collectable>().SetCollectable_so(collectable);
                    collectable.ResetChanceToSpawn();
                }
                else
                {
                    collectable.IncrementChanceToSpawn();
                }
            }
            
        }

        if (collections.CurrentCollection.RemainingCollectables().Count == 0)
        {
            PlayerPrefs.SetInt(collections.CurrentCollection.name, 1);
            collections.CurrentCollection.SetAsComplete();
            ResetCollectablesChanceToSpawn();
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
