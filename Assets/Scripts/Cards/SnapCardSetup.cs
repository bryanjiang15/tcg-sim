using CardHouse;
using UnityEngine;

public class SnapCardSetup : CardSetup {
    
    public SnapCard card;
    public SpriteRenderer Image;
    public override void Apply(CardDefinition data)
    {
        if (data is SnapCardDefinition snapCardDef)
        {
            SnapCardStats stats = new SnapCardStats(snapCardDef.power, snapCardDef.cost, snapCardDef.card_name, snapCardDef.series);
            card.initCardStats(stats);
            Image.sprite = snapCardDef.Art;
        }else{
            Debug.LogError("SnapCardSetup.Apply: CardDefinition is not SnapCardDefinition");
        }

    }
}