using System.Collections.Generic;
using CardHouse;
using TMPro;
using UnityEngine;

public class SnapCardSetup : CardSetup {
    
    public SnapCard card;
    [SerializeField] SpriteRenderer Image;
    [SerializeField] TextMeshPro NameLabel;
    public override void Apply(CardDefinition data)
    {
        if (data is SnapCardDefinition snapCardDef)
        {
            SnapCardStats stats = new SnapCardStats(snapCardDef.power, snapCardDef.cost, snapCardDef.card_name, snapCardDef.series);
            card.initCardStats(stats);
            Image.sprite = snapCardDef.Art;
            NameLabel.text = snapCardDef.card_name;

            List<AbilityDefinition> abilities = new List<AbilityDefinition>(snapCardDef.abilities);
            abilities.ForEach(abilityDef => {
                var ability = new Ability();
                ability.SetOwner(card);
                ability.SetUpDefinition(abilityDef);
                AbilityManager.Instance.RegisterAbility(ability);
            });
        }else{
            Debug.LogError("SnapCardSetup.Apply: CardDefinition is not SnapCardDefinition");
        }

    }
}