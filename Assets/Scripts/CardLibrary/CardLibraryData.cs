using System;
using System.Collections.Generic;
using CardHouse;
using UnityEngine;

namespace CardLibrary
{
    [Serializable]
    public class CardLibraryData
    {
        public List<CardEntry> cards = new List<CardEntry>();
    }

    [Serializable]
    public class CardEntry
    {
        public int cardId;
        public string cardName => cardData.card_name;
        public SnapCardData cardData;
        public int quantity;
        public DateTime dateAcquired;
        public bool isFoil;
        public List<string> tags;
        public Dictionary<string, int> stats;

        public SnapCardDefinition getCardDefinition()
        {
            return cardData.getCardDefinition();
        }
    }
} 