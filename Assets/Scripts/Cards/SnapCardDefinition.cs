using CardHouse;
using UnityEngine;

[CreateAssetMenu(fileName = "SnapCardDefinition", menuName = "CardHouse/Card Definition/SnapCard")]
public class SnapCardDefinition : CardDefinition {
    public int cost;
    public int power;
    public string card_name;
    public int series;
    public Sprite Art;
    public AbilityDefinition[] abilities;
}