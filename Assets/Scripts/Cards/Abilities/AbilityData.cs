public enum AbilityTrigger
{
    OnReveal,
    Ongoing,
    GameStart,
    EndTurn,
    EndGame,
    InHand,
    Destroyed,
    Discarded,
    Moved,
    Banished,
    StartTurn,
    Activate,
    CardPlayed,
    AfterAbilityTriggered,
    None
}

public enum AbilityEffect
{
    GainPower,
    LosePower, //Self loses power
    StealPower,
    Afflict, //Decrease power of target
    Draw,
    Discard,
    Destroy,
    Move,
    GainEnergy,
    GainMaxEnergy,
    LoseEnergy,
    AddPowerToLocation,
    CreateCardInHand,
    CreateCardInDeck,
    CreateCardInLocation,
    RemoveAbility,
    ReduceCost,
    IncreaseCost,
    Merge,
    Return,
    AddCardToLocation,
    AddCardToHand,
    SetPower,
    SetCost,
    CopyAndActivate,
    AddKeyword,
}

public enum AbilityAmountType
{
    Constant,
    ConstForEveryReqMet,
    Cardid,
    Json
}

public enum AbilityTarget {
    Deck,
    Hand,
    Self,
    PlayerDirectLocationCards,
    EnemyDirectLocationCards,
    AllDirectLocationCards,
    AllPlayerCards,
    AllEnemyCards,
    AllBoardCards,
    PlayerDirectLocation,
    AllPlayerLocation,
    EnemyDirectLocation,
    AllEnemyLocation,
    DirectLocation,
    AllLocation,
    EnemyDeck,
    EnemyHand,
    NextPlayedCard,
}

public enum AbilityTargetRange{
    None,
    First,
    Last,
    All,
    Random,
    AllRequirementsMet,
    RandomRequirementsMet,
}

public enum AbilityTargetSort
{
    None,
    Power,
    BaseCost,
    CurrentCost,
    PlayedOrder,
    LocationOrder,

}

public enum AbilityRequirementType
{
    None,
    Power,
    Cost,
    NumberOfCards,
    CurrentTurn,
    CurrentMaxEnergy,
    LocationPowerDifference,
    HasKeyword,
    IsCreated,
    CardName, // New: Check for specific card names
    BuffPresent, // New: Check if a specific buff is present
    LocationOccupied, // New: Check if a location is occupied
}

public enum AbilityRequirementComparator
{
    None,
    Equal,
    GreaterThan,
    LessThan,
    GEQ,
    LEQ,
    IsHighestInLocation,
    IsMinInGroup,
    NotEqual, // New: Check for inequality
    Contains, // New: Check if a collection contains a value
    DoesNotContain, // New: Check if a collection does not contain a value
}
