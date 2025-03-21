using UnityEngine;
using UnityEngine.UI;
using CardHouse;
using TMPro;
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
    public SnapCard cardRepresentation;

    void Start() {
        if (cardGroup != null) {
            cardGroup.OnCardMounted.AddListener(subscribePowerListener);
            cardGroup.OnCardUnmounted.AddListener(unsubscribePowerListener);
            UpdatePowerLabel();
        }

        cardRepresentation.initCardStats(new SnapCardStats(0, 0, "Location", 0));
    }

    void subscribePowerListener(Card card) 
    {
        if (card is SnapCard snapCard) {
            snapCard.SetPlayedLocation(this);
            if (!SnapPhaseManager.Instance.GetCurrentPhaseType().Equals(SnapPhaseType.Preparation)) 
                UpdatePowerLabel();
            else
                snapCard.CardRevealed.AddListener(UpdatePowerLabel);
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

    void UpdatePowerLabel() {
        int totalPower = 0;
        foreach (var card in cardGroup.MountedCards) {
            if (card is SnapCard && ((SnapCard)card).Revealed) {
                SnapCard snapCard = (SnapCard)card;
                totalPower += snapCard.stats.power;
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