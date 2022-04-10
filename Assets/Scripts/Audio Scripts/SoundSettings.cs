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
        InitialiseMusicVolume();

        InitialiseSFXVolume();
    }

    private void InitialiseMusicVolume()
    {
        if (PlayerPrefs.HasKey("musicVolume"))
        {
            musicVolume = PlayerPrefs.GetFloat("musicVolume", musicVolume);
        }

        SetMusicVolume(musicVolume);
        musicSlider.value = musicVolume;
        musicSlider.onValueChanged.AddListener(SetMusicVolume);
    }

    private void InitialiseSFXVolume()
    {
        if (PlayerPrefs.HasKey("sfxVolume"))
        {
            sfxVolume = PlayerPrefs.GetFloat("sfxVolume", sfxVolume);
        }

        SetSFXVolume(sfxVolume);
        sfxSlider.value = sfxVolume;
        sfxSlider.onValueChanged.AddListener(SetSFXVolume);
    }

    public void SetMusicVolume(float newVolume)
    {
        musicVolume = newVolume;
        SetVolume("musicVolume", musicVolume);
    }
    public void SetSFXVolume(float value)
    {
        sfxVolume = value;
        SetVolume("sfxVolume", sfxVolume);
    }

    private void SetVolume(string audioName, float newVolume)
    {
        PlayerPrefs.SetFloat(audioName, newVolume);
        // Mixer volume uses a logarithmic scale but we want the slider to be linear so we need a conversion
        var dbVolume = Mathf.Log10(newVolume) * 20;
        if (newVolume == 0.0f)
            dbVolume = -80.0f;
        audioMixer.SetFloat(audioName, dbVolume);
    }
}
