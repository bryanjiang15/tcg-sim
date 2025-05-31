using System;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Linq;

public class CardLibraryManager : MonoBehaviour
{
    public static CardLibraryManager Instance { get; private set; }

    [SerializeField] private GameObject cardLibraryGrid;
    [SerializeField] private GameObject cardUIPrefab;

    private CardGenerator cardGenerator;

    [Serializable]
    public class CardLibraryData
    {
        public List<CardEntry> cards = new List<CardEntry>();
    }

    [Serializable]
    public class CardEntry
    {
        public string cardId;
        public string cardName => cardDefinition.card_name;
        public SnapCardDefinition cardDefinition; // Store the information about the card
        public int quantity;
        public DateTime dateAcquired;
        public bool isFoil; // For special/foil versions of cards
        public List<string> tags; // For custom organization
        public Dictionary<string, int> stats; // For tracking card usage stats
    }

    private CardLibraryData libraryData;
    private string savePath;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeLibrary();
            cardGenerator = new CardGenerator();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeLibrary()
    {
        savePath = Path.Combine(Application.persistentDataPath, "cardLibrary.json");
        LoadLibrary();
    }

    public void AddCard(string cardId, SnapCardDefinition cardDefinition, bool isFoil = false)
    {
        var existingCard = libraryData.cards.Find(c => c.cardId == cardId);
        if (existingCard == null)
        {
            libraryData.cards.Add(new CardEntry
            {
                cardId = cardId,
                cardDefinition = cardDefinition,
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

    public void RemoveCard(string cardId)
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

    public CardEntry GetCardInfo(string cardId)
    {
        return libraryData.cards.Find(c => c.cardId == cardId);
    }

    public List<CardEntry> GetAllCards()
    {
        return libraryData.cards;
    }

    public void AddTagToCard(string cardId, string tag)
    {
        var card = libraryData.cards.Find(c => c.cardId == cardId);
        if (card != null && !card.tags.Contains(tag))
        {
            card.tags.Add(tag);
            SaveLibrary();
        }
    }

    public void RemoveTagFromCard(string cardId, string tag)
    {
        var card = libraryData.cards.Find(c => c.cardId == cardId);
        if (card != null)
        {
            card.tags.Remove(tag);
            SaveLibrary();
        }
    }

    public void UpdateCardStats(string cardId, string statName, int value)
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
        string json = JsonUtility.ToJson(libraryData, true);
        Debug.Log(json);
        File.WriteAllText(savePath, json);
    }

    private void LoadLibrary()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            libraryData = JsonUtility.FromJson<CardLibraryData>(json);
            Debug.Log("Loading card library:");
            foreach (var cardEntry in libraryData.cards)
            {
                Debug.Log($"Card ID: {cardEntry.cardId}");
                Debug.Log($"  Name: {cardEntry.cardDefinition.card_name}");
                Debug.Log($"  Quantity: {cardEntry.quantity}");
                Debug.Log($"  Tags: {string.Join(", ", cardEntry.tags)}");
                Debug.Log($"  Is Foil: {cardEntry.isFoil}");
                if (cardEntry.stats.Count > 0)
                {
                    Debug.Log("  Stats:");
                    foreach (var stat in cardEntry.stats)
                    {
                        Debug.Log($"    {stat.Key}: {stat.Value}");
                    }
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

    // Additional utility methods
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

    public Dictionary<string, int> GetCardStats(string cardId)
    {
        var card = libraryData.cards.Find(c => c.cardId == cardId);
        return card != null ? card.stats : new Dictionary<string, int>();
    }

    public CardGenerator GetCardGenerator()
    {
        return cardGenerator;
    }
} 