using UnityEngine;

public class RevealCardGA : GameAction, ILocationCardsUpdated {
    public SnapCard card;
    public RevealCardGA(SnapCard card) {
        this.card = card;
    }
}