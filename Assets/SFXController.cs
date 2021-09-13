using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SFXController : MonoBehaviour
{
    AudioSource audioSource;
    [SerializeField] AudioClip comboLevelUpSound;
    [SerializeField] AudioClip[] loseSounds;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
        EventBroker.ComboLevelUp += OnComboLevelUp;
        EventBroker.MissedEverything += OnMissedEverything;
    }

    public void OnComboLevelUp()
    {
        audioSource.PlayOneShot(comboLevelUpSound);
    }

    public void OnMissedEverything()
    {
        audioSource.PlayOneShot(loseSounds[Random.Range(0, loseSounds.Length - 1)]);
    }

}
