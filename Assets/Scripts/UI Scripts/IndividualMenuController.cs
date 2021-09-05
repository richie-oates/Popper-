using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IndividualMenuController : MonoBehaviour
{
    public GameObject previousMenu;
    protected virtual void OnEnable()
    {
        OpenMenu(gameObject);
    }
    public void OpenNewMenu(GameObject nextMenu)
    {
        nextMenu.GetComponent<IndividualMenuController>().previousMenu = gameObject;
        var seq = LeanTween.sequence();
        seq.append(LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setIgnoreTimeScale(true)); // do a tween
        seq.append(LeanTween.scale(gameObject, Vector3.zero, 0.5f).setIgnoreTimeScale(true)); // do a tween
        seq.append(() => 
        {
            Debug.Log("Tween Finished: opening " + nextMenu.name + " menu");
            nextMenu.SetActive(true);
            gameObject.SetActive(false);
        });      
    }
    public void OpenPreviousMenu()
    {
        var seq = LeanTween.sequence();
        seq.append(LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setIgnoreTimeScale(true)); // do a tween
        seq.append(LeanTween.scale(gameObject, Vector3.zero, 0.5f).setIgnoreTimeScale(true)); // do a tween
        seq.append(() =>
        {
            Debug.Log("Tween Finished: opening " + previousMenu.name + " menu");
            previousMenu.SetActive(true);
            gameObject.SetActive(false);
        });
    }

    private void OpenMenu(GameObject menu)
    {
        transform.localScale = Vector3.zero;
        var seq = LeanTween.sequence();
        seq.append(LeanTween.scale(menu, new Vector3(1.1f, 1.1f, 1f), 0.5f).setIgnoreTimeScale(true)); // do a tween
        seq.append(LeanTween.scale(menu, Vector3.one, 0.1f).setIgnoreTimeScale(true)); // do a tween
    }

    public void CloseThisMenu()
    {
        var seq = LeanTween.sequence();
        seq.append(LeanTween.scale(gameObject, new Vector3(1.1f, 1.1f, 1f), 0.1f).setIgnoreTimeScale(true)); // do a tween
        seq.append(LeanTween.scale(gameObject, Vector3.zero, 0.5f).setIgnoreTimeScale(true)); // do a tween
        seq.append(() => {
            gameObject.SetActive(false);
        });
    }
}
