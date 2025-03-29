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
                if (typeof(T) == typeof(int))
                {
                    // Assuming value is a JSON string containing a dictionary of target values
                    var targetValues = JsonConvert.DeserializeObject<Dictionary<string, object>>(value);
                    AbilityTargetDefinition targetDefinition = JsonConvert.DeserializeObject<AbilityTargetDefinition>(targetValues["target"].ToString());
                    int multiplierValue = int.Parse(targetValues["value"].ToString());
                    List<SnapCard> targets = TargetSystem.Instance.GetTargets(new List<AbilityTargetDefinition> { targetDefinition }, owner);
                    return (T)System.Convert.ChangeType(targets.Count * multiplierValue, typeof(int));
                }
                return default(T);
            case AbilityAmountType.Boolean:
                if (typeof(T) == typeof(bool)) return (T)System.Convert.ChangeType(value, typeof(bool));
                return default(T);
            default:
                return default(T);
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