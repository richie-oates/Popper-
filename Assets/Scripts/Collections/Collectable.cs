using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : ObjectOnClick
{
    [SerializeField] Collectable_so collectable_so;
    [SerializeField] float animationMoveSpeed = 1.5f;
    [SerializeField] ParticleSystem particlePrefab;
    Animator animator;
    SpriteRenderer spriteRenderer;
    CircleCollider2D thisCollider;

    void Awake()
    {
        animator = GetComponent<Animator>();
        thisCollider = GetComponent<CircleCollider2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        InitialiseCollectable();
        if (PlayerPrefs.GetInt(name) == 1)
        {
            SetAsCollected();
        }
    }

    protected override void OnEnable()
    {
        base.OnEnable();

        InitialiseCollectable();
    }

    void InitialiseCollectable()
    {
        if(collectable_so != null)
        {
            name = collectable_so.name;
            spriteRenderer.sprite = collectable_so.SpriteThis;
            Vector2 size = spriteRenderer.sprite.bounds.extents;
            thisCollider.radius = Mathf.Max(size.x, size.y) * 1.2f;
        }
    }

    public override void OnClickOnObject()
    {
        if (HasBeenCollected)
        {
            print("Hit " + gameObject + " but has been collected");
                return;
        }
        if (GameManager.Instance.CurrentGameState != GameManager.GameState.RUNNING)
        {
            print("Not running");
            return;
        }

        if (PlayerScore.Instance.gameOver == true) return;
        base.OnClickOnObject();
        Instantiate(particlePrefab,transform.position, Quaternion.identity);
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
        UIManager.Instance.ShowText("You found the " + gameObject.name, 10, collectable_so.TextColor, transform.position + new Vector3(0, 1.25f, 0), Vector3.up * 5, 2f);
        
        bool buttonPressed = false;
        while (!buttonPressed)
        {
            yield return new WaitForEndOfFrame();
            if (Input.anyKey) buttonPressed = true;
        }

        GameManager.Instance.UpdateState(GameManager.GameState.RUNNING);
        Destroy(gameObject);
    }

    public void SetAsCollected()
    {
        HasBeenCollected = true;
        print("Collected: " + gameObject);
    }

    public void SetCollectable_so(Collectable_so so)
    {
        collectable_so = so;
        InitialiseCollectable();
    }

    #region Properties

    public Collectable_so GetCollectable_So
    {
        get { return collectable_so; }
    }
    public int Rarity
    {
        get { return collectable_so.Rarity; }
    }

    public bool HasBeenCollected
    {
        get { return collectable_so.HasBeenCollected; }
        set { collectable_so.HasBeenCollected = value; }
    }
    #endregion
}
