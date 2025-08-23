using System.Collections.Generic;
using CardHouse;

public class Deck : CardGroup, ICardContainer
{
    public List<SnapCard> cards;
    public Player ownedPlayer;
    Player ITargetable.ownedPlayer => ownedPlayer;

    public AbilityAmount GetTargetValue(AbilityRequirementType reqType, AbilityAmount reqAmount = null) {
        switch (reqType)
        {
            case AbilityRequirementType.NumberOfCards:
                return new AbilityAmount { amountType = AbilityAmountType.Constant, value = cards.Count.ToString() };
            default:
                return new AbilityAmount { amountType = AbilityAmountType.Constant, value = "0" };
        }
    }
}
