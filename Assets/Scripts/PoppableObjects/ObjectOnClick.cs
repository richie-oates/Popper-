using UnityEngine;

public class ObjectOnClick : MonoBehaviour
{

    protected GameManager.GameState _currentState;
    [SerializeField] protected AudioSource audioSource;
    [SerializeField] protected AudioClip[] onClickSounds;
    [SerializeField] protected bool objectHit;

    protected virtual void Start()
    {
        // Add listener for GameState changes
        GameManager.Instance.OnGameStateChanged.AddListener(HandleGameStateChanged);
        audioSource = GetComponent<AudioSource>();
    }

    protected virtual void OnEnable()
    {
        objectHit = false;
    }

    public virtual void HandleGameStateChanged(GameManager.GameState currentState, GameManager.GameState previousState)
    {
        _currentState = currentState;
    }

    private void OnMouseDown()
    {
        if (_currentState == GameManager.GameState.PAUSED || objectHit)
            return;
        OnClickOnObject();    
    }

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
