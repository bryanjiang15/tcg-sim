using UnityEditor.ShaderGraph.Serialization;
using UnityEngine;

[System.Serializable]
public struct AbilityDefinition {
    public AbilityTrigger trigger;
    public AbilityEffect effect;
    public AbilityAmount amount;
    public AbilityTarget target;
    public AbilityTargetRange targetRange;
    public AbilityTargetSort targetSort;
    public AbilityRequirement requirement;
    public bool excludeSelf;
    public string description;
}

[System.Serializable]
public struct AbilityAmount{
    public AbilityAmountType type;
    public string value;
}
//TODO: Implement this
[System.Serializable]
public struct AbilityRequirement {
    public bool hasRequirement;
    public AbilityTarget ReqTarget;
    public AbilityTargetRange ReqTargetRange;
    public AbilityTargetSort ReqTargetSort;
    public AbilityRequirementType ReqType;
    public AbilityRequirementComparator ReqComparator;
    public AbilityAmount ReqAmount;
}