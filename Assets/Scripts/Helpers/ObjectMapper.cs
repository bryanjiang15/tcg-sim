using System.Linq;
using CardHouse;
using UnityEngine;
using CardLibrary;
using System.Collections.Generic;
using Newtonsoft.Json;

public class ObjectMapper
{
    public static SnapCardDefinition GetSnapCardDefinition(SnapCardData cardData) {
        var cardDefinition = ScriptableObject.CreateInstance<SnapCardDefinition>();
        cardDefinition.cost = cardData.cost;
        cardDefinition.power = cardData.power;
        cardDefinition.card_name = cardData.card_name;
        cardDefinition.series = cardData.series;
        cardDefinition.abilities = cardData.abilities.Select(ability => GetAbilityDefinition(ability)).ToList();
        return cardDefinition;
    }
    
    public static SnapCardData GetSnapCardData(SnapCardDefinition cardDefinition) {
        return new SnapCardData {
            cost = cardDefinition.cost,
            power = cardDefinition.power,
            card_name = cardDefinition.card_name,
            series = cardDefinition.series,
            abilities = cardDefinition.abilities.Select(ability => GetSnapAbilityData(ability)).ToList(),
            artPath = cardDefinition.artPath
        };
    }

    public static SnapAbilityData GetSnapAbilityData(AbilityDefinition abilityDefinition) {
        return new SnapAbilityData {
            triggerDefinition = abilityDefinition.triggerDefinition,
            snapComponentData = abilityDefinition.snapComponentDefinitions.Select(
                block => JsonConvert.DeserializeObject<Dictionary<string, object>>(JsonConvert.SerializeObject(block))).ToList(),
            description = abilityDefinition.description
        };
    }

    public static AbilityDefinition GetAbilityDefinition(SnapAbilityData snapAbilityData) {
        var abilityDefinition = new AbilityDefinition();
        abilityDefinition.triggerDefinition = snapAbilityData.triggerDefinition;
        abilityDefinition.snapComponentDefinitions = snapAbilityData.GetBlockDefinitions();
        abilityDefinition.description = snapAbilityData.description;
        return abilityDefinition;
    }

    public static SnapDeckData GetSnapDeckData(DeckDefinition deckDefinition) {
        return new SnapDeckData {
            deckArtPath = deckDefinition.name,
            CardIds = deckDefinition.CardCollection.Select(card => card.card_id).ToList()
        };
    }

    public static DeckDefinition GetDeckDefinition(SnapDeckData deckData) {
        var deckDefinition = ScriptableObject.CreateInstance<DeckDefinition>();
        deckDefinition.name = deckData.deckArtPath;
        deckDefinition.CardCollection = new List<SnapCardDefinition>();

        foreach (int cardId in deckData.CardIds) {
            deckDefinition.CardCollection.Add(CardLibraryManager.Instance.GetCard(cardId));
        }

        return deckDefinition;
    }
}
