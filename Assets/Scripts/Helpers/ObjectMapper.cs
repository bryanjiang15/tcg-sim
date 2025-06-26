using System.Linq;
using CardHouse;
using UnityEngine;
using CardLibrary;
using System.Collections.Generic;

public class ObjectMapper
{
    public static SnapCardDefinition GetSnapCardDefinition(SnapCardData cardData) {
        var cardDefinition = ScriptableObject.CreateInstance<SnapCardDefinition>();
        cardDefinition.cost = cardData.cost;
        cardDefinition.power = cardData.power;
        cardDefinition.card_name = cardData.card_name;
        cardDefinition.series = cardData.series;
        cardDefinition.abilities = cardData.abilities;
        return cardDefinition;
    }
    
    public static SnapCardData GetSnapCardData(SnapCardDefinition cardDefinition) {
        return new SnapCardData {
            cost = cardDefinition.cost,
            power = cardDefinition.power,
            card_name = cardDefinition.card_name,
            series = cardDefinition.series,
            abilities = cardDefinition.abilities,
            artPath = cardDefinition.artPath
        };
    }

    public static SnapDeckData GetSnapDeckData(DeckDefinition deckDefinition) {
        return new SnapDeckData {
            deckArtPath = deckDefinition.name,
            CardIds = deckDefinition.CardCollection.Select(card => card.cost).ToList()
        };
    }

    public static DeckDefinition GetDeckDefinition(SnapDeckData deckData) {
        var deckDefinition = ScriptableObject.CreateInstance<DeckDefinition>();
        deckDefinition.name = deckData.deckArtPath;
        deckDefinition.CardCollection = new List<SnapCardDefinition>();

        foreach (int cardId in deckData.CardIds) {
            deckDefinition.CardCollection.Add(CardLibraryManager.Instance.GetCard(cardId).getCardDefinition());
        }

        return deckDefinition;
    }

    public static SnapDeckLibraryData GetSnapDeckLibraryData(DeckLibrary deckLibrary) {
        return new SnapDeckLibraryData {
            decks = deckLibrary.decks.Select(deck => GetSnapDeckData(deck)).ToList()
        };
    }

    public static DeckLibrary GetDeckLibraryData(SnapDeckLibraryData snapDeckLibraryData) {
        return new DeckLibrary {
            decks = snapDeckLibraryData.decks.Select(deck => GetDeckDefinition(deck)).ToList()
        };
    }
}
