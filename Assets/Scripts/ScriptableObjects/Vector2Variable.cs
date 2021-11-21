using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Vector2 variable")]
public class Vector2Variable : ScriptableObject
{
    private Vector2 value;

    public Vector2 Value { get; set; }
}
