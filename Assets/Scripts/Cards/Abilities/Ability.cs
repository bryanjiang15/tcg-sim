using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardHouse;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class Ability
{
    public AbilityDefinition definition;
    public Type abilityEffectType { get; protected set; }
    public SnapCard owner { get; private set; }
    public bool exhaust = false;

    private Action<GameAction> unsubscribeActionCallback;

    public void SetOwner(SnapCard owner){
        this.owner = owner;
    }

    public GameAction getAbilityEffect(List<ITargetable> targets = null, GameAction triggeredAction = null) {
        if (targets == null) {
            targets = TargetSystem.Instance.GetTargets(definition.targetDefinition, owner, triggeredAction: triggeredAction);
        }
        if (definition.effect == AbilityEffect.Draw) {
            return new DrawCardGA(definition.amount.GetValue<int>(owner, triggeredAction), owner.ownedPlayer, source: owner);
        }
        if (definition.effect == AbilityEffect.GainMaxEnergy) {
            return new GainMaxEnergyGA(this, owner.ownedPlayer, definition.amount);
        }
        if (targets.Count == 0) return null;
        return (GameAction)Activator.CreateInstance(abilityEffectType, this, targets, definition.amount);
    }

    public static Dictionary<AbilityEffect, Type> AbilityEffectTypeMap = new Dictionary<AbilityEffect, Type>(){
        {AbilityEffect.GainPower, typeof(GainPowerGA)},
        {AbilityEffect.LosePower, typeof(GainPowerGA)},
        {AbilityEffect.Draw, typeof(DrawCardGA)},
        {AbilityEffect.Discard, typeof(DiscardCardGA)},
        {AbilityEffect.Destroy, typeof(DestroyCardGA)},
        {AbilityEffect.Move, typeof(MoveCardGA)},
        {AbilityEffect.StealPower, typeof(StealPowerGA)},
        {AbilityEffect.AddPowerToLocation, typeof(AddPowerToLocationGA)},
        {AbilityEffect.CreateCardInHand, typeof(CreateCardInHandGA)},
        {AbilityEffect.CreateCardInDeck, typeof(CreateCardInDeckGA)},
        {AbilityEffect.CreateCardInLocation, typeof(CreateCardInLocationGA)},
        {AbilityEffect.ReduceCost, typeof(IncreaseCostGA)}, // Negative amount reduces cost
        {AbilityEffect.IncreaseCost, typeof(IncreaseCostGA)},
        {AbilityEffect.Merge, typeof(MergeCardsGA)},
        {AbilityEffect.Return, typeof(ReturnCardGA)},
        {AbilityEffect.AddCardToHand, typeof(AddCardToHandGA)},
        {AbilityEffect.AddCardToLocation, typeof(AddCardToLocationGA)},
        {AbilityEffect.SetPower, typeof(SetPowerGA)},
        {AbilityEffect.AddKeyword, typeof(AddKeywordGA)},
        {AbilityEffect.AddTemporaryAbility, typeof(AddTemporaryAbilityGA)},
        // Add more cases as needed for additional AbilityEffect values
    };

    public void SetUpDefinition(AbilityDefinition abilityDefinition)
    {
        definition = abilityDefinition;
        abilityEffectType = AbilityEffectTypeMap[abilityDefinition.effect];
    }

    public void SetUnsubscribeActionCallback(Action<GameAction> callback) {
        unsubscribeActionCallback = callback;
    }

    public void UnsubscribeActionCallback() {
        //activate the callback if it is not null
        if (unsubscribeActionCallback != null) {
            unsubscribeActionCallback.Invoke(null);
            unsubscribeActionCallback = null;
        }
    }
}
