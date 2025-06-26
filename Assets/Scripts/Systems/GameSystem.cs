using System.Collections.Generic;
using CardHouse;
using UnityEngine;

public class GameSystem : Singleton<GameSystem>
{

    [SerializeField] private GroupSetup groupSetup;
    [SerializeField] private List<DeckSetup> decksToSetup;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        DeckSelectionPanel.OnDeckConfirmed += OnDeckConfirmed;
    }

    private void OnDeckConfirmed(DeckDefinition deckDefinition) {
        Debug.Log("Deck confirmed: " + deckDefinition.name);
        groupSetup.DoSetup();
        foreach (DeckSetup deckSetup in decksToSetup) {
            deckSetup.DeckDefinition = deckDefinition;
            deckSetup.DoSetup();
        }
    }
}
