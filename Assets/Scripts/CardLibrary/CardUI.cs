using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardHouse;
using UnityEngine.EventSystems;
using CardLibrary;
using System;

public class CardUI : MonoBehaviour, IPointerClickHandler
{
    [Header("Card Display")]
    [SerializeField] private Image cardImage;
    [SerializeField] private Image foilOverlay;
    [SerializeField] private TextMeshProUGUI cardNameText;
    [SerializeField] private TextMeshProUGUI powerText;
    [SerializeField] private TextMeshProUGUI costText;
    [SerializeField] private TextMeshProUGUI quantityText;
    [SerializeField] private GameObject selectedIndicator;

    private CardEntry cardEntry;
    private bool isSelected;
    private DeckBuilderManager deckBuilderManager;

    public static event Action<CardEntry, bool> OnCardClicked;

    private void Start()
    {
        deckBuilderManager = FindAnyObjectByType<DeckBuilderManager>();
    }

    public void Initialize(CardEntry cardEntry, bool isFoil)
    {
        if (cardEntry == null) return;

        // Store the card entry
        this.cardEntry = cardEntry;

        // Update UI elements
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (cardEntry == null || cardEntry.getCardDefinition() == null) return;

        // Update basic card information
        cardNameText.text = cardEntry.getCardDefinition().card_name;
        powerText.text = cardEntry.getCardDefinition().power.ToString();
        costText.text = cardEntry.getCardDefinition().cost.ToString();
        
        // Update quantity if available
        if (quantityText != null)
        {
            quantityText.text = cardEntry.quantity > 1 ? $"x{cardEntry.quantity}" : "";
        }

        // TODO: Load and set card image if you have card artwork
        if (cardEntry.getCardDefinition().Art != null)
        {
            cardImage.sprite = cardEntry.getCardDefinition().Art;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (cardEntry == null || cardEntry.getCardDefinition() == null) return;

        bool isRightClick = eventData.button == PointerEventData.InputButton.Right;
        
        // Trigger the card click event
        OnCardClicked?.Invoke(cardEntry, isRightClick);

        // Handle left click for deck building
        if (!isRightClick)
        {
            if (deckBuilderManager != null)
            {
                Debug.Log("CardUI: OnPointerClick");
                deckBuilderManager.OnCardLibraryCardClicked(cardEntry);
            }
            else
            {
                ToggleSelection();
            }
        }
    }

    public void ToggleSelection()
    {
        isSelected = !isSelected;
        selectedIndicator.SetActive(isSelected);
    }

    public void Deselect()
    {
        isSelected = false;
        selectedIndicator.SetActive(false);
    }

    public CardEntry GetCardEntry()
    {
        return cardEntry;
    }

    public bool IsSelected()
    {
        return isSelected;
    }
} 