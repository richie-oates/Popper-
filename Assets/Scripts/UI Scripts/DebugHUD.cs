using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Shows some extra information in game, useful for debugging
public class DebugHUD : MonoBehaviour
{
    [SerializeField] GameObject debugHUD;
    [SerializeField] Toggle debugHUDToggle;

    private void Start()
    {
        debugHUDToggle.onValueChanged.AddListener(delegate {
            ToggleShowDebugHUD(debugHUDToggle);
        });
    }
    public void ToggleShowDebugHUD(Toggle toggle)
    {
        if (toggle.isOn)
        {
            debugHUD.SetActive(true);
        }
        else
        {
            debugHUD.SetActive(false);
        }
    }
}
