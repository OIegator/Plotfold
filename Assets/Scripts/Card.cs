using System;
using System.Collections.Generic;
using UnityEngine;

public enum Tag
{
    Default,
    Transport,
    Route,
    Weapon
}

[Serializable]
public class TagIntPair
{
    public Tag tag;
    public int value;
}

[CreateAssetMenu(fileName = "New Card", menuName = "Cards/Card", order = 1)]
public class Card : ScriptableObject
{
    public Sprite image;
    public int id;
    public string description;
    public List<TagIntPair> tags;
}
