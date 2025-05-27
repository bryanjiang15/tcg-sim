public interface ITargetable
{
    Player ownedPlayer { get; }
    AbilityAmount GetTargetValue(AbilityRequirementType reqType);
}

public interface IDestructible : ITargetable
{
    public void DestroyCard(SnapCard source);
    public bool canBeDestroyed();
}

public interface IDiscardable : ITargetable
{
    public void DiscardCard(SnapCard source);
    public bool canBeDiscarded();
}

public interface IMoveable : ITargetable
{
    public void MoveCard(SnapCard source, LocationPosition locationPosition);
    public bool canBeMoved();
}

public interface ICardContainer : ITargetable
{
    
}


