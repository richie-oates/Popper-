using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : MonoBehaviour
{
    private int size;
    [SerializeField] private float largeScale, mediumScale, smallScale, tinyScale;
    [SerializeField] private int value, largeValue, mediumValue, smallValue, tinyValue;

    public int Size
    { get; set; }

    public int Value
    { get { return value; } }

    private void OnEnable()
    {
        SetRandomBubbleSize();
    }

    public void SetRandomBubbleSize()
    {
        size = Random.Range(0, 4);
        switch (size)
        {
            case 0:
                transform.localScale = new Vector3(largeScale, largeScale, 1);
                value = largeValue;
                break;
            case 1:
                transform.localScale = new Vector3(mediumScale, mediumScale, 1);
                value = mediumValue;
                break;
            case 2:
                transform.localScale = new Vector3(smallScale, smallScale, 1);
                value = smallValue;
                break;
            case 3:
                transform.localScale = new Vector3(tinyScale, tinyScale, 1);
                value = tinyValue;
                break;
            default:
                value = 100;
                break;
        }
    }
}
