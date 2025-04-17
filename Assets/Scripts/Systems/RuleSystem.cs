using System;
using System.Collections.Generic;
using System.Data;
using CardHouse;
using UnityEngine;

public class RuleSystem : Singleton<RuleSystem> {

    //Effects that target cards on the board
    static List<AbilityEffect> PlayedCardTargetingEffects = new List<AbilityEffect> {
        AbilityEffect.AddKeyword,
        AbilityEffect.Afflict,
        AbilityEffect.GainPower,
        AbilityEffect.LosePower,
        AbilityEffect.Destroy,
        AbilityEffect.Move,
        //AbilityEffect.Discard,
        AbilityEffect.Return,
        AbilityEffect.Merge,
        AbilityEffect.SetPower,
    };
    
    public bool checkEffectAmount(AbilityAmount amount, AbilityEffect effect, SnapCard owner=null) {
        //Check for boolean restrictions
        if (amount.type == AbilityAmountType.Boolean)
        {
            List<AbilityEffect> UnrestrictedEffects = new List<AbilityEffect> {
                AbilityEffect.Discard,
                AbilityEffect.Destroy,
                AbilityEffect.AddCardToLocation,
                AbilityEffect.AddCardToHand,
                AbilityEffect.CopyAndActivate,
            };
            if(!UnrestrictedEffects.Contains(effect)) return false;
        }else if (amount.type == AbilityAmountType.Cardid)
        {
            List<AbilityEffect> UnrestrictedEffects = new List<AbilityEffect> {
                AbilityEffect.CreateCardInDeck,
                AbilityEffect.CreateCardInHand,
                AbilityEffect.CreateCardInLocation,
            };
            if(!UnrestrictedEffects.Contains(effect)) return false;
        }else 
        {
            List<AbilityEffect> RestrictedEffects = new List<AbilityEffect> {
                AbilityEffect.CreateCardInHand,
                AbilityEffect.CreateCardInDeck,
                AbilityEffect.CreateCardInLocation,
            };
            if(RestrictedEffects.Contains(effect)) return false;
        }
        return true;
    }

    public bool checkAbilityTarget(AbilityTargetDefinition target, AbilityEffect effect, AbilityTriggerDefinition trigger, GameAction triggeredAction = null) {
        if (target.target == AbilityTarget.CreatedCard){
            if (trigger.trigger != AbilityTrigger.AfterAbilityTriggered) return false;
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
        List<AbilityTarget> abilityTargets = new List<AbilityTarget>();
        foreach (var targetDefinition in ability.definition.targetDefinition) {
            abilityTargets.Add(targetDefinition.target);
        }
        if (!abilityTargets.Contains(AbilityTarget.NextPlayedCard)) return false;
        if (!PlayedCardTargetingEffects.Contains(ability.definition.effect)) return false;
        return true;
    }
}