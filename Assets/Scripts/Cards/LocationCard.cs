using UnityEngine;

public class LocationCard : SnapCard {
    // A card object that represents the location. Holds location buffs
    Location location;

    public bool IsFull() {
        return location.isFull();
    }

    public bool IsEmpty() {
        return location.cardGroup.MountedCards.Count == 0;
    }

    public void SetLocation(Location location) {
        this.location = location;
    }

    // public int GetLocationPower() {
    //     return location;
    // }
}