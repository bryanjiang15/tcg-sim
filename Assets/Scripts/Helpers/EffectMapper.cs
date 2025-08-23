using System;
using System.Collections.Generic;

public class EffectMapper {
    public static Dictionary<AbilityEffectType, Type> AbilityEffectTypeMap = new Dictionary<AbilityEffectType, Type>(){
        {AbilityEffectType.GainPower, typeof(GainPowerGA)},
        {AbilityEffectType.LosePower, typeof(GainPowerGA)},
        {AbilityEffectType.Draw, typeof(DrawCardGA)},
        {AbilityEffectType.Discard, typeof(DiscardCardGA)},
        {AbilityEffectType.Destroy, typeof(DestroyCardGA)},
        {AbilityEffectType.Move, typeof(MoveCardGA)},
        {AbilityEffectType.StealPower, typeof(StealPowerGA)},
        {AbilityEffectType.AddPowerToLocation, typeof(AddPowerToLocationGA)},
        {AbilityEffectType.CreateCardInHand, typeof(CreateCardInHandGA)},
        {AbilityEffectType.CreateCardInDeck, typeof(CreateCardInDeckGA)},
        {AbilityEffectType.CreateCardInLocation, typeof(CreateCardInLocationGA)},
        {AbilityEffectType.ReduceCost, typeof(IncreaseCostGA)}, // Negative amount reduces cost
        {AbilityEffectType.IncreaseCost, typeof(IncreaseCostGA)},
        {AbilityEffectType.Merge, typeof(MergeCardsGA)},
        {AbilityEffectType.Return, typeof(ReturnCardGA)},
        {AbilityEffectType.AddCardToHand, typeof(AddCardToHandGA)},
        {AbilityEffectType.AddCardToLocation, typeof(AddCardToLocationGA)},
        {AbilityEffectType.SetPower, typeof(SetPowerGA)},
        {AbilityEffectType.AddKeyword, typeof(AddKeywordGA)},
        {AbilityEffectType.AddTemporaryAbility, typeof(AddTemporaryAbilityGA)},
        // Add more cases as needed for additional AbilityEffect values
    };
}