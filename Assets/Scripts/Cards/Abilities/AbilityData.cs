public enum AbilityTrigger
{
    OnReveal,
    Ongoing,
    GameStart,
    EndTurn,
    EndGame,
    InHand,
    InDeck,
    Destroyed,
    Discarded,
    Moved,
    Banished,
    StartTurn,
    Activate,
    BeforeCardPlayed, //Triggered before OnrevealGA is performed
    AfterCardPlayed, //Triggered after OnrevealGA is performed
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
    AddTemporaryAbility,
}

public enum AbilityAmountType
{
    Constant,
    ForEachTarget,
    Cardid,
    Json,
    TargetValue,
    Boolean,
}

public enum AbilityAmountValueType
{
    Total,
    Count,
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
    LocationFull, // New: Check if a location is occupied
}

public enum AbilityRequirementComparator
{
    None,
    Equal,
    Greater,
    Less,
    GEQ,
    LEQ,
    IsHighestInLocation,
    IsMinInGroup,
    NotEqual, // New: Check for inequality
    Contains, // New: Check if a collection contains a value
    DoesNotContain, // New: Check if a collection does not contain a value
}

public enum AbilityRequirementCondition
{
    All,
    Any
}