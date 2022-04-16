using UnityEngine;

public class ObjectOnClick : MonoBehaviour
{

    protected GameManager.GameState _currentState;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] onClickSounds;
    [SerializeField] protected bool objectHit;

    Camera cam;
    Collider2D thisCollider;

    protected virtual void Start()
    {
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        audioSource = GetComponent<AudioSource>();
        objectHit = false;

        cam = Camera.main;
        thisCollider = GetComponent<Collider2D>();
    }

    protected virtual void OnEnable()
    {
        objectHit = false;
    }

#if UNITY_ANDROID
    void Update()
    {
        CheckTouchInput();
    }
#endif

    void CheckTouchInput()
    {
        if (_currentState == GameManager.GameState.PAUSED || objectHit)
            return;

        foreach (Touch touch in Input.touches)
        {
            if (touch.phase == TouchPhase.Began)
            {
                // Construct a ray from the current touch coordinates
                Ray ray = Camera.main.ScreenPointToRay(touch.position);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit))
                {
                    if (hit.collider == thisCollider)
                    {
                        OnClickOnObject();
                    }
                }
            }
        }
    }

    public virtual void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _currentState = currentState;
    }

#if UNITY_WEBGL || UNITY_EDITOR
    private void OnMouseDown()
    {
        if (_currentState == GameManager.GameState.PAUSED || objectHit)
            return;
        OnClickOnObject();
    }
#endif

    protected virtual void OnClickOnObject()
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
