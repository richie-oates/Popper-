using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CollectionPanel : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI collectionTitleText;
    [SerializeField] Image[] collectableImages;
    [SerializeField] Color shadowColor;

    public void SetText(string text)
    {
        collectionTitleText.text = text;
    }

    public void SetImages(Sprite[] images)
    {
        for (int i = 0; i < images.Length; i++)
        {
            collectableImages[i].sprite = images[i];
        }
    }

    public void SetImages(Sprite[] images, bool[] isShadow)
    {
        for (int i = 0; i < images.Length; i++)
        {
            collectableImages[i].sprite = images[i];
            print("isShadow " + i + " " + isShadow[i]);
            if (isShadow[i])
            {
                
                collectableImages[i].color = shadowColor;
            }
        }
    }
}
