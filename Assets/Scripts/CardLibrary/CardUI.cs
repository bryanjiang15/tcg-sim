using UnityEngine;
using UnityEngine.UI;
using TMPro;
using CardHouse;
using UnityEngine.EventSystems;

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

    private CardLibraryManager.CardEntry cardEntry;
    private bool isSelected;

    public void Initialize(SnapCardDefinition cardDefinition, bool isFoil)
    {
        if (cardDefinition == null) return;

        // Store the card entry
        cardEntry = new CardLibraryManager.CardEntry
        {
            cardDefinition = cardDefinition,
            isFoil = isFoil
        };

        // Update UI elements
        UpdateUI();
    }

    private void UpdateUI()
    {
        if (cardEntry == null || cardEntry.cardDefinition == null) return;

        // Update basic card information
        cardNameText.text = cardEntry.cardDefinition.card_name;
        powerText.text = cardEntry.cardDefinition.power.ToString();
        costText.text = cardEntry.cardDefinition.cost.ToString();
        
        // Update foil effect
        foilOverlay.gameObject.SetActive(cardEntry.isFoil);
        
        // Update quantity if available
        if (quantityText != null)
        {
            quantityText.text = cardEntry.quantity > 1 ? $"x{cardEntry.quantity}" : "";
        }

        // TODO: Load and set card image if you have card artwork
        // if (cardImage != null)
        // {
        //     cardImage.sprite = cardEntry.cardDefinition.cardArtwork;
        // }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        ToggleSelection();
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

    public CardLibraryManager.CardEntry GetCardEntry()
    {
        return cardEntry;
    }

    public bool IsSelected()
    {
        return isSelected;
    }
} 