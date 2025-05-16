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

    [Serializable]
    public class CardLibraryData
    {
        public Dictionary<string, CardEntry> cards = new Dictionary<string, CardEntry>();
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
        if (!libraryData.cards.ContainsKey(cardId))
        {
            libraryData.cards[cardId] = new CardEntry
            {
                cardId = cardId,
                cardDefinition = cardDefinition,
                quantity = 1,
                dateAcquired = DateTime.Now,
                isFoil = isFoil,
                tags = new List<string>(),
                stats = new Dictionary<string, int>()
            };
        }
        else
        {
            libraryData.cards[cardId].quantity++;
        }
        SaveLibrary();
    }

    public void RemoveCard(string cardId)
    {
        if (libraryData.cards.ContainsKey(cardId))
        {
            if (libraryData.cards[cardId].quantity > 1)
            {
                libraryData.cards[cardId].quantity--;
            }
            else
            {
                libraryData.cards.Remove(cardId);
            }
            SaveLibrary();
        }
    }

    public CardEntry GetCardInfo(string cardId)
    {
        return libraryData.cards.ContainsKey(cardId) ? libraryData.cards[cardId] : null;
    }

    public List<CardEntry> GetAllCards()
    {
        return libraryData.cards.Values.ToList();
    }

    public void AddTagToCard(string cardId, string tag)
    {
        if (libraryData.cards.ContainsKey(cardId) && !libraryData.cards[cardId].tags.Contains(tag))
        {
            libraryData.cards[cardId].tags.Add(tag);
            SaveLibrary();
        }
    }

    public void RemoveTagFromCard(string cardId, string tag)
    {
        if (libraryData.cards.ContainsKey(cardId))
        {
            libraryData.cards[cardId].tags.Remove(tag);
            SaveLibrary();
        }
    }

    public void UpdateCardStats(string cardId, string statName, int value)
    {
        if (libraryData.cards.ContainsKey(cardId))
        {
            if (!libraryData.cards[cardId].stats.ContainsKey(statName))
            {
                libraryData.cards[cardId].stats[statName] = 0;
            }
            libraryData.cards[cardId].stats[statName] += value;
            SaveLibrary();
        }
    }

    private void SaveLibrary()
    {
        string json = JsonUtility.ToJson(libraryData, true);
        File.WriteAllText(savePath, json);
    }

    private void LoadLibrary()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            libraryData = JsonUtility.FromJson<CardLibraryData>(json);
        }
        else
        {
            libraryData = new CardLibraryData();
            SaveLibrary();
        }
    }

    // Additional utility methods
    public List<CardEntry> GetCardsByTag(string tag)
    {
        return libraryData.cards.Values.Where(card => card.tags.Contains(tag)).ToList();
    }

    public List<CardEntry> GetFoilCards()
    {
        return libraryData.cards.Values.Where(card => card.isFoil).ToList();
    }

    public int GetTotalCardCount()
    {
        return libraryData.cards.Values.Sum(card => card.quantity);
    }

    public Dictionary<string, int> GetCardStats(string cardId)
    {
        return libraryData.cards.ContainsKey(cardId) ? libraryData.cards[cardId].stats : new Dictionary<string, int>();
    }
} 