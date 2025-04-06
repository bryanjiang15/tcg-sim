using UnityEngine;

public class RevealCardGA : GameAction, ILocationCardsUpdated {
    public SnapCard card;
    public bool IsCardPlayed = true;
    public RevealCardGA(SnapCard card, bool IsCardPlayed = true) {
        this.card = card;
        this.IsCardPlayed = IsCardPlayed;
    }
}