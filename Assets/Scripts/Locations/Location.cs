using UnityEngine;
using UnityEngine.UI;
using CardHouse;
using TMPro;

public class Location : MonoBehaviour {
    public CardGroup cardGroup;
    public TextMeshPro powerLabel;

    void Start() {
        if (cardGroup != null) {
            cardGroup.OnGroupChanged.AddListener(UpdatePowerLabel);
            UpdatePowerLabel();
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
            if (typeof(SnapCard).IsAssignableFrom(card.GetType())) {
                SnapCard snapCard = (SnapCard)card;
                Debug.Log("Card power: " + snapCard.stats.power);
                totalPower += snapCard.stats.power;
            }
        }
        Debug.Log("Total power: " + totalPower);
        powerLabel.text = totalPower.ToString();
    }
}