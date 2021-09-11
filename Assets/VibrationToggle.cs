using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VibrationToggle : MonoBehaviour
{
    [SerializeField] Slider vibrationSlider;
    bool vibrationOn = true;

    private void Start()
    {
        if (PlayerPrefs.HasKey("vibration"))
            vibrationOn = PlayerPrefs.GetFloat("vibration") > 0.0f;
        EventBroker.CallVibrationSwitch(vibrationOn);
        vibrationSlider.value = vibrationOn ? 1.0f : 0.0f;

        vibrationSlider.onValueChanged.AddListener(SetVibration);
    }

    public void SetVibration(float value)
    {
        bool vibrationBool = value > 0.0f;
        vibrationOn = vibrationBool;
        if (vibrationBool)
            Handheld.Vibrate();
        EventBroker.CallVibrationSwitch(vibrationBool);
    }

    public void SaveVibrationSetting()
    {
        PlayerPrefs.SetFloat("vibration", vibrationOn ? 1.0f : 0.0f);
    }
}
