using System.Collections.Generic;
using CardHouse;
using TMPro;
using UnityEngine;

public class SnapCardSetup : CardSetup {
    
    public SnapCard card;
    [SerializeField] SpriteRenderer Image;
    [SerializeField] TextMeshPro NameLabel;
    
    private static int nextCardInstanceId = 1;
    public override void Apply(CardDefinition data)
    {
        if (data is SnapCardDefinition snapCardDef)
        {
            SnapCardStats stats = GetSnapCardStats(snapCardDef);
            card.initCardStats(stats);
            
            // Assign unique card instance ID
            card.SetCardInstanceId(nextCardInstanceId++);
            
            Image.sprite = snapCardDef.Art;
            NameLabel.text = snapCardDef.card_name;

            AbilityManager.Instance.SetUpAbilities(snapCardDef, card);
        }else{
            Debug.LogError("SnapCardSetup.Apply: CardDefinition is not SnapCardDefinition");
        }

    }

    private SnapCardStats GetSnapCardStats(SnapCardDefinition snapCardDef) {
        SnapCardStats stats = new SnapCardStats(snapCardDef.card_name, snapCardDef.series, snapCardDef.card_id);
        if (snapCardDef.Stats != null)
        {
            foreach (var statModal in snapCardDef.Stats)
            {
                if (statModal == null || statModal.StatTypeId <= 0) continue;
                
                // Get the StatTypeModal from the registry
                var statType = StatTypeRegistry.Instance.GetStatTypeById(statModal.StatTypeId);
                if (statType == null) continue;
                
                int.TryParse(statModal.BaseValue, out int value);
                stats.AddStat(statType, value);
            }
        }

        // Fallback: ensure Cost and Power exist based on legacy fields if not provided in Stats
        if (!stats.HasStat("Cost") && snapCardDef.cost != 0)
        {
            stats.AddStat("Cost", snapCardDef.cost);
        }
        if (!stats.HasStat("Power") && snapCardDef.power != 0)
        {
            stats.AddStat("Power", snapCardDef.power);
        }

        return stats;
    }
}