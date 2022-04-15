using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;


// TODO: In need of some refactoring, decoupling and splitting into smaller scripts

// Repetitively spawns objects from the pool at intervals which vary depending on user actions
// Sets the random range of speed for bubbles as well as size of bubbles and clouds
// Times and tracks the waves and spawns objects depending on which wave we're on

public class ObjectSpawner : MonoBehaviour
{
    // Speed variables; editable in editor
    [SerializeField] private float vertSpeed_max_initial = 4f, vertSpeed_min_initial = 0.1f, vertSpeed_max_limit = 6.0f, vertSpeed_min_limit = 3.0f;
    [SerializeField] private float vertSpeed_min_increase = 0.05f, vertSpeed_max_increase = 0.1f;
    [SerializeField] private float vertSpeed_min, vertSpeed_max;
    // Spawn interval variables
    [SerializeField] private float initialSpawnInterval = 1.0f, absolute_Min_SpawnInterval = 0.1f, spawnIntervalIncrement = 0.025f, current_spawnInterval;
    [SerializeField] private int spawnFrequencyIncreaseInterval = 10;
    // Wave time variables
    [SerializeField] private float waveTime = 30.0f, waveTime_increasePerLevel = 10.0f, timeBetweenWaves = 5.0f;
    [SerializeField] int waveTime_remaining;
    public int waveCounter = 1; // TODO: check if this really needs to be public
    
    // Text for debug HUD
    [SerializeField] TextMeshProUGUI waveText, waveTimeText;

    private Coroutine spawnTimerCoroutine, spawnFrequencyCoroutine, waveTimerCoroutine;
    private bool spawning; // flag to set if we are spawning or not
    private bool betweenWaves; // so we don't start spawning again when returning from frozen or paused state inbetween waves
    public bool gameOver; // game over has trigerred but we don't want to change state just yet. TODO: Check if this really needs to be public

    private void Start()
    {
        InitialiseVariables();
        // Add listener for GameState changes
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        // Starts the Bubble SpawnTimer
        spawning = true;
        spawnTimerCoroutine = StartCoroutine(SpawnTimer());
        // Event listeners
        EventBroker.BubblePopped += OnBubblePopped;
        EventBroker.ObjectLost += OnObjectLost;
        EventBroker.MissedEverything += OnMissedEverything;
        EventBroker.GameOverTriggered += OnGameOverTriggered;
    }

    private void InitialiseVariables()
    {
        // Reset variables
        gameOver = false;
        ResetSpeed();
        current_spawnInterval = initialSpawnInterval;
        foreach (ObjectPoolItem spawnableObject in ObjectPooler.SharedInstance.itemsToPool)
        {
            spawnableObject.spawnChance = spawnableObject.rarity;
        }
        waveCounter = 1;

        // Update text for debugHUD
        UpdateWaveText();

        // Stop coroutines
        if (spawnFrequencyCoroutine != null)
            StopCoroutine(spawnFrequencyCoroutine);
        if (waveTimerCoroutine != null)
            StopCoroutine(waveTimerCoroutine);
    }

    
    private void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        if (currentState == GameManager.GameState.PREGAME)
        {
            InitialiseVariables();
        }
        else if (currentState == GameManager.GameState.RUNNING && previousState == GameManager.GameState.PREGAME)
        {
            ResetSpeed();
            current_spawnInterval = initialSpawnInterval;
            // Clears all spawned objects from the screen
            foreach (GameObject item in ObjectPooler.SharedInstance.pooledObjects)
            {
                item.SetActive(false);
            }
            spawning = true;
            spawnFrequencyCoroutine = StartCoroutine(SpawnFrequencyIncrease());
            waveTimerCoroutine =  StartCoroutine(WaveTimer(waveTime + waveCounter * waveTime_increasePerLevel));
        }
        else if (currentState == GameManager.GameState.ENDGAME || currentState == GameManager.GameState.FROZEN || currentState == GameManager.GameState.PAUSED)
        {
            spawning = false;
        }
        else if (currentState == GameManager.GameState.RUNNING && (previousState == GameManager.GameState.FROZEN || previousState == GameManager.GameState.PAUSED))
        {
            if (!betweenWaves) spawning = true;
        }
    }

    // Repeating Coroutine which spawns objects
    IEnumerator SpawnTimer()
    {
        // Do nothing if we're not spawning
        while (!spawning) yield return null;

        // Wait a random period of time between 0.05 and the current spawnInterval value
        yield return new WaitForSeconds(Random.Range(0.05f, current_spawnInterval));
        // loop through each type of spawnable object (i.e bubble, cloud, bird etc)
        foreach (ObjectPoolItem spawnableObject in ObjectPooler.SharedInstance.itemsToPool)
        {
            // Check if the current wave is equal or higher than the wave the object is first introduced 
            if ( waveCounter >= spawnableObject.waveIntroduced)
            {
                // Make a random check to see if we spawn the object this time
                if (Random.Range(0, spawnableObject.spawnChance) == 0)
                {
                    SpawnObject(spawnableObject.objectToPool.tag);
                    // Reset the spawnChance variable back to its original rarity value
                    spawnableObject.spawnChance = spawnableObject.rarity;
                }
                else
                {
                    // decrease the value of spawnChance each time the object isn't spawned
                    // this creates a sort of controlled randomness
                    // each time it doesn't spawn, it's more likely to spawn the next time
                    // up to a point where it will certainly spawn
                    spawnableObject.spawnChance--;
                }
            }
        }
        // Start the coroutine again
        StartCoroutine(SpawnTimer());
    }

    IEnumerator WaveTimer(float time)
    {
        // Countdown timer updates text for DebugHUD every second
        float timer = time;
        waveTime_remaining = (int)timer;
        UpdateWaveText();
        while (timer > 0.0f)
        {
            while (!spawning) yield return null;
            yield return new WaitForSeconds(1);
            timer--;
            waveTime_remaining = (int)timer;
            UpdateWaveText();
        }
        // Once timer hits zero:
        // Increment wave counter
        waveCounter++;
        UpdateWaveText();
        // Reset the spawn interval
        current_spawnInterval = initialSpawnInterval;
        ResetSpeed();

        // Pause spawning for a certain amount of time and until the screen is mostly clear
        spawning = false;
        betweenWaves = true;
        yield return new WaitForSeconds(timeBetweenWaves);
        // Check that most of the objects have been cleared from the screen
        while (ActiveObjectsInScene() > 5) yield return null;
        // Restart spawning
        spawning = true;
        betweenWaves = false;
        // Restart the wave timer
        waveTimerCoroutine = StartCoroutine(WaveTimer(waveTime + waveTime_increasePerLevel * waveCounter));
    }

    // Increases the spawn frequency automatically
    IEnumerator SpawnFrequencyIncrease()
    {
        while (!spawning) yield return null;

        yield return new WaitForSeconds(spawnFrequencyIncreaseInterval);
        DecreaseSpawnInterval();
        spawnFrequencyCoroutine = StartCoroutine(SpawnFrequencyIncrease());
    }

    // Spawns an object from the object pool
    public GameObject SpawnObject(string objectName)
    {
        // returns null if we're not spawning
        if (!spawning) return null;

        // Gets an object from the object pool
        GameObject spawnedObject = ObjectPooler.SharedInstance.GetPooledObject(objectName);

        // returns null if there were no objects available in the pool
        if (spawnedObject == null) return null;

        // Sets random size and speed of bubble or random size of cloud
        // TODO: figure out how to decouple this
        if (objectName == "Bubble")
        { 
            spawnedObject.GetComponent<BubbleMovement>().SetVertSpeed(Random.Range(vertSpeed_min, vertSpeed_max));
        }

        spawnedObject.SetActive(true);
        return spawnedObject;  
    }

    public void OnBubblePopped()
    {
        // TODO: only do this on certain difficulty levels
        IncreaseSpeed();
    }

    public void OnObjectLost(OutOfBoundsEventArgs args)
    {
        // TODO: Difficulty levels
        if (args.objectTag == "Bubble" && args.screenSide == ScreenSides.Side.BOTTOM)
            IncreaseSpeed();
    }

    private void IncreaseSpeed()
    {
        vertSpeed_min = Mathf.Min(vertSpeed_min_limit, vertSpeed_min += vertSpeed_min_increase);
        vertSpeed_max = Mathf.Min(vertSpeed_max_limit, vertSpeed_max += vertSpeed_max_increase);
    }

    private void ResetSpeed()
    {
        vertSpeed_min = vertSpeed_min_initial;
        vertSpeed_max = vertSpeed_max_initial;
    }    

    private void DecreaseSpawnInterval()
    {
        if (!gameOver)
        {
            current_spawnInterval = Mathf.Max(current_spawnInterval -= spawnIntervalIncrement, absolute_Min_SpawnInterval);
        }
    }

    // Wave text just for debug HUD
    void UpdateWaveText()
    {
        waveText.text = "Wave: " + waveCounter;
        waveTimeText.text = "Wave time: " + waveTime_remaining;
    }

    // Returns a count of the number of active objects in the scene
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
        LeanTween.value(current_spawnInterval, 0.01f, 0.2f).setOnUpdate((float val) =>
        {
            current_spawnInterval = val;
        });
    }
    
    // Clicking and missing makes the game get harder
    // TODO: Difficulty levels
    public void OnMissedEverything()
    {
        DecreaseSpawnInterval();
        IncreaseSpeed();
    }
}
