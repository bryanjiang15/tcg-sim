using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

[System.Serializable]
public struct AbilityDefinition {
    public AbilityTrigger trigger;
    public AbilityEffect effect;
    public AbilityAmount amount;
    public AbilityTargetDefinition targetDefinition;
    public List<AbilityRequirement> activationRequirements;
    public string description;
}

[System.Serializable]
public struct AbilityAmount{
    public AbilityAmountType type;
    public string value;
    public T GetValue<T>(){
        switch (type)
        {
            case AbilityAmountType.Constant:
                if (typeof(T) == typeof(int)) return (T)System.Convert.ChangeType(value, typeof(int));
                else if (typeof(T) == typeof(float)) return (T)System.Convert.ChangeType(value, typeof(float));
                else return default(T);
            default:
                return default(T);
        }
    }
}
//TODO: Implement this
[System.Serializable]
public struct AbilityRequirement {
    public AbilityTarget ReqTarget;
    public AbilityTargetRange ReqTargetRange;
    public AbilityRequirementType ReqType;
    public AbilityRequirementComparator ReqComparator;
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