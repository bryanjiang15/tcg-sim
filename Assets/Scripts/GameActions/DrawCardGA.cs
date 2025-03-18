using UnityEngine;

public class DrawCardGA : GameAction {
    public int numberOfCards;
    public Player player;
    public bool initialDraw;

    public DrawCardGA(int numberOfCards, Player player, bool initialDraw = false) {
        this.player = player;
        this.numberOfCards = numberOfCards;
        this.initialDraw = initialDraw;
    }
}