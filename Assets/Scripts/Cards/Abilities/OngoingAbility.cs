using System;
using System.Collections.Generic;
using System.Linq;
using CardHouse;
using UnityEngine;
using UnityEngine.Events;

public class OngoingAbility : Ability {
    private List<SnapCard> currentTargets = new();
    static List<Type> ignoredTypes = new() {
        typeof(RefillEnergyGA),
        typeof(AddTemporaryAbilityGA),
    };
    public List<Type> listenedTypes;

    public OngoingAbility(Ability ability) {
        definition = ability.definition;
        SetOwner(ability.owner);
        abilityEffectType = ability.abilityEffectType;
    }

    void OnDestroy() {
        ActionSystem.UnsubscribeReaction<GameAction>(UpdateTargets, ReactionTiming.POST);
    }

    public void Activate() {
        listenedTypes = new List<Type>();
        listenedTypes.AddRange(ConfigureAmountListeners(definition.amount));
        listenedTypes.AddRange(ConfigureTargetListeners(definition.targetDefinition));

        ActionSystem.SubscribeReaction<GameAction>(UpdateTargets, ReactionTiming.POST);
        //Ongoing abilities does not have a triggered gameaction
        currentTargets = TargetSystem.Instance.GetTargets(definition.targetDefinition, owner).Cast<SnapCard>().ToList();
        ActionSystem.Instance.AddReaction(getAbilityEffect(currentTargets.Cast<ITargetable>().ToList()));
    }

    private void UpdateTargets(GameAction action) {
        if (!listenedTypes.Contains(action.GetType()) && 
            !listenedTypes.Exists(type => type.IsAssignableFrom(action.GetType()))) return;

        List<SnapCard> newTargets = TargetSystem.Instance.GetTargets(definition.targetDefinition, owner).Cast<SnapCard>().ToList();
        List<SnapCard> unaffectedTargets = newTargets.FindAll(target => !currentTargets.Contains(target));
        List<SnapCard> affectedTargets = currentTargets.FindAll(target => !newTargets.Contains(target));
        if (definition.amount.amountType == AbilityAmountType.ForEachTarget) {
            foreach(SnapCard target in newTargets) {
                int newAmount = definition.amount.GetValue<int>(target); 
                if (newAmount != GetTargetCurrentBuffAmount(target)) {
                    //Remove current buff from affected targets without triggering update, then add it to unaffected targets
                    RemoveAbilityEffect(new List<SnapCard> { target }, replacingBuff: true);
                    unaffectedTargets.Add(target);
                }
            }
            
        }
        if (affectedTargets.Count > 0) RemoveAbilityEffect(affectedTargets);
        if (unaffectedTargets.Count > 0) ActionSystem.Instance.AddReaction(getAbilityEffect(unaffectedTargets.Cast<ITargetable>().ToList()));
        
        currentTargets = newTargets;
        

        
    }

    private List<Type> ConfigureTargetListeners(List<AbilityTargetDefinition> targetDefinitions) {
        return GetListenedTypesFromTargetDefinitions(targetDefinitions);
    }

    private List<Type> ConfigureAmountListeners(AbilityAmount amount) {
        List<Type> typesToListenTo = new();
        switch (amount.amountType) {
            case AbilityAmountType.ForEachTarget:
                AbilityTargetDefinition targetDefinition = amount.GetDependentTargetDefinition(owner);
                return GetListenedTypesFromTargetDefinitions(new List<AbilityTargetDefinition> { targetDefinition });
            case AbilityAmountType.Constant:
                // Do not listen to any GameAction
                break;
            default:
                Debug.LogWarning($"Unhandled AbilityAmountType: {amount.amountType}");
                break;
        }
        return typesToListenTo;
    }

    public static List<Type> GetListenedTypesFromTargetDefinitions(List<AbilityTargetDefinition> targetDefinitions) {
        List<Type> typesToListenTo = new();
        foreach (var targetDefinition in targetDefinitions) {
            switch (targetDefinition.targetType) {
                case AbilityTargetType.Deck:
                    typesToListenTo.Add(typeof(IDeckUpdated));
                    break;
                case AbilityTargetType.Hand:
                    typesToListenTo.Add(typeof(IHandUpdated));
                    break;
                case AbilityTargetType.EnemyDeck:
                    typesToListenTo.Add(typeof(IDeckUpdated));
                    break;
                case AbilityTargetType.EnemyHand:
                    typesToListenTo.Add(typeof(IHandUpdated));
                    break;
                case AbilityTargetType.PlayerDirectLocationCards:
                case AbilityTargetType.EnemyDirectLocationCards:
                case AbilityTargetType.AllPlayerPlayedCards:
                case AbilityTargetType.AllEnemyPlayedCards:
                case AbilityTargetType.AllPlayedCards:
                    typesToListenTo.Add(typeof(ILocationCardsUpdated));
                    break;
                case AbilityTargetType.AllPlayerCards:
                case AbilityTargetType.AllEnemyCards:
                    typesToListenTo.Add(typeof(CreateCardGA));
                    break;
                case AbilityTargetType.PlayerDirectLocation:
                    typesToListenTo.Add(typeof(ILocationCardsUpdated));
                    typesToListenTo.Add(typeof(AddPowerToLocationGA));
                    break;
                case AbilityTargetType.AllPlayerLocation:
                    typesToListenTo.Add(typeof(ILocationCardsUpdated));
                    typesToListenTo.Add(typeof(AddPowerToLocationGA));
                    break;
                    // Add more cases as needed
            }
        }
        return typesToListenTo;
    }

    public GameAction RemoveAbilityEffect(List<SnapCard> targets, bool replacingBuff = false) {
        foreach (SnapCard target in targets) {
            Buff buffToRemove = target.buffs.Find(buff => buff.source == owner);
            if (buffToRemove != null) {
                 // Remove the buff from the target
                target.RemoveBuff(buffToRemove, replacingRemovedBuff: replacingBuff);
            }
        }
        return null; // Return null as no specific GameAction is performed here
    }

    public int GetTargetCurrentBuffAmount(SnapCard target) {
        StatBuff buff = target.buffs.Find(
            buff => buff.source == owner &&
            buff is StatBuff) as StatBuff;
        if (buff != null) {
            return buff.amount;
        }
        return 0;
    }
}