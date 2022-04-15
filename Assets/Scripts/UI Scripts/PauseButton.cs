using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Pause button needs to be held down for a certain time for it to trigger
// This is to avoid it being accidentally pressed when popping bubbles
public class PauseButton : MonoBehaviour
{
    float timer = 0;
    bool counting = false;
    [SerializeField] float holdTime = 1;
    [SerializeField] Animator helperTextAnimator;

    void Update()
    {
        if (counting) timer += Time.deltaTime;
        if (timer >= holdTime)
        {
            counting = false;
            timer = 0;
            GameManager.Instance.PauseToggle();
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
        if (helperTextAnimator != null)
            helperTextAnimator.Play("PauseHelperDisplay");
    }

    public void ShowHelperText()
    {
        if (helperTextAnimator != null)
            helperTextAnimator.SetTrigger("PauseHelperDisplay");
    }
}
