using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseButton : MonoBehaviour
{
    float timer = 0;
    bool counting = false;
    [SerializeField] float holdTime = 1;

    void Update()
    {
        if (counting) timer += Time.deltaTime;
        if (timer >= holdTime)
        {
            counting = false;
            timer = 0;
            GameManager.Instance.UpdateState(GameManager.GameState.PAUSED);
        }
    }


    public void OnPointerUp()
    {
        counting = false;
        timer = 0;
    }

    public void OnPointerDown()
    {
        counting = true;
    }
}
