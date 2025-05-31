using UnityEngine;

public class LocationCard : SnapCard {
    // A card object that represents the location. Holds location buffs
    public Location location { get; private set; }

    public override Player ownedPlayer => location.player;
    public int maxCards => location.maxCards;

    public bool IsFull() {
        return location.isFull();
    }

    public bool IsEmpty() {
        return location.cardGroup.MountedCards.Count == 0;
    }

    public void SetLocation(Location location) {
        this.location = location;
    }

    public override int GetPower() {
        int power = location.totalPower;
        foreach (var buff in buffs) {
            // Apply buffs to the power
            if (buff.type == BuffType.AdditionalPower) {
                StatBuff statBuff = buff as StatBuff;
                power += statBuff.amount;
            }
        }
        return power;
    }

    public int GetPowerBuffs() {
        int power = 0;
        foreach (var buff in buffs) {
            // Apply buffs to the power
            if (buff.type == BuffType.AdditionalPower) {
                StatBuff statBuff = buff as StatBuff;
                power += statBuff.amount;
            }
        }
        return power;
    }

    public override AbilityAmount GetTargetValue(AbilityRequirementType reqType) {
        switch (reqType)
        {
            case AbilityRequirementType.Power:
                return new AbilityAmount { amountType = AbilityAmountType.Constant, value = GetPower().ToString() };
            case AbilityRequirementType.LocationFull:
                return new AbilityAmount { amountType = AbilityAmountType.Boolean, value = IsFull().ToString() };
            default:
                return base.GetTargetValue(reqType);
        }
    }
}