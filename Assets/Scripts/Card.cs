using System.Collections.Generic;
using UnityEngine;

public enum Tag
{
    Default,
    Transport,
    Weapon
}

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card", order = 1)]
public class Card : ScriptableObject
{
    public Sprite image;
    public string id;
    public string description;
    public List<Tag> tags;
}