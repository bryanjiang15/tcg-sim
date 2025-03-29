using UnityEngine;

public class LocationCard : SnapCard {
    Location location;

    public bool IsFull() {
        return !location.cardGroup.HasRoom() && AreAllCardsRevealed();
    }

    bool AreAllCardsRevealed() {
        foreach (var card in location.playedCards) {
            if (!card.Revealed) {
                return false;
            }
        }
        return true;
    }

    public bool IsEmpty() {
        return location.cardGroup.MountedCards.Count == 0;
    }

    public void SetLocation(Location location) {
        this.location = location;
    }
}