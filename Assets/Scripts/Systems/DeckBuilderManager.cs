using UnityEngine;
using CardLibrary;
using System.Collections.Generic;
using UnityEngine.UI;

namespace CardLibrary
{
    public class DeckBuilderManager : Singleton<DeckBuilderManager>
    {
        [SerializeField] private CardLibraryGridManager cardLibraryGrid;
        [SerializeField] private DeckPanel deckPanel;
        private List<CardEntry> deckCards = new List<CardEntry>();

        private void Start()
        {
            if (cardLibraryGrid == null || deckPanel == null)
            {
                Debug.LogError("DeckBuilderManager: Missing required references!");
                return;
            }

        }

        public void OnCardLibraryCardClicked(CardEntry card)
        {
            if (card == null) return;

            // Check if the card is already in the deck
            bool isInDeck = deckCards.Exists(c => c.cardId == card.cardId);

            if (isInDeck)
            {
                deckCards.Remove(card);
                deckPanel.RemoveCard(card);
            }
            else
            {                
                deckCards.Add(card);
                deckPanel.AddCard(card);
            }
        }

        public List<CardEntry> GetDeckCards()
        {
            return deckCards;
        }

        public void CompleteDeck()
        {
            if (deckCards.Count == 0)
            {
                Debug.LogWarning("Cannot save an empty deck!");
                return;
            }

            CardLibraryManager.Instance.SaveDeck("Deck", deckCards);
            Debug.Log($"Deck has been saved successfully!");
        }
    }
} 