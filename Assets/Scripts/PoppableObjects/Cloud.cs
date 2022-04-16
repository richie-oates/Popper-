using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cloud : ObjectOnClick
{
    private int size;
    [SerializeField] float timeDelayBeforeDestroy;
    [SerializeField] float[] scales;                        // Array of scales for different sized clouds
    [SerializeField] private GameObject particlePrefab;     // particle effect prefab
    ObjectSpawner objectSpawner;                            // Needed to spawn smaller clouds

    // Property 'size' is an integer value signifying the different positions in the scales array
    // Setting the size uses the relevant scale values and applies them to the transform
    public int Size
    { get { return size; }
        set 
        { 
            size = value;
            transform.localScale = new Vector3(scales[size], scales[size], 0);   
        } 
    }

    public float[] Scales
    { get; }

    protected override void Start()
    {
        base.Start();
        objectSpawner = FindObjectOfType<ObjectSpawner>();
        //Needs to use a seperate audiosource because on click destroys itself before it would have chance to play a sound
        audioSource = GameObject.Find("SFX Audio Source").GetComponent<AudioSource>();
    }

    protected override void OnEnable()
    {
        base.OnEnable();
        Size = Random.Range(0, scales.Length);
    }

    private void OnDisable()
    {
        Size = scales.Length - 1;
    }

    public override void OnClickOnObject()
    {
        base.OnClickOnObject();
            if (size == 0)
            {
                // Particle effect when completely destroyed
                Instantiate(particlePrefab, transform.position, transform.rotation);
            }
            else // Spawns two smaller clouds
            {
                for (int i = 0; i < 2; i++)
                {
                    GameObject newCloud = objectSpawner.SpawnObject("Cloud");
                    if (newCloud)
                    {
                        newCloud.GetComponent<Cloud>().Size = size - 1;
                        newCloud.GetComponent<Cloud>().transform.position = transform.position;
                    }
                }
            }
            // Need a slight delay so android doesn't register erroneous misses if you hold your finger down too long
            StartCoroutine(DisableAfterDelay(timeDelayBeforeDestroy));     
    }

    IEnumerator DisableAfterDelay(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        gameObject.SetActive(false);
    }
}
