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
                AddPlayedCard(snapCard);
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
        int totalPower = 0;
        foreach (var card in cardGroup.MountedCards) {
            if (card is SnapCard && ((SnapCard)card).Revealed) {
                SnapCard snapCard = (SnapCard)card;
                totalPower += snapCard.GetPower();
            }
        }
        totalPower += cardRepresentation.GetPower();
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
}