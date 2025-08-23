using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CardHouse;
using UnityEngine;

[Serializable]
public class SnapTrigger
{
    public AbilityTriggerDefinition trigger;
    public AbilityActivationLocation activationLocation;
    public List<string> specifiedActivationLocations;
    public ReactionTiming reactionTiming;
    public List<ISnapComponent> actions;
    public Ability ability;

    private void TriggerAction(GameAction triggerGA)
    {
        if (activationLocation == AbilityActivationLocation.AllUnexcluded)
        {
            GroupName groupName = GroupRegistry.Instance.GetGroupName(ability.owner.Group);
            if (specifiedActivationLocations.Contains(groupName.ToString()))
            {
                return;
            }
        }
        else if (activationLocation == AbilityActivationLocation.AllIncluded)
        {
            GroupName groupName = GroupRegistry.Instance.GetGroupName(ability.owner.Group);
            if (!specifiedActivationLocations.Contains(groupName.ToString()))
            {
                return;
            }
        }
        List<ITargetable> triggerTargets = GetTriggerTargets(triggerGA);

        ability.owner.StartCoroutine(ExecuteActionChain(triggerGA, triggerTargets));
    }

    //Currently, the order of trigger is based on order of initialization. TODO: Make order based on current player's trigger, then the other player's trigger
    public void Register(Ability ability)
    {
        this.ability = ability;
        ActionSystem.SubscribeReaction(TriggerMapper.GetTriggerAction(trigger.triggerType), TriggerAction, reactionTiming);
    }

    public void Unregister()
    {
        this.ability = null;
        ActionSystem.UnsubscribeReaction(TriggerMapper.GetTriggerAction(trigger.triggerType), TriggerAction, reactionTiming);
    }

    private List<ITargetable> GetTriggerTargets(GameAction triggerGA)
    {
        if (triggerGA is AbilityEffectGA abilityEffectGA)
        {
            List<ITargetable> validTriggerTargets = TargetSystem.Instance.GetTargets(trigger.triggerSource, ability.owner, triggeredAction: triggerGA);

            if (validTriggerTargets.Count == 0) 
            {
                Debug.Log($"This reaction does not have valid trigger targets for ability: {ability.owner.name}, {trigger.triggerSource[0].targetType}");
                return new List<ITargetable>();
            }
            else 
            {
                List<ITargetable> triggeredTargets = abilityEffectGA.targets;
                return validTriggerTargets.Intersect(triggeredTargets).ToList();
            }
        }
        else if (triggerGA is RevealCardGA onRevealGA)
        {
            if (onRevealGA.card == ability.owner)
            {
                return new List<ITargetable> { ability.owner };
            }
        }

        return new List<ITargetable>();
    }

    private IEnumerator ExecuteActionChain(GameAction triggerGA, List<ITargetable> triggerTargets)
    {
        //For each target that triggers the reaction, execute the actions
        foreach (var target in triggerTargets)
        {
            AbilityManager.Instance.AbilityChain.Push(ability);
            //Execute Action chain
            foreach (var action in actions)
            {
                while (ActionSystem.Instance.IsPerforming || AbilityManager.Instance.AbilityChain.Peek() != ability) yield return null;
                yield return action.Execute(ability, triggerGA, target);
            }
            AbilityManager.Instance.AbilityChain.Pop();
        }
        
    }
}