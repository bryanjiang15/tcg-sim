using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

public enum SnapConditionType
{
    IfElse,
    Switch,
    For,
}

[Serializable]
public abstract class SnapConditions: ISnapComponent
{
    public SnapConditionType conditionType;

    public abstract IEnumerator Execute(Ability ability, GameAction triggeredAction = null, ITargetable triggeredTarget = null);
}

public class SnapIfCondition: SnapConditions
{
    public SnapIfCondition()
    {
        conditionType = SnapConditionType.IfElse;
    }

    //Will need to update ability requirement. For now, have a requirementTarget and apply requirement to that target
    public AbilityRequirement requirement;
    public AbilityTargetDefinition requirementTarget;

    public List<ISnapComponent> trueActions;
    public List<ISnapComponent> falseActions;

    public override IEnumerator Execute(Ability ability, GameAction triggeredAction = null, ITargetable triggeredTarget = null)
    {
        List<ITargetable> targets = TargetSystem.Instance.GetTargets(requirementTarget, ability.owner);

        if (TargetSystem.Instance.IsRequirementMet(requirement, targets))
        {
            foreach (var action in trueActions)
            {
                yield return action.Execute(ability, triggeredAction, triggeredTarget);
            }
        }
        else
        {
            foreach (var action in falseActions)
            {
                yield return action.Execute(ability, triggeredAction, triggeredTarget);
            }
        }
    }
}

public class SnapWhileCondition: SnapConditions
{
    public SnapWhileCondition()
    {
        this.conditionType = SnapConditionType.For;
    }

    public AbilityRequirement requirement;
    public AbilityTargetDefinition requirementTarget;
    public List<ISnapComponent> loopActions;

    public override IEnumerator Execute(Ability ability, GameAction triggeredAction = null, ITargetable triggeredTarget = null)
    {
        List<ITargetable> targets = TargetSystem.Instance.GetTargets(requirementTarget, ability.owner);

        while (TargetSystem.Instance.IsRequirementMet(requirement, targets))
        {
            foreach (var action in loopActions)
            {
                yield return action.Execute(ability, triggeredAction, triggeredTarget);
            }
            
            // Re-evaluate targets after each iteration
            targets = TargetSystem.Instance.GetTargets(requirementTarget, ability.owner);
        }
    }
}