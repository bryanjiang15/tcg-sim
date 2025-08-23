using System;
using System.Collections.Generic;
using CardHouse;
using UnityEngine;

[Serializable]
[CreateAssetMenu(fileName = "SnapCardDefinition", menuName = "CardHouse/Card Definition/SnapCard")]
public class SnapCardDefinition : CardDefinition {
    public int card_id;
    public int cost;
    public int power;
    public string card_name;
    public int series;
    public Sprite Art;
    public string artPath;
    public List<AbilityDefinition> abilities;
}