using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class ObjectSpawner : MonoBehaviour
{
    [SerializeField] private float initial_Max_VertSpeed = 4f, initial_Min_VertSpeed = 0.1f, absolute_Max_VertSpeed = 6.0f; 
    [SerializeField] private float initialSpawnInterval = 1.0f, absolute_Min_SpawnInterval = 0.1f, timeBetweenWaves = 5.0f;
    [SerializeField] private float wave_length_time = 30.0f, waveTimeIncreasePerLevel = 10.0f;
    public int waveCounter = 1;
    [SerializeField] private float min_VertSpeed, max_VertSpeed, spawnInterval;
    bool spawning; // flag to set if we are spawning or not
    Coroutine spawnTimerCoroutine, spawnFrequencyCoroutine, waveTimerCoroutine;
    [SerializeField] TextMeshProUGUI waveText, waveTimeText;
    [SerializeField] int waveTimeRemaining;
    private bool betweenWaves;
    public bool gameOver;
    private void Start()
    {
        InitialiseVariables();

        // Add listener for GameState changes
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        // Starts the Bubble SpawnTimer
        spawning = true;
        spawnTimerCoroutine = StartCoroutine(SpawnTimer());
        EventBroker.BubblePopped += IncreaseSpeed;
        EventBroker.BubbleLost += IncreaseSpeed;
        EventBroker.MissedEverything += OnMissedEverything;
        EventBroker.GameOverTriggered += OnGameOverTriggered;
    }

    private void InitialiseVariables()
    {
        gameOver = false;
        ResetSpeed();
        spawnInterval = initialSpawnInterval;
        if (spawnFrequencyCoroutine != null)
            StopCoroutine(spawnFrequencyCoroutine);
        waveCounter = 1;
        UpdateWaveText();
        if (waveTimerCoroutine != null)
            StopCoroutine(waveTimerCoroutine);
        foreach (ObjectPoolItem spawnableObject in ObjectPooler.SharedInstance.itemsToPool)
        {
            spawnableObject.spawnChance = spawnableObject.rarity;
        }
    }

    
    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        //  Reset speed and intervals on restart game
        if (currentState == GameManager.GameState.PREGAME)
        {
            Debug.Log("ObjectSpawner: GameState change registered");
            InitialiseVariables();
        }
        else if (currentState == GameManager.GameState.RUNNING && previousState == GameManager.GameState.PREGAME)
        {
            Debug.Log("ObjectSpawner: GameState change registered");
            foreach (GameObject item in ObjectPooler.SharedInstance.pooledObjects)
            {
                item.SetActive(false);
            }
            spawning = true;
            spawnFrequencyCoroutine = StartCoroutine(SpawnFrequencyIncrease());
            waveTimerCoroutine =  StartCoroutine(WaveTimer(wave_length_time + waveCounter * waveTimeIncreasePerLevel));
        }
        else if (currentState == GameManager.GameState.ENDGAME || currentState == GameManager.GameState.FROZEN || currentState == GameManager.GameState.PAUSED)
        {
            Debug.Log("ObjectSpawner: GameState change registered");
            spawning = false;
        }
        else if (currentState == GameManager.GameState.RUNNING && (previousState == GameManager.GameState.FROZEN || previousState == GameManager.GameState.PAUSED))
        {
            Debug.Log("ObjectSpawner: GameState change registered");
            if (!betweenWaves) spawning = true;
        }
    }

    // Repeating Coroutine which spawns objects
    IEnumerator SpawnTimer()
    {
        while (!spawning) yield return null;

        yield return new WaitForSeconds(Random.Range(0, spawnInterval));

        foreach (ObjectPoolItem spawnableObject in ObjectPooler.SharedInstance.itemsToPool)
        {
            if ( waveCounter >= spawnableObject.waveIntroduced)
            {
                if (Random.Range(0, spawnableObject.spawnChance) == 0)
                {
                    SpawnObject(spawnableObject.objectToPool.tag);
                    spawnableObject.spawnChance = spawnableObject.rarity;
                }
                else
                {
                    spawnableObject.spawnChance--;
                }
            }
        }

        StartCoroutine(SpawnTimer());
    }

    IEnumerator WaveTimer(float time)
    {
        float _time = time;
        waveTimeRemaining = (int)_time;
        UpdateWaveText();
        while (_time > 0.0f)
        {
            while (!spawning) yield return null;
            yield return new WaitForSeconds(1);
            _time--;
            waveTimeRemaining = (int)_time;
            UpdateWaveText();
        }
        waveCounter++;
        UpdateWaveText();
        spawnInterval = initialSpawnInterval;

        // Pause spawning for a certain amount of time and until the screen is clear
        spawning = false;
        betweenWaves = true;
        yield return new WaitForSeconds(timeBetweenWaves); 
        while (ActiveObjectsInScene() > 3) yield return null;
        spawning = true;
        betweenWaves = false;
        waveTimerCoroutine = StartCoroutine(WaveTimer(wave_length_time + waveTimeIncreasePerLevel * waveCounter));
    }

    IEnumerator SpawnFrequencyIncrease()
    {
        while (!spawning) yield return null;

        yield return new WaitForSeconds(10);
        DecreaseSpawnInterval();
        spawnFrequencyCoroutine = StartCoroutine(SpawnFrequencyIncrease());
    }

    IEnumerator PauseSpawning(float seconds)
    {
        spawning = false;
        yield return new WaitForSeconds(seconds);
        spawning = true;
    }

    // Spawns an object from the object pool
    public GameObject SpawnObject(string objectName)
    {
        if (spawning)
        {
            GameObject spawnedObject = ObjectPooler.SharedInstance.GetPooledObject(objectName);

            if (spawnedObject != null)
            {
                if (objectName == "Bubble")
                { 
                    spawnedObject.GetComponent<Bubble>().Size = Random.Range(0, 4);
                    spawnedObject.GetComponent<BubbleMovement>().SetVertSpeed(Random.Range(min_VertSpeed, max_VertSpeed));
                }
                else if (objectName == "Cloud")
                {
                    spawnedObject.GetComponent<Cloud>().Size = Random.Range(0, 3);
                }
                spawnedObject.SetActive(true);
                return spawnedObject;
            }   
        }
        return null;
    }
   
    private void IncreaseSpeed()
    {
        min_VertSpeed = Mathf.Min(3, min_VertSpeed += 0.05f);
        max_VertSpeed = Mathf.Min(absolute_Max_VertSpeed, max_VertSpeed += 0.1f);
    }

    private void ResetSpeed()
    {
        min_VertSpeed = initial_Min_VertSpeed;
        max_VertSpeed = initial_Max_VertSpeed;
    }    

    private void DecreaseSpawnInterval()
    {
        Debug.Log("ObjectSpawner: DecreaseSpawnInterval()");
        if (!gameOver)
        {
            spawnInterval = Mathf.Max(spawnInterval -= 0.05f, absolute_Min_SpawnInterval);
        }
    }

    void UpdateWaveText()
    {
        waveText.text = "Wave: " + waveCounter;
        waveTimeText.text = "Wave time: " + waveTimeRemaining;
    }

    private int ActiveObjectsInScene()
    {
        int count = 0;
        foreach (GameObject pooledObject in ObjectPooler.SharedInstance.pooledObjects)
        {
            if (pooledObject.activeInHierarchy) count++;
        }
        return count;
    }

    // Spawns lots of bubbles after game over has been triggered
    public void OnGameOverTriggered()
    {
        gameOver = true;
        LeanTween.value(spawnInterval, 0.01f, 0.5f).setOnUpdate((float val) =>
        {
            spawnInterval = val;
        });
    }
    
    public void OnMissedEverything()
    {
        DecreaseSpawnInterval();
        IncreaseSpeed();
    }
}
