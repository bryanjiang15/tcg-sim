using System;
using System.Collections.Generic;

public interface ISnapComponentDefinition {
    public SnapComponentType componentType { get; }  
}

[Serializable]
public class SnapActionDefinition: ISnapComponentDefinition {
    public SnapComponentType componentType => SnapComponentType.Action;
    
    public AbilityEffectType effect;
    public AbilityAmount amount;
    public List<AbilityTargetDefinition> targetDefinition;
}

[Serializable]
public class SnapIfDefinition: ISnapComponentDefinition {
    public SnapComponentType componentType => SnapComponentType.If;

    public AbilityRequirement requirement;

    public AbilityTargetDefinition requirementTarget;
}

[Serializable]
public class SnapElseDefinition: ISnapComponentDefinition {
    public SnapComponentType componentType => SnapComponentType.Else;
}

[Serializable]
public class SnapEndConditionDefinition: ISnapComponentDefinition {
    public SnapComponentType componentType => SnapComponentType.EndCondition;
}

[Serializable]
public class SnapWhileDefinition: ISnapComponentDefinition {
    public SnapComponentType componentType => SnapComponentType.While;

    public AbilityRequirement requirement;
}

[Serializable]
public class SnapChoiceDefinition: ISnapComponentDefinition {
    public SnapComponentType componentType => SnapComponentType.Choice;

    public AbilityChoiceDefinition choiceDefinition;
    
}