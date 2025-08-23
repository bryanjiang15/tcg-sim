using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;

public class SnapChoice<T> : ISnapComponent {
    AbilityChoiceDefinition choiceDefinition;

    public SnapChoice(AbilityChoiceDefinition choiceDefinition) {
        this.choiceDefinition = choiceDefinition;
    }

    public IEnumerator Execute(Ability ability, GameAction triggeredAction = null, ITargetable triggeredTarget = null) {
        // Generate choice options based on choiceDefinition
        List<T> choiceOptions = GenerateChoiceOptions(ability, triggeredAction, triggeredTarget);
        
        if (choiceOptions.Count == 0) {
            Debug.LogWarning("No choice options available for SnapChoice");
            yield break;
        }
        
        // Create the appropriate choice based on choiceType
        IChoice<T> choice = CreateChoice(choiceOptions);
        
        // Wait for the choice to be made
        yield return choice.MakeChoice((IEnumerable<T> choices) => {
            ability.owner.context.AddVariable(choiceDefinition.choiceName, choices);
        });
        
    }

    private List<T> GenerateChoiceOptions(Ability ability, GameAction triggeredAction, ITargetable triggeredTarget) {
        List<T> options = new List<T>();
        
        if (choiceDefinition.choiceTargets == null || choiceDefinition.choiceTargets.Count == 0) {
            return options;
        }
        
        // Get targets based on choiceTargets
        List<ITargetable> targets = TargetSystem.Instance.GetTargets(choiceDefinition.choiceTargets, ability.owner, triggeredAction);
        
        // Convert targets to the appropriate type T
        foreach (var target in targets) {
            if (target is T typedTarget) {
                options.Add(typedTarget);
            }
        }
        
        return options;
    }

    private IChoice<T> CreateChoice(List<T> options) {
        // Create choice based on the choiceType
        switch (choiceDefinition.choiceType) {
            case AbilityChoiceType.Card:
                if (typeof(T) == typeof(SnapCard)) {
                    return new CardChoice(options.Cast<SnapCard>().ToList()) as IChoice<T>;
                }
                break;
            case AbilityChoiceType.Location:
                if (typeof(T) == typeof(Location)) {
                    return new LocationChoice(options.Cast<Location>().ToList()) as IChoice<T>;
                }
                break;
            case AbilityChoiceType.Player:
                if (typeof(T) == typeof(Player)) {
                    return new PlayerChoice(options.Cast<Player>().ToList()) as IChoice<T>;
                }
                break;
            case AbilityChoiceType.Resource:
                if (typeof(T) == typeof(string)) {
                    return new ResourceChoice(options.Cast<string>().ToList()) as IChoice<T>;
                }
                break;
            default:
                Debug.LogError($"Unsupported choice type: {choiceDefinition.choiceType}");
                break;
        }
        
        // Fallback to a generic choice if type doesn't match
        return new GenericChoice<T>(options);
    }

    //Implementation
    //Choosing a target adds the target to the ability's list of temporary property
    //TargetDefinition can use the temporary property to determine the target
    //When the ability is executed, the temporary property is removed
    //AI: create a name for the temporary property and generate a list of choices. 
}