using UnityEngine;
using TMPro;
using CardHouse;
using CardLibrary;
using UnityEngine.EventSystems;

public class CardDisplayPanel : MonoBehaviour, IDeselectHandler
{
    [SerializeField] private TextMeshProUGUI abilityTriggerText;
    [SerializeField] private TextMeshProUGUI abilityTargetText;
    [SerializeField] private TextMeshProUGUI abilityEffectText;
    [SerializeField] private TextMeshProUGUI abilityAmountText;

    private void Start()
    {
        // Hide the panel initially
        gameObject.SetActive(false);

        // Subscribe to the card click event
        CardUI.OnCardClicked += HandleCardClick;
    }

    private void OnDestroy()
    {
        // Unsubscribe from the event when destroyed
        CardUI.OnCardClicked -= HandleCardClick;
    }

    private void HandleCardClick(CardEntry cardEntry, bool isRightClick)
    {
        if (!isRightClick) return; // Only show panel on right click

        DisplayCard(cardEntry.getCardDefinition());
    }

    public void DisplayCard(SnapCardDefinition cardDefinition)
    {
        if (cardDefinition == null) return;

        Debug.Log("Displaying card: " + cardDefinition.card_name);

        // Clear previous ability text
        abilityTriggerText.text = "";
        abilityTargetText.text = "";
        abilityEffectText.text = "";
        abilityAmountText.text = "";

        // Display the first ability if any
        if (cardDefinition.abilities != null && cardDefinition.abilities.Count > 0)
        {
            var ability = cardDefinition.abilities[0];
            abilityTriggerText.text = ability.triggerDefinition.triggerType.ToString();
            abilityTargetText.text = ability.targetDefinition[0].targetType.ToString();
            abilityEffectText.text = ability.effect.ToString();
            abilityAmountText.text = ability.amount.ToString();
        }

        // Show the panel and make it selectable
        gameObject.SetActive(true);
        EventSystem.current.SetSelectedGameObject(gameObject);
    }

    public void OnDeselect(BaseEventData eventData)
    {
        gameObject.SetActive(false);
    }
}