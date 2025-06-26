using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using CardHouse;
using CardLibrary;
using TMPro;

public class DeckPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GridLayoutGroup deckGrid;
    [SerializeField] private GameObject cardUIPrefab;
    [SerializeField] private TextMeshProUGUI deckCountText;

    [Header("Deck Settings")]
    [SerializeField] private int maxDeckSize = 40;
    [SerializeField] private int minDeckSize = 40;

    private Dictionary<int, int> cardQuantities = new Dictionary<int, int>();

    private void Start()
    {
        UpdateDeckCount();
    }

    public bool AddCard(CardEntry card)
    {
        int cardId = card.cardId;
        if (cardQuantities.ContainsKey(cardId))
        {
            if (cardQuantities[cardId] >= 4) // Maximum 4 copies of a card
            {
                Debug.Log("Maximum copies of this card reached!");
                return false;
            }
            cardQuantities[cardId]++;
        }
        else
        {
            cardQuantities[cardId] = 1;
        }

        CreateCardUI(card);
        UpdateDeckCount();
        return true;
    }

    public void RemoveCard(CardEntry card)
    {
        int cardId = card.cardId;
        if (cardQuantities.ContainsKey(cardId))
        {
            cardQuantities[cardId]--;
            if (cardQuantities[cardId] <= 0)
            {
                cardQuantities.Remove(cardId);
            }
        }

        UpdateDeckCount();
        RefreshDeckUI();
    }

    private void CreateCardUI(CardEntry card)
    {
        if (cardUIPrefab == null || deckGrid == null) return;

        GameObject cardUI = Instantiate(cardUIPrefab, deckGrid.transform);
        CardUI cardUIComponent = cardUI.GetComponent<CardUI>();
        
        if (cardUIComponent != null)
        {
            cardUIComponent.Initialize(card, card.isFoil);
        }
    }

    private void RefreshDeckUI()
    {
        // Clear existing cards
        foreach (Transform child in deckGrid.transform)
        {
            Destroy(child.gameObject);
        }

        // Recreate UI for all cards
        foreach (var card in DeckBuilderManager.Instance.GetDeckCards())
        {
            CreateCardUI(card);
        }
    }

    private void UpdateDeckCount()
    {
        var deckCards = DeckBuilderManager.Instance.GetDeckCards();
        if (deckCountText != null)
        {
            deckCountText.text = $"Deck: {deckCards.Count}/{maxDeckSize}";
        }
    }

    public bool IsDeckValid()
    {
        var deckCards = DeckBuilderManager.Instance.GetDeckCards();
        return deckCards.Count >= minDeckSize && deckCards.Count <= maxDeckSize;
    }
}
