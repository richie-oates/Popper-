using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectOnClick : MonoBehaviour
{

    protected GameManager.GameState _currentState;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] onClickSounds;
    [SerializeField] protected bool objectHit;

    Camera cam;
    Collider2D myCollider2D;


    protected virtual void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        audioSource = GetComponent<AudioSource>();
        myCollider2D = GetComponent<Collider2D>();
        objectHit = false;

        cam = Camera.main;
    }

    protected virtual void OnEnable()
    {
        objectHit = false;
    }

   void Update()
    {
        if (objectHit)
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
    }

    public virtual void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _currentState = currentState;
    }

/*#if UNITY_WEBGL || UNITY_EDITOR
    private void OnMouseDown()
    {
        if (_currentState == GameManager.GameState.PAUSED || objectHit)
            return;
        OnClickOnObject();
    }
#endif*/

    public virtual void OnClickOnObject()
    {
        if (_currentState == GameManager.GameState.PAUSED || objectHit)
            return;

        objectHit = true;
        if (onClickSounds.Length > 0)
        {
            audioSource.PlayOneShot(onClickSounds[Random.Range(0, onClickSounds.Length)]);
        }

        EventBroker.CallHitObject(gameObject);
    }
}
