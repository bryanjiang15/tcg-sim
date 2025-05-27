using System.Collections.Generic;
using CardHouse;

public class Hand : CardGroup, ICardContainer
{
    public List<SnapCard> cards;
    public Player ownedPlayer;
    Player ITargetable.ownedPlayer => ownedPlayer;

    public AbilityAmount GetTargetValue(AbilityRequirementType reqType) {
        switch (reqType)
        {
            case AbilityRequirementType.NumberOfCards:
                return new AbilityAmount { type = AbilityAmountType.Constant, value = cards.Count.ToString() };
            default:
                return new AbilityAmount { type = AbilityAmountType.Constant, value = "0" };
        }
    }
}
