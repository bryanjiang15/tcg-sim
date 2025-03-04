using CardHouse;
using UnityEngine;

[CreateAssetMenu(fileName = "SnapCardDefinition", menuName = "Cardhouse/Card Definition/SnapCard")]
public class SnapCardDefinition : CardDefinition {
    public int power;
    public int cost;
    public string card_name;
    public int series;

    public Sprite Art;
    
}