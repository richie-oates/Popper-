using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndividualMenuController : MonoBehaviour
{
    public GameObject previousMenu;
    public GameObject[] menuItemsToTween;
    protected virtual void OnEnable()
    {
        if (menuItemsToTween.Length == 0)
            menuItemsToTween = new GameObject[] { gameObject };
        OpenMenu(gameObject);
    }
    public void OpenNewMenu(GameObject nextMenu)
    {
        nextMenu.GetComponent<IndividualMenuController>().previousMenu = gameObject;
        var seq = LeanTween.sequence();
        foreach (GameObject menuItem in menuItemsToTween)
        {
            seq = LeanTween.sequence();
            seq.append(LeanTween.scale(menuItem, new Vector3(1.1f, 1.1f, 1f), 0.1f).setIgnoreTimeScale(true)); // grow slightly
            seq.append(LeanTween.scale(menuItem, Vector3.zero, 0.3f).setIgnoreTimeScale(true)); // shrink to nothing
        }
        
        seq.append(() => 
        {
            nextMenu.SetActive(true);
            gameObject.SetActive(false);
        });      
    }
    public void OpenPreviousMenu()
    {
        var seq = LeanTween.sequence();
        foreach (GameObject menuItem in menuItemsToTween)
        {
            seq = LeanTween.sequence();
            seq.append(LeanTween.scale(menuItem, new Vector3(1.1f, 1.1f, 1f), 0.1f).setIgnoreTimeScale(true)); // do a tween
            seq.append(LeanTween.scale(menuItem, Vector3.zero, 0.3f).setIgnoreTimeScale(true)); // do a tween
        }
        seq.append(() =>
        {
            previousMenu.SetActive(true);
            gameObject.SetActive(false);
        });
    }

    private void OpenMenu(GameObject menu)
    {
        foreach (GameObject menuItem in menuItemsToTween)
            menuItem.transform.localScale = Vector3.zero;

        foreach (GameObject menuItem in menuItemsToTween)
        {
        var seq = LeanTween.sequence();
            seq.append(LeanTween.scale(menuItem, new Vector3(1.1f, 1.1f, 1f), 0.5f).setIgnoreTimeScale(true)); // do a tween
            seq.append(LeanTween.scale(menuItem, Vector3.one, 0.1f).setIgnoreTimeScale(true)); // do a tween
        }
        
    }

    public void CloseThisMenu()
    {
        var seq = LeanTween.sequence();
        foreach (GameObject menuItem in menuItemsToTween)
        {
            seq = LeanTween.sequence();
            seq.append(LeanTween.scale(menuItem, new Vector3(1.1f, 1.1f, 1f), 0.1f).setIgnoreTimeScale(true)); // do a tween
            seq.append(LeanTween.scale(menuItem, Vector3.zero, 0.3f).setIgnoreTimeScale(true)); // do a tween
        }
        seq.append(() => {
            gameObject.SetActive(false);
        });
    }
}
