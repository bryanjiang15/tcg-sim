using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using CardHouse;
using Newtonsoft.Json;

namespace CardLibrary
{
    public class CardLibraryManager : Singleton<CardLibraryManager>
    {
        private CardGenerator cardGenerator;
        private ArtLibraryManager artLibraryManager;

        protected override void Awake() 
        {
            base.Awake();
            cardGenerator = new CardGenerator();
            artLibraryManager = new ArtLibraryManager();
            artLibraryManager.InitializeArtLibrary();
            InitializeLibrary();
        }

        private void InitializeLibrary()
        {
            // Registries handle their own initialization in Awake()
            // Preload all card art for better performance
            artLibraryManager.PreloadAllCardArt();
            Debug.Log($"Preloaded {artLibraryManager.GetCachedSpriteCount()} card art sprites");
        }

        public void AddCard(SnapCardDefinition cardDefinition, bool isFoil = false)
        {
            // Create serializable data from the card definition
            var cardData = ObjectMapper.GetSnapCardData(cardDefinition);
            
            CardRegistry.Instance.InsertCard(cardData);
        }

        public void RemoveCard(int cardId)
        {
            CardRegistry.Instance.RemoveCard(cardId);
        }

        public SnapCardDefinition GetCard(int cardId)
        {
            var cardData = CardRegistry.Instance.GetCard(cardId);
            if (cardData == null) return null;
            
            // Convert SnapCardData back to CardEntry for compatibility
            return cardData.GetCardDefinition();
        }

        public List<CardEntry> GetAllCards()
        {
            var cardEntries = new List<CardEntry>();
            foreach (var cardData in CardRegistry.Instance.GetAllCards())
            {
                if (cardData != null)
                {
                    cardEntries.Add(new CardEntry
                    {
                        cardId = cardData.cardId,
                        cardData = cardData,
                        quantity = 1,
                        dateAcquired = DateTime.Now,
                        isFoil = false,
                        tags = new List<string>(),
                        stats = new Dictionary<string, int>()
                    });
                }
            }
            return cardEntries;
        }

        public void AddTagToCard(int cardId, string tag)
        {
            // Note: CardRegistry doesn't support tags directly
            // This method is kept for compatibility but doesn't persist tags
            Debug.LogWarning("AddTagToCard: Tags are not supported in CardRegistry. Use CardEntry for tag functionality.");
        }

        public void RemoveTagFromCard(int cardId, string tag)
        {
            // Note: CardRegistry doesn't support tags directly
            // This method is kept for compatibility but doesn't persist tags
            Debug.LogWarning("RemoveTagFromCard: Tags are not supported in CardRegistry. Use CardEntry for tag functionality.");
        }

        public void UpdateCardStats(int cardId, string statName, int value)
        {
            // Note: CardRegistry doesn't support stats directly
            // This method is kept for compatibility but doesn't persist stats
            Debug.LogWarning("UpdateCardStats: Stats are not supported in CardRegistry. Use CardEntry for stats functionality.");
        }

        public List<CardEntry> GetCardsByTag(string tag)
        {
            // Note: CardRegistry doesn't support tags directly
            // This method is kept for compatibility but returns empty list
            Debug.LogWarning("GetCardsByTag: Tags are not supported in CardRegistry. Use CardEntry for tag functionality.");
            return new List<CardEntry>();
        }

        public List<CardEntry> GetFoilCards()
        {
            // Note: CardRegistry doesn't support foil status directly
            // This method is kept for compatibility but returns empty list
            Debug.LogWarning("GetFoilCards: Foil status is not supported in CardRegistry. Use CardEntry for foil functionality.");
            return new List<CardEntry>();
        }

        public int GetTotalCardCount()
        {
            return CardRegistry.Instance.Count;
        }

        public Dictionary<string, int> GetCardStats(int cardId)
        {
            // Note: CardRegistry doesn't support stats directly
            // This method is kept for compatibility but returns empty dictionary
            Debug.LogWarning("GetCardStats: Stats are not supported in CardRegistry. Use CardEntry for stats functionality.");
            return new Dictionary<string, int>();
        }

        public CardGenerator GetCardGenerator()
        {
            return cardGenerator;
        }

        public void ClearLibrary()
        {
            // Clear all cards from CardRegistry
            var cardIds = CardRegistry.Instance.AllIds.ToList();
            foreach (var cardId in cardIds)
            {
                CardRegistry.Instance.RemoveCard(cardId);
            }
            
            Debug.Log("Card library has been cleared.");
        }

        public void SaveDeck(string deckName, List<CardEntry> cards)
        {
            if (string.IsNullOrEmpty(deckName) || cards == null || cards.Count == 0)
            {
                Debug.LogError("Invalid deck data provided for saving");
                return;
            }

            // Extract card IDs from CardEntry list
            var cardIds = cards.Select(card => card.cardId).ToList();
            
            // Use DeckRegistry to save the deck
            DeckRegistry.Instance.InsertDeck(deckName, cardIds);
            Debug.Log($"Deck '{deckName}' saved successfully with {cards.Count} cards");
        }

        public IEnumerable<DeckDefinition> GetDecks()
        {
            return DeckRegistry.Instance.GetAllDecks();
        }

        /// <summary>
        /// Saves card art using the ArtLibraryManager
        /// </summary>
        /// <param name="cardId">The ID of the card</param>
        /// <param name="cardArt">The texture to save</param>
        /// <returns>The file path where the art was saved</returns>
        public string SaveCardArt(int cardId, Texture2D cardArt)
        {
            return artLibraryManager.SaveCardArt(cardId, cardArt);
        }

        /// <summary>
        /// Loads card art using the ArtLibraryManager
        /// </summary>
        /// <param name="artPath">The file path to load the art from</param>
        /// <returns>The loaded sprite, or null if loading failed</returns>
        public Sprite LoadCardArt(string artPath)
        {
            return artLibraryManager.LoadCardArt(artPath);
        }

        /// <summary>
        /// Loads card art by card ID using the ArtLibraryManager
        /// </summary>
        /// <param name="cardId">The ID of the card</param>
        /// <returns>The loaded sprite, or null if loading failed</returns>
        public Sprite LoadCardArtById(int cardId)
        {
            return artLibraryManager.LoadCardArtById(cardId);
        }

        /// <summary>
        /// Gets a card entry by card ID
        /// </summary>
        /// <param name="cardId">The ID of the card</param>
        /// <returns>The card entry, or null if not found</returns>
        public CardEntry GetCardEntry(int cardId) {
            return new CardEntry {
                cardId = cardId,
                cardData = CardRegistry.Instance.GetCard(cardId),
                quantity = 1,
                dateAcquired = DateTime.Now,
                isFoil = false,
                tags = new List<string>(),
                stats = new Dictionary<string, int>()
            };
        }
    }
} 