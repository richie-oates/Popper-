using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Used by the balloon animator to set the sprite in the sprite renderer from sets of different colours 
public class SpriteSelector : MonoBehaviour
{
    //custom class allows us to have an array of arrays in the inspector
    [System.Serializable]
    public class AnimationSprites
    {
        public Sprite[] sprites;

        public AnimationSprites (Sprite[] sprites)
        {
            this.sprites = sprites;
        }
    }

    [SerializeField]
    AnimationSprites[] animationSprites;
    int color;
    SpriteRenderer spriteRenderer;

    private void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    private void OnEnable()
    {
        // pick a random sprite set
        color = Random.Range(0, animationSprites.Length - 1);
    }

    // called by the animator to set the sprite in the animation sequence
    public void SetSprite(int spriteNumber)
    {
        spriteRenderer.sprite = animationSprites[color].sprites[spriteNumber];
    }
}
