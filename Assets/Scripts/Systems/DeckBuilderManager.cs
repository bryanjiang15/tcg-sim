using UnityEngine;
using System.Collections.Generic;
using TMPro;

namespace CardLibrary
{
    public class DeckBuilderManager : Singleton<DeckBuilderManager>
    {
        [SerializeField] private CardLibraryGridManager cardLibraryGrid;
        [SerializeField] private DeckPanel deckPanel;
        [SerializeField] private TMP_InputField deckNameInput;
        private List<CardEntry> deckCards = new List<CardEntry>();

        private void Start()
        {
            if (cardLibraryGrid == null || deckPanel == null)
            {
                Debug.LogError("DeckBuilderManager: Missing required references!");
                return;
            }

            // Set default deck name if input field exists
            if (deckNameInput != null)
            {
                deckNameInput.text = "New Deck";
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

            // Get deck name from input field
            string deckName = "New Deck"; // Default fallback
            if (deckNameInput != null && !string.IsNullOrWhiteSpace(deckNameInput.text))
            {
                deckName = deckNameInput.text.Trim();
            }
            else
            {
                Debug.LogWarning("No deck name provided, using default name 'New Deck'");
            }

            CardLibraryManager.Instance.SaveDeck(deckName, deckCards);
            Debug.Log($"Deck '{deckName}' has been saved successfully!");
        }
    }
} 