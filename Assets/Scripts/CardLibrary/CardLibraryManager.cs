using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;
using CardHouse;
using Newtonsoft.Json;

namespace CardLibrary
{
    [Serializable]
    public class DeckLibrary
    {
        public List<DeckDefinition> decks = new List<DeckDefinition>();
    }

    public class CardLibraryManager : Singleton<CardLibraryManager>
    {

        private CardGenerator cardGenerator;
        private CardLibraryData libraryData;
        private string savePath;
        private DeckLibrary deckLibrary;
        private string deckSavePath;

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
            savePath = Path.Combine(Application.persistentDataPath, "cardLibrary.json");
            deckSavePath = Path.Combine(Application.persistentDataPath, "deckLibrary.json");
            LoadLibrary();
            LoadDeckLibrary();
            
            // Preload all card art for better performance
            artLibraryManager.PreloadAllCardArt();
            Debug.Log($"Preloaded {artLibraryManager.GetCachedSpriteCount()} card art sprites");
        }

        public void AddCard(int cardId, SnapCardDefinition cardDefinition, bool isFoil = false)
        {
            var existingCard = libraryData.cards.Find(c => c.cardId == cardId);
            if (existingCard == null)
            {
                // Create serializable data from the card definition
                var cardData = ObjectMapper.GetSnapCardData(cardDefinition);

                libraryData.cards.Add(new CardEntry
                {
                    cardId = cardId,
                    cardData = cardData,
                    quantity = 1,
                    dateAcquired = DateTime.Now,
                    isFoil = isFoil,
                    tags = new List<string>(),
                    stats = new Dictionary<string, int>()
                });
            }
            else
            {
                existingCard.quantity++;
            }
            SaveLibrary();
        }

        public void RemoveCard(int cardId)
        {
            var card = libraryData.cards.Find(c => c.cardId == cardId);
            if (card != null)
            {
                if (card.quantity > 1)
                {
                    card.quantity--;
                }
                else
                {
                    libraryData.cards.Remove(card);
                }
                SaveLibrary();
            }
        }

        public CardEntry GetCard(int cardId)
        {
            return libraryData.cards.Find(c => c.cardId == cardId);
        }

        public List<CardEntry> GetAllCards()
        {
            return libraryData.cards;
        }

        public void AddTagToCard(int cardId, string tag)
        {
            var card = libraryData.cards.Find(c => c.cardId == cardId);
            if (card != null && !card.tags.Contains(tag))
            {
                card.tags.Add(tag);
                SaveLibrary();
            }
        }

        public void RemoveTagFromCard(int cardId, string tag)
        {
            var card = libraryData.cards.Find(c => c.cardId == cardId);
            if (card != null)
            {
                card.tags.Remove(tag);
                SaveLibrary();
            }
        }

        public void UpdateCardStats(int cardId, string statName, int value)
        {
            var card = libraryData.cards.Find(c => c.cardId == cardId);
            if (card != null)
            {
                if (!card.stats.ContainsKey(statName))
                {
                    card.stats[statName] = 0;
                }
                card.stats[statName] += value;
                SaveLibrary();
            }
        }

        private void SaveLibrary()
        {
            Debug.Log("Saving card library:");
            Debug.Log(libraryData.cards.Count);
            string json = JsonConvert.SerializeObject(libraryData, Formatting.Indented);
            Debug.Log(json);
            File.WriteAllText(savePath, json);
        }

        private void LoadLibrary()
        {
            if (File.Exists(savePath))
            {
                string json = File.ReadAllText(savePath);
                libraryData = JsonConvert.DeserializeObject<CardLibraryData>(json);
                Debug.Log("Loading card library:");
                foreach (var cardEntry in libraryData.cards)
                {
                    Debug.Log($"Card ID: {cardEntry.cardId}");
                    if (cardEntry.cardData.abilities != null && cardEntry.cardData.abilities.Count > 0)
                    {
                        Debug.Log($"Abilities: {cardEntry.cardData.abilities[0]}");
                    }
                    Debug.Log("-------------------");
                }
            }
            else
            {
                Debug.Log("No card library found. Creating new one.");
                libraryData = new CardLibraryData();
                SaveLibrary();
            }
        }

        public List<CardEntry> GetCardsByTag(string tag)
        {
            return libraryData.cards.Where(card => card.tags.Contains(tag)).ToList();
        }

        public List<CardEntry> GetFoilCards()
        {
            return libraryData.cards.Where(card => card.isFoil).ToList();
        }

        public int GetTotalCardCount()
        {
            return libraryData.cards.Sum(card => card.quantity);
        }

        public Dictionary<string, int> GetCardStats(int cardId)
        {
            var card = libraryData.cards.Find(c => c.cardId == cardId);
            return card != null ? card.stats : new Dictionary<string, int>();
        }

        public CardGenerator GetCardGenerator()
        {
            return cardGenerator;
        }

        public void ClearLibrary()
        {
            // Delete the saved file if it exists
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
            }

            // Reset the library data
            libraryData = new CardLibraryData();
            SaveLibrary();
            
            Debug.Log("Card library has been cleared.");
        }

        private void LoadDeckLibrary()
        {
            if (File.Exists(deckSavePath))
            {
                string json = File.ReadAllText(deckSavePath);
                var snapDeckLibraryData = JsonConvert.DeserializeObject<SnapDeckLibraryData>(json);
                deckLibrary = ObjectMapper.GetDeckLibraryData(snapDeckLibraryData);
            }
            else
            {
                deckLibrary = new DeckLibrary();
                SaveDeckLibrary();
            }
        }

        private void SaveDeckLibrary()
        {
            SnapDeckLibraryData snapDeckLibraryData = ObjectMapper.GetSnapDeckLibraryData(deckLibrary);
            string json = JsonConvert.SerializeObject(snapDeckLibraryData, Formatting.Indented);
            File.WriteAllText(deckSavePath, json);
        }

        public void SaveDeck(string deckName, List<CardEntry> cards)
        {
            if (string.IsNullOrEmpty(deckName) || cards == null || cards.Count == 0)
            {
                Debug.LogError("Invalid deck data provided for saving");
                return;
            }

            // Create a new DeckDefinition ScriptableObject
            var deckDefinition = ScriptableObject.CreateInstance<DeckDefinition>();
            deckDefinition.name = deckName;
            deckDefinition.CardCollection = cards.Select(card => ObjectMapper.GetSnapCardDefinition(card.cardData)).ToList();
            deckLibrary.decks.Add(deckDefinition);
            SaveDeckLibrary();
            Debug.Log($"Deck '{deckName}' saved successfully with {cards.Count} cards");
        }

        public List<DeckDefinition> GetDecks()
        {
            return deckLibrary.decks;
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
            return libraryData.cards.Find(c => c.cardId == cardId);
        }
    }
} 