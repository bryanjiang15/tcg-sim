using UnityEngine;
using UnityEngine.UI;
using CardHouse;
using TMPro;
using System.Collections.Generic;
using UnityEngine.Events;
public enum LocationPosition {
    Left,
    Center,
    Right
}

public class Location : MonoBehaviour {

    public Player player;
    public LocationPosition position;
    public CardGroup cardGroup;
    public TextMeshPro powerLabel;
    public LocationCard cardRepresentation;
    public List<SnapCard> playedCards;
    public UnityEvent OnPlayedCardChanged = new UnityEvent();
    public int maxCards = 4;
    public int freeSlots => maxCards - cardGroup.MountedCards.Count;
    public int totalPower {get; private set;} // Total power of all cards in this location, including the location card itself

    void Start() {
        cardRepresentation.initCardStats(new SnapCardStats(0, 0, "Location", 0));
        cardRepresentation.SetLocation(this);
        if (cardGroup != null) {
            cardGroup.OnCardMounted.AddListener(subscribePowerListener);
            cardGroup.OnCardUnmounted.AddListener(unsubscribePowerListener);
            UpdatePowerLabel();
        }
    }

    void subscribePowerListener(Card card) 
    {
        if (card is SnapCard snapCard) {
            snapCard.SetPlayedLocation(this);
            if (!SnapPhaseManager.Instance.GetCurrentPhaseType().Equals(SnapPhaseType.Preparation)) {
                Debug.Log("Adding card to played cards: " + snapCard.stats.card_name);
                AddPlayedCard(snapCard);
                UpdatePowerLabel();
            }
            else{
                snapCard.CardRevealed.AddListener(UpdatePowerLabel);
                snapCard.CardRevealed.AddListener(() => {
                    AddPlayedCard(snapCard);
                });
            }
            Power power = snapCard.GetComponent<Power>();
            if (power != null) {
                power.PowerChanged.AddListener(UpdatePowerLabel);
            }
        }
    }

    void unsubscribePowerListener(Card card) {
        if (card is SnapCard snapCard) {
            snapCard.SetPlayedLocation(null);
            if (!SnapPhaseManager.Instance.GetCurrentPhaseType().Equals(SnapPhaseType.Preparation)) 
                UpdatePowerLabel();
            else
                snapCard.CardRevealed.RemoveListener(UpdatePowerLabel);
            Power power = snapCard.GetComponent<Power>();
            if (power != null) {
                power.PowerChanged.RemoveListener(UpdatePowerLabel);
            }
        }
    }

    void OnEnable() {
        if (cardGroup != null) {
            cardGroup.OnGroupChanged.AddListener(UpdatePowerLabel);
        }
    }

    void OnDestroy() {
        if (cardGroup != null) {
            cardGroup.OnGroupChanged.RemoveListener(UpdatePowerLabel);
        }
    }

    void AddPlayedCard(SnapCard card) {
        playedCards.Add(card);
        OnPlayedCardChanged.Invoke();
    }

    void UpdatePowerLabel() {
        totalPower = 0;
        foreach (var card in playedCards) {
            if (card is SnapCard snapCard) {
                totalPower += snapCard.GetPower();
            }
        }
        totalPower += cardRepresentation.GetPowerBuffs();
        powerLabel.text = totalPower.ToString();
    }

    public void readyLocation() {
        foreach (var card in cardGroup.MountedCards) {
            if (card is SnapCard) {
                SnapCard snapCard = (SnapCard)card;
                if (!snapCard.Revealed) {
                    snapCard.SetFacing(CardFacing.FaceDown);
                }
            }
        }
    }

    public bool isFull() {
        return playedCards.Count >= maxCards;
    }
}