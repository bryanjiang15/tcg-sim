using UnityEngine;

public class DrawCardGA : GameAction, IHandUpdated, IDeckUpdated {
    public int numberOfCards;
    public Player player;
    public bool initialDraw;
    public SnapCard source;

    public DrawCardGA(int numberOfCards, Player player, bool initialDraw = false, SnapCard source = null) {
        this.player = player;
        this.numberOfCards = numberOfCards;
        this.initialDraw = initialDraw;
        this.source = source;
    }
}