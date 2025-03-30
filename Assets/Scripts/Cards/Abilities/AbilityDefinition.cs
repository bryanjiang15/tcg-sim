using System.Collections.Generic;
using System.IO;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;
using Newtonsoft.Json;

[System.Serializable]
public struct AbilityDefinition {
    public AbilityTrigger trigger;
    public AbilityEffect effect;
    public AbilityAmount amount;
    public List<AbilityTargetDefinition> targetDefinition;
    public List<AbilityRequirement> activationRequirements;
    public AbilityTargetDefinition activationRequirementTargets;
    public string description;
}

[System.Serializable]
public struct AbilityAmount{
    public AbilityAmountType type;
    public string value;
    public T GetValue<T>(SnapCard owner){
        switch (type)
        {
            case AbilityAmountType.Constant:
                if (typeof(T) == typeof(int)) return (T)System.Convert.ChangeType(value, typeof(int));
                else if (typeof(T) == typeof(float)) return (T)System.Convert.ChangeType(value, typeof(float));
                else return default(T);
            case AbilityAmountType.ForEachTarget:
                return (T)System.Convert.ChangeType(GetForEachTargetValue(owner), typeof(T));
            case AbilityAmountType.TargetValue:
                return (T)System.Convert.ChangeType(GetTargetValue(owner), typeof(T));
            case AbilityAmountType.Boolean:
                if (typeof(T) == typeof(bool)) return (T)System.Convert.ChangeType(value, typeof(bool));
                return default(T);
            default:
                return default(T);
        }
    }

    private int GetForEachTargetValue(SnapCard owner) {
        var targetValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
        AbilityTargetDefinition targetDefinition = JsonConvert.DeserializeObject<AbilityTargetDefinition>(targetValues["target"].ToString());
        int multiplierValue = int.Parse(targetValues["value"].ToString());
        List<SnapCard> targets = TargetSystem.Instance.GetTargets(new List<AbilityTargetDefinition> { targetDefinition }, owner);
        return targets.Count * multiplierValue;
    }

    private int GetTargetValue(SnapCard owner) {
        var jsonValue = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
        AbilityRequirementType requirementType = (AbilityRequirementType)System.Enum.Parse(typeof(AbilityRequirementType), jsonValue["type"].ToString());
        AbilityTargetDefinition targetDef = JsonConvert.DeserializeObject<AbilityTargetDefinition>(jsonValue["target"].ToString());
        List<SnapCard> targetCards = TargetSystem.Instance.GetTargets(new List<AbilityTargetDefinition> { targetDef }, owner);
        CalculationType calculationType = (CalculationType)System.Enum.Parse(typeof(CalculationType), jsonValue["calculation"].ToString());
        if (calculationType == CalculationType.MaxValue)
        {
            int maxValue = 0;
            foreach (var targetCard in targetCards)
            {
                AbilityAmount value = TargetSystem.Instance.GetTargetValue(requirementType, targetCard);
                int cardValue = value.GetValue<int>(targetCard);
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
                int cardValue = value.GetValue<int>(targetCard);
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
                totalValue += value.GetValue<int>(targetCard);
            }
            return totalValue;
        }
    }

    public AbilityTargetDefinition GetDependentTargetDefinition(SnapCard owner){
        switch (type)
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
    public AbilityRequirementType ReqType;
    public AbilityRequirementComparator ReqComparator;
    public AbilityRequirementCondition ReqCondition;
    public AbilityAmount ReqAmount;
}

[System.Serializable]
public struct AbilityTargetDefinition {
    public AbilityTarget target;
    public AbilityTargetRange targetRange;
    public AbilityTargetSort targetSort;
    public List<AbilityRequirement> targetRequirement;
    public bool excludeSelf;
}