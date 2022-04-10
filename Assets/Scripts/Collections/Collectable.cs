using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : ObjectOnClick
{
    [SerializeField] int rarity;
    [SerializeField] float animationMoveSpeed = 1.5f;

    Animator animator;
    SpriteRenderer spriteRenderer;
    bool hasBeenCollected;

    void Awake()
    {
        animator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (PlayerPrefs.GetInt(name) == 1)
        {
            SetAsCollected();
        }
    }

    protected override void OnClickOnObject()
    {
        base.OnClickOnObject();
        if (hasBeenCollected) return;
        StartCoroutine(CollectActions());
    }

    IEnumerator CollectActions()
    {
        GameManager.Instance.UpdateState(GameManager.GameState.FROZEN);
        PlayerPrefs.SetInt(name, 1);
        SetAsCollected();

        spriteRenderer.sortingLayerName = "VeryFront";
        animator.Play("collectableFound");

        while (Mathf.Abs(transform.position.x) > 0.25 || Mathf.Abs(transform.position.y) > 0.25f)
        {
            transform.position = Vector3.MoveTowards(transform.position, Vector3.zero, animationMoveSpeed * Time.deltaTime);
            yield return new WaitForEndOfFrame();
        }
        transform.position = Vector3.zero;
        yield return new WaitForSeconds(2);
        UIManager.Instance.ShowText("You found the " + gameObject.name, 10, new Color(0.1137f, 0.1215f, 0.898f), transform.position + new Vector3(0, 2, 0), Vector3.up * 5, 2f);
        
        bool buttonPressed = false;
        while (!buttonPressed)
        {
            yield return new WaitForEndOfFrame();
            if (Input.anyKey) buttonPressed = true;
        }

        GameManager.Instance.UpdateState(GameManager.GameState.RUNNING);
        gameObject.SetActive(false);
    }

    public void SetAsCollected()
    {
        hasBeenCollected = true;
        print("Collected: " + gameObject);
    }

    #region Properties
    public int Rarity
    {
        get { return rarity; }
    }

    public bool HasBeenCollected
    {
        get { return hasBeenCollected; }
    }
    #endregion
}
