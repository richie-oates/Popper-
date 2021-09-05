using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(CanvasGroup))]
[AddComponentMenu("UI/Blur Panel")]
public class BlurPanel : Image
{
    public bool animate = true;
    public float time = 0.5f;
    public float delay = 0f;
    public float blurRadius = 4.0f;

    new CanvasGroup canvas;

    protected override void Awake()
    {
        canvas = GetComponent<CanvasGroup>();
        base.Awake();
    }
    protected override void OnEnable()
    {
        base.OnEnable();

        if(Application.isPlaying && animate)
        {
            material.SetFloat("_Size", 0);
            canvas.alpha = 0;
            LeanTween.value(gameObject, UpdateBlur, 0, 1, time).setDelay(delay).setIgnoreTimeScale(true);
        }
    }

    void UpdateBlur(float value)
    {
        material.SetFloat("_Size", value * blurRadius);
        canvas.alpha = value;
    }
}
