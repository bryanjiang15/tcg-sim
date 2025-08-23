using System;
using System.Collections.Generic;

//Temprorary until AblityTrigger becomes custom
public class TriggerMapper
{
    public static Type GetTriggerAction(AbilityTriggerType triggerType)
    {
        return triggerType switch
        {
            AbilityTriggerType.OnReveal => typeof(RevealCardGA),
            AbilityTriggerType.Ongoing => typeof(GameAction), // Ongoing abilities don't have a specific trigger action
            AbilityTriggerType.GameStart => typeof(BeginPhaseGA),
            AbilityTriggerType.EndTurn => typeof(EndPhaseGA),
            AbilityTriggerType.EndGame => typeof(EndPhaseGA),
            AbilityTriggerType.InHand => typeof(GameAction), // Hand abilities don't have a specific trigger action
            AbilityTriggerType.InDeck => typeof(GameAction), // Deck abilities don't have a specific trigger action
            AbilityTriggerType.Destroyed => typeof(DestroyCardGA),
            AbilityTriggerType.Discarded => typeof(DiscardCardGA),
            AbilityTriggerType.Moved => typeof(MoveCardGA),
            AbilityTriggerType.Banished => typeof(DestroyCardGA), // Banished is similar to destroyed
            AbilityTriggerType.StartTurn => typeof(BeginPhaseGA),
            AbilityTriggerType.Activate => typeof(GameAction), // Manual activation
            AbilityTriggerType.BeforeCardPlayed => typeof(RevealCardGA),
            AbilityTriggerType.AfterCardPlayed => typeof(RevealCardGA),
            AbilityTriggerType.AfterAbilityTriggered => typeof(AbilityEffectGA),
            AbilityTriggerType.OnCreated => typeof(CreateCardGA),
            AbilityTriggerType.None => typeof(GameAction),
            _ => typeof(GameAction) // Default case
        };
    }
}
