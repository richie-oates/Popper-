using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Collectable_so")]
public class Collectable_so : ScriptableObject
{
    [SerializeField] Sprite sprite;
    [SerializeField] int rarity;
    [SerializeField] Color textColor = Color.blue;
    [SerializeField] int minimumLevelForSpawning;
    int chanceToSpawn;

    public bool HasBeenCollected;

    public bool CanSpawn = true;

    public Sprite SpriteThis { get { return sprite; } }

    public int Rarity { get { return rarity; } }

    public int ChanceToSpawn { get { return chanceToSpawn; } }

    public Color TextColor { get { return textColor; } }

    public int MinimumLevelForSpawning { get { return minimumLevelForSpawning; } }

    public void ResetChanceToSpawn()
    {
        chanceToSpawn = rarity;
    }

    public void IncrementChanceToSpawn()
    {
        chanceToSpawn = Mathf.Max(chanceToSpawn -1, 1);
    }
}
