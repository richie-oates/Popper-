using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class RecordsMenu : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI highScoreText, highComboText;
    int highScore, highCombo;

    private void OnEnable()
    {
        highScore = PlayerPrefs.GetInt("highscore", highScore);
        highScoreText.text = String.Format("{0:#,###0}", highScore);
        highCombo = PlayerPrefs.GetInt("highCombo", highCombo);
        highComboText.text = String.Format("{0:#,###0}", highCombo);
    }
}
