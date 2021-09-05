using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Audio;
using UnityEngine.Events;
using System;

public class SoundSettings : MonoBehaviour
{
    [SerializeField] Slider musicSlider, sfxSlider;
    [SerializeField] AudioMixer audioMixer;
    private float musicVolume = 0.75f, sfxVolume = 0.75f;

    private void Start()
    {
        // Get settings from player prefs
        if (PlayerPrefs.HasKey("musicVolume"))
            musicVolume = PlayerPrefs.GetFloat("musicVolume", musicVolume);
        SetMusicVolume(musicVolume);
        musicSlider.value = musicVolume;
        if (PlayerPrefs.HasKey("sfxVolume"))
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume", sfxVolume);
        SetSFXVolume(sfxVolume);
        sfxSlider.value = sfxVolume;

        musicSlider.onValueChanged.AddListener(SetMusicVolume);
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float value)
    {
        musicVolume = value;
        PlayerPrefs.SetFloat("musicVolume", musicVolume);
        // Mixer volume uses a logarithmic scale but we want the slider to be linear so we need a conversion
        var dbVolume = Mathf.Log10(value) * 20;
        if (value == 0.0f)
            dbVolume = -80.0f;
        audioMixer.SetFloat("musicVolume", dbVolume);
    }
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        PlayerPrefs.SetFloat("sfxVolume", sfxVolume);
        var dbVolume = Mathf.Log10(value) * 20;
        if (value == 0.0f)
            dbVolume = -80.0f;
        audioMixer.SetFloat("sfxVolume", dbVolume);
    }
}
