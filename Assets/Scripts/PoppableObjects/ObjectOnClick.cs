using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectOnClick : MonoBehaviour
{
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] onClickSounds;
    [SerializeField] protected bool objectHit;

    Camera cam;
    Collider2D myCollider2D;

    void Awake()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
    }

    protected virtual void Start()
    {
        audioSource = GetComponent<AudioSource>();
        myCollider2D = GetComponent<Collider2D>();
        objectHit = false;

        cam = Camera.main;
    }

    protected virtual void OnEnable()
    {
        objectHit = false;
    }

    // TODO: Add setting to choose input method

   /*void Update()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.PAUSED || objectHit)
            return;
#if UNITY_ANDROID

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                Vector3 worldPoint = cam.ScreenToWorldPoint(touch.position);
                if (myCollider2D.OverlapPoint(worldPoint))
                    OnClickOnObject();
            }
        }
#endif
        if (Input.GetMouseButtonDown(0))
        {
            Vector3 worldPoint = cam.ScreenToWorldPoint(Input.mousePosition);

            if (myCollider2D.OverlapPoint(worldPoint))
                OnClickOnObject();
        }
    }*/

    private void OnMouseDown()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.PAUSED || objectHit)
            return;
        OnClickOnObject();
    }

    public virtual void OnClickOnObject()
    {
        if (GameManager.Instance.CurrentGameState == GameManager.GameState.PAUSED || objectHit)
            return;

        objectHit = true;
        if (onClickSounds.Length > 0)
        {
            audioSource.PlayOneShot(onClickSounds[Random.Range(0, onClickSounds.Length)]);
        }

        EventBroker.CallHitObject(gameObject);
    }

    public virtual void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        // blank to be overriden
    }
}
