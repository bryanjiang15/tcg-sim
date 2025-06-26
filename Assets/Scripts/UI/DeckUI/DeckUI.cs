using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using System;
using CardHouse;

public class DeckUI : MonoBehaviour, IPointerClickHandler
{
    [Header("UI References")]
    [SerializeField] private Image deckImage;
    [SerializeField] private TextMeshProUGUI deckNameText;
    [SerializeField] private GameObject selectedIndicator;

    private bool isSelected;
    private DeckDefinition deckDefinition;
    public static event Action<DeckUI> OnDeckSelected;

    public void Initialize(DeckDefinition deckDefinition)
    {
        this.deckDefinition = deckDefinition;
        deckNameText.text = deckDefinition.name;
        selectedIndicator.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            Select();
        }
    }

    public void Select()
    {
        isSelected = true;
        selectedIndicator.SetActive(true);
        OnDeckSelected?.Invoke(this);
    }

    public void Deselect()
    {
        isSelected = false;
        selectedIndicator.SetActive(false);
    }

    public bool IsSelected()
    {
        return isSelected;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    public DeckDefinition GetDeckDefinition()
    {
        return deckDefinition;
    }


}
