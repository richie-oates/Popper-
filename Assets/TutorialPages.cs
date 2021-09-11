using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class TutorialPages : MonoBehaviour
{
    [SerializeField] GameObject[] tutorialSlides;
    [SerializeField] TextMeshProUGUI tileNumberText;
    int currentSlide;

    private void OnEnable()
    {
        currentSlide = 0;
        ShowSlide(currentSlide);
    }

    public void ChangeSlide(bool right)
    {
        if (right)
        {
            currentSlide++;
            if (currentSlide >= tutorialSlides.Length)
                currentSlide = 0;
        }
        else
        {
            currentSlide--;
            if (currentSlide < 0)
                currentSlide = tutorialSlides.Length - 1;
        }

        ShowSlide(currentSlide);
    }

    private void ShowSlide(int slideNumber)
    {
        if (tutorialSlides.Length == 0)
            return;

        foreach (GameObject slide in tutorialSlides)
        {
            slide.SetActive(false);
        }
        tutorialSlides[slideNumber].SetActive(true);
        tileNumberText.text = (currentSlide + 1) + "/" + tutorialSlides.Length;
    }
}
