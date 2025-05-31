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
        if (definition.effect == AbilityEffectType.Draw) {
            return new DrawCardGA(definition.amount.GetValue<int>(owner, triggeredAction), owner.ownedPlayer, source: owner);
        }
        if (definition.effect == AbilityEffectType.GainMaxEnergy) {
            return new GainMaxEnergyGA(this, owner.ownedPlayer, definition.amount);
        }
        if (targets.Count == 0) return null;
        return (GameAction)Activator.CreateInstance(abilityEffectType, this, targets, definition.amount);
    }

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

    public void SetUpDefinition(AbilityDefinition abilityDefinition)
    {
        definition = abilityDefinition;
        abilityEffectType = AbilityEffectTypeMap[abilityDefinition.effect];
        Debug.Log("AbilityDefinition: " + JsonUtility.ToJson(abilityDefinition, true));
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
