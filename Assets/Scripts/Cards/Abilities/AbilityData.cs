public enum AbilityTriggerType
{
    OnReveal, //When card is revealed - from any place (hand,deck,created,played) to location
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
    // Card played only if it played from hand during preparation phase
    BeforeCardPlayed, //Triggered before OnrevealGA is performed
    AfterCardPlayed, //Triggered after OnrevealGA is performed
    AfterAbilityTriggered,
    OnCreated, //Triggered when the card is created
    None
}

public enum AbilityEffectType
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
    CopyCard,
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

public enum CalculationType
{
    Total,
    MaxValue,
    MinValue
}

public enum AbilityTargetType
{
    Deck,
    Hand,
    Self,
    PlayerDirectLocationCards,
    EnemyDirectLocationCards,
    AllDirectLocationCards,
    AllPlayerCards,
    AllEnemyCards,
    AllPlayedCards,
    PlayerDirectLocation,
    AllPlayerLocation,
    EnemyDirectLocation,
    AllEnemyLocation,
    DirectLocation,
    AllLocation,
    EnemyDeck,
    EnemyHand,
    NextPlayedCard,
    TriggeredActionTargets, // Targets that the action that triggered this ability affected
    TriggeredActionSource, // The source card of the action that triggered this ability
    CreatedCard, //Only activated when trigger is afterAbilied is a create card ability
    AllPlayerPlayedCards,
    AllEnemyPlayedCards,
    OwnedPlayer,
    EnemyPlayer,
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
    CurrentCost,
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