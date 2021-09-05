using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBackground : MonoBehaviour
{
    SpriteRenderer spriteRenderer;
    [SerializeField] float scrollSpeed;
    float spriteWidth;
    Vector3 startPos;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        spriteWidth = spriteRenderer.sprite.bounds.size.x;
        startPos = transform.position;
    }

    private void Update()
    {
        transform.Translate(Vector3.left * Time.deltaTime * scrollSpeed);
        if (transform.position.x <= startPos.x - spriteWidth)
            transform.position = startPos;
    }
}
