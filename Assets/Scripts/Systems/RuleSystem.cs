using System;
using System.Collections.Generic;
using System.Data;
using CardHouse;
using UnityEngine;

public class RuleSystem : Singleton<RuleSystem> {
    
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
}