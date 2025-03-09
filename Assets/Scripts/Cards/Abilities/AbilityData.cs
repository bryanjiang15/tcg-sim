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
    Cost,
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
    Created,
}

public enum AbilityRequirementComparator
{
    None,
    Equal,
    GreaterThan,
    LessThan,
    GEQ,
    LEQ,
}
