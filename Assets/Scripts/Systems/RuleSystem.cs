using System;
using System.Collections.Generic;
using System.Data;
using CardHouse;
using UnityEngine;

public class RuleSystem : Singleton<RuleSystem> {

    //Effects that target cards on the board
    static List<AbilityEffectType> PlayedCardTargetingEffects = new List<AbilityEffectType> {
        AbilityEffectType.AddKeyword,
        AbilityEffectType.Afflict,
        AbilityEffectType.GainPower,
        AbilityEffectType.LosePower,
        AbilityEffectType.Destroy,
        AbilityEffectType.Move,
        //AbilityEffect.Discard,
        AbilityEffectType.Return,
        AbilityEffectType.Merge,
        AbilityEffectType.SetPower,
    };
    
    public bool checkEffectAmount(AbilityAmount amount, AbilityEffectType effect, SnapCard owner=null) {
        //Check for boolean restrictions
        if (amount.amountType == AbilityAmountType.Boolean)
        {
            List<AbilityEffectType> UnrestrictedEffects = new List<AbilityEffectType> {
                AbilityEffectType.Discard,
                AbilityEffectType.Destroy,
                AbilityEffectType.AddCardToLocation,
                AbilityEffectType.AddCardToHand,
                AbilityEffectType.CopyAndActivate,
            };
            if(!UnrestrictedEffects.Contains(effect)) return false;
        }else if (amount.amountType == AbilityAmountType.Cardid)
        {
            List<AbilityEffectType> UnrestrictedEffects = new List<AbilityEffectType> {
                AbilityEffectType.CreateCardInDeck,
                AbilityEffectType.CreateCardInHand,
                AbilityEffectType.CreateCardInLocation,
            };
            if(!UnrestrictedEffects.Contains(effect)) return false;
        }else 
        {
            List<AbilityEffectType> RestrictedEffects = new List<AbilityEffectType> {
                AbilityEffectType.CreateCardInHand,
                AbilityEffectType.CreateCardInDeck,
                AbilityEffectType.CreateCardInLocation,
            };
            if(RestrictedEffects.Contains(effect)) return false;
        }
        return true;
    }

    public bool checkAbilityTarget(AbilityTargetDefinition target, AbilityEffectType effect, AbilityTriggerDefinition trigger, GameAction triggeredAction = null) {
        if (target.targetType == AbilityTargetType.CreatedCard){
            if (trigger.triggerType != AbilityTriggerType.AfterAbilityTriggered) return false;
            List<Type> triggerableEffects = new List<Type> {
                typeof(CreateCardInHandGA),
                typeof(CreateCardInDeckGA),
                typeof(CreateCardInLocationGA),
            };
            if (!triggerableEffects.Contains(triggeredAction.GetType())) return false;
        }
        return true;
    }

    //Queueing up ability targeting the next played card. Check if ability is allowed:
    public bool checkAbilityTargetingNextCard(Ability ability) {
        List<AbilityTargetType> abilityTargets = new List<AbilityTargetType>();
        foreach (var targetDefinition in ability.definition.targetDefinition) {
            abilityTargets.Add(targetDefinition.targetType);
        }
        if (!abilityTargets.Contains(AbilityTargetType.NextPlayedCard)) return false;
        if (!PlayedCardTargetingEffects.Contains(ability.definition.effect)) return false;
        return true;
    }
}