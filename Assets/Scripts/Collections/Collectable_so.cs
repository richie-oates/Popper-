using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectable_so")]
public class Collectable_so : ScriptableObject
{
    [SerializeField] Sprite sprite;
    [SerializeField] int rarity;

    public bool HasBeenCollected;

    public bool CanSpawn = true;

    public Sprite SpriteThis { get { return sprite; } }

    public int Rarity { get { return rarity; } }
}