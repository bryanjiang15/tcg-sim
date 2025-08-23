using System.Collections.Generic;
using System.Linq;
using CardHouse;
using CardLibrary;
using UnityEngine;

public class CardPileSelectionPanel : MonoBehaviour
{
    public CardGroup selectedPile;
    public List<SnapCard> cards;
    [SerializeField]public  GameObject cardUIPrefab;

    public void StartSelection(CardGroup pile) {
        selectedPile = pile;
        gameObject.SetActive(true);

        cards = selectedPile.MountedCards.Select(card => card.GetComponent<SnapCard>()).ToList();

        List<CardUI> cardUIs = new List<CardUI>();
        foreach (var card in cards) {
            CardUI cardUI = Instantiate(cardUIPrefab, transform).GetComponent<CardUI>();
            CardEntry cardEntry = CardLibraryManager.Instance.GetCardEntry(card.stats.card_id);
            cardUI.Initialize(cardEntry, false);
            cardUIs.Add(cardUI);
        }
    }
}
