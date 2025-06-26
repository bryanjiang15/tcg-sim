using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using CardLibrary;
using CardHouse;

public class DeckSelectionPanel : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private ScrollRect scrollView;
    [SerializeField] private RectTransform content;
    [SerializeField] private GameObject deckUIPrefab;
    [SerializeField] private Button confirmButton;

    private List<DeckUI> deckUIs = new List<DeckUI>();
    private DeckUI selectedDeck;

    public static event Action<DeckDefinition> OnDeckConfirmed;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        PopulateDecks();

        // Subscribe to deck selection events
        DeckUI.OnDeckSelected += HandleDeckSelected;
    }

    private void OnDestroy()
    {
        // Unsubscribe from deck selection events
        DeckUI.OnDeckSelected -= HandleDeckSelected;
    }

    public void AddDeck(DeckDefinition deckDefinition)
    {
        if (deckUIPrefab == null || content == null) return;

        GameObject deckObj = Instantiate(deckUIPrefab, content);
        DeckUI deckUI = deckObj.GetComponent<DeckUI>();
        
        if (deckUI != null)
        {
            deckUI.Initialize(deckDefinition);
            deckUIs.Add(deckUI);
        }
    }

    private void PopulateDecks()
    {
       var decks = CardLibraryManager.Instance.GetDecks();
       foreach (var deck in decks)
       {
        AddDeck(deck);
       }
    }

    private void HandleDeckSelected(DeckUI selectedDeckUI)
    {
        // Deselect previously selected deck
        if (selectedDeck != null && selectedDeck != selectedDeckUI)
        {
            selectedDeck.Deselect();
        }

        selectedDeck = selectedDeckUI;
    }

    public void OnConfirmButtonClicked()
    {
        if (selectedDeck != null)
        {
            OnDeckConfirmed?.Invoke(selectedDeck.GetDeckDefinition());
        }
        gameObject.SetActive(false);
    }

    public void ClearDecks()
    {
        foreach (DeckUI deckUI in deckUIs)
        {
            if (deckUI != null)
            {
                Destroy(deckUI.gameObject);
            }
        }
        deckUIs.Clear();
        selectedDeck = null;
    }
}
