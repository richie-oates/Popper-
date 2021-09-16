using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

// Attached to background so if it gets clicked it means everything else was missed
// Implements IPointerClickHandler so we can get the clicked location from the eventData
public class MissedEverything : MonoBehaviour, IPointerClickHandler
{
    PlayerScore playerScore;
    bool vibrationOn;

    private void Awake()
    {
        EventBroker.VibrationSwitch += OnVibrationSwitch;
    }

    void Start()
    {
        playerScore = FindObjectOfType<PlayerScore>();
    }

    public void OnVibrationSwitch(bool boolean)
    {
        vibrationOn = boolean;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.FROZEN || GameManager.Instance.CurrentGameState == GameManager.GameState.RUNNING)
        {
#if UNITY_ANDROID
            if (vibrationOn)
            {
                Handheld.Vibrate();
            }
#endif
            UIManager.Instance.ShowText("MISSED!", 50, Color.red, eventData.pointerCurrentRaycast.worldPosition, Vector3.up * 10, 0.5f);
            EventBroker.CallMissedEverything();
        }
    }
}
