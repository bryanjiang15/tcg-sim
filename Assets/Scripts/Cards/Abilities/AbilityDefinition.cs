using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using Newtonsoft.Json;
using System.Linq;

[System.Serializable]
public struct AbilityDefinition {
    public AbilityTriggerDefinition triggerDefinition;
    public AbilityEffectType effect;
    public AbilityAmount amount;
    public List<AbilityTargetDefinition> targetDefinition;
    public List<AbilityRequirement> activationRequirements;
    public AbilityTargetDefinition activationRequirementTargets;
    public string description;
}

[System.Serializable]
public struct AbilityAmount{
    public AbilityAmountType amountType;
    public string value;
    public T GetValue<T>(ITargetable owner, GameAction triggeredAction = null) {
        SnapCard snapCard = owner as SnapCard;
        switch (amountType)
        {
            case AbilityAmountType.Constant:
                if (typeof(T) == typeof(int)) return (T)System.Convert.ChangeType(value, typeof(int));
                else if (typeof(T) == typeof(float)) return (T)System.Convert.ChangeType(value, typeof(float));
                else return default(T);
            case AbilityAmountType.ForEachTarget:
                if (snapCard != null) return (T)System.Convert.ChangeType(GetForEachTargetValue(snapCard, triggeredAction), typeof(T));
                else return default(T);
            case AbilityAmountType.TargetValue:
                if (snapCard != null) return (T)System.Convert.ChangeType(GetTargetValue(snapCard, triggeredAction), typeof(T));
                else return default(T);
            case AbilityAmountType.Boolean:
                if (typeof(T) == typeof(bool)) return (T)System.Convert.ChangeType(value, typeof(bool));
                return default(T);
            case AbilityAmountType.Cardid:
                if (typeof(T) == typeof(string)) return (T)System.Convert.ChangeType(value, typeof(string));
                return default(T);
            default:
                return default(T);
        }
    }

    private int GetForEachTargetValue(SnapCard owner, GameAction triggeredAction = null) {
        var targetValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
        AbilityTargetDefinition targetDefinition = JsonConvert.DeserializeObject<AbilityTargetDefinition>(targetValues["target"].ToString());
        int multiplierValue = int.Parse(targetValues["value"].ToString());
        List<ITargetable> targets = TargetSystem.Instance.GetTargets(new List<AbilityTargetDefinition> { targetDefinition }, owner, triggeredAction: triggeredAction);
        return targets.Count * multiplierValue;
    }
 
    private int GetTargetValue(SnapCard owner, GameAction triggeredAction = null) {
        var jsonValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
        AbilityRequirementType requirementType = (AbilityRequirementType)System.Enum.Parse(typeof(AbilityRequirementType), jsonValue["type"].ToString());
        AbilityTargetDefinition targetDef = JsonConvert.DeserializeObject<AbilityTargetDefinition>(jsonValue["target"].ToString());
        List<ITargetable> targetCards = TargetSystem.Instance.GetTargets(new List<AbilityTargetDefinition> { targetDef }, owner, triggeredAction: triggeredAction);
        CalculationType calculationType = (CalculationType)System.Enum.Parse(typeof(CalculationType), jsonValue["calculation"].ToString());
        if (calculationType == CalculationType.MaxValue)
        {
            int maxValue = 0;
            foreach (var targetCard in targetCards)
            {
                AbilityAmount value = targetCard.GetTargetValue(requirementType);
                int cardValue = value.GetValue<int>(targetCard, triggeredAction);
                if (cardValue > maxValue)
                {
                    maxValue = cardValue;
                }
            }
            return maxValue;
        }else if (calculationType == CalculationType.MinValue)
        {
            int minValue = int.MaxValue;
            foreach (var targetCard in targetCards)
            {
                AbilityAmount value = TargetSystem.Instance.GetTargetValue(requirementType, targetCard);
                int cardValue = value.GetValue<int>(targetCard, triggeredAction);
                if (cardValue < minValue)
                {
                    minValue = cardValue;
                }
            }
            return minValue;
        }else
        {
            int totalValue = 0;
            foreach (var targetCard in targetCards)
            {
                AbilityAmount value = TargetSystem.Instance.GetTargetValue(requirementType, targetCard);
                totalValue += value.GetValue<int>(targetCard, triggeredAction);
            }
            return totalValue;
        }
    }

    public AbilityTargetDefinition GetDependentTargetDefinition(SnapCard owner){
        switch (amountType)
        {
            case AbilityAmountType.ForEachTarget:
                var targetValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
                return JsonConvert.DeserializeObject<AbilityTargetDefinition>(targetValues["target"].ToString());
            default:
                return default(AbilityTargetDefinition);
        }
    }
}
//TODO: Implement this
[System.Serializable]
public struct AbilityRequirement {
    public AbilityRequirementType requirementType;
    public AbilityRequirementComparator requirementComparator;
    public AbilityRequirementCondition requirementCondition;
    public AbilityAmount requirementAmount;
}

[System.Serializable]
public struct AbilityTargetDefinition {
    public AbilityTargetType targetType;
    public AbilityTargetRange targetRange;
    public AbilityTargetSort targetSort;
    public List<AbilityRequirement> targetRequirements;
    public bool excludeSelf;

    public AbilityTargetDefinition(AbilityTargetType target, AbilityTargetRange targetRange = AbilityTargetRange.None, AbilityTargetSort targetSort = AbilityTargetSort.None) {
        this.targetType = target;
        this.targetRange = targetRange;
        this.targetSort = targetSort;
        this.targetRequirements = new List<AbilityRequirement>();
        this.excludeSelf = false;
    }
}

[System.Serializable]
public struct AbilityTriggerDefinition 
{
    public AbilityTriggerType triggerType;
    public List<AbilityTargetDefinition> triggerSource;//Targets that can trigger the ability
}