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

    void Start() {
        if (cardGroup != null) {
            cardGroup.OnCardMounted.AddListener((Card card) => {
                if (card is SnapCard snapCard) {
                    snapCard.CardRevealed.AddListener(UpdatePowerLabel);
                }
            });
            cardGroup.OnCardUnmounted.AddListener((Card card) => {
                if (card is SnapCard snapCard) {
                    snapCard.CardRevealed.RemoveListener(UpdatePowerLabel);
                }
            });
            UpdatePowerLabel();
        }
    }

    void OnDestroy() {
        if (cardGroup != null) {
            cardGroup.OnGroupChanged.RemoveListener(UpdatePowerLabel);
        }
    }

    void UpdatePowerListener() {
        
        foreach (var card in cardGroup.MountedCards) {
            
        }
    }

    void UpdatePowerLabel() {
        int totalPower = 0;
        foreach (var card in cardGroup.MountedCards) {
            if (card is SnapCard && ((SnapCard)card).revealed) {
                SnapCard snapCard = (SnapCard)card;
                totalPower += snapCard.stats.power;
            }
        }
        powerLabel.text = totalPower.ToString();
    }

    public void readyLocation() {
        foreach (var card in cardGroup.MountedCards) {
            if (card is SnapCard) {
                SnapCard snapCard = (SnapCard)card;
                if (!snapCard.revealed) {
                    snapCard.SetFacing(CardFacing.FaceDown);
                }
            }
        }
    }
}