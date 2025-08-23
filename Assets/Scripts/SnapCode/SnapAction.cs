using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

[Serializable]
public class SnapAction: ISnapComponent
{
    public AbilityEffectType effect;
    public AbilityAmount amount;
    public List<AbilityTargetDefinition> targetDefinition;

    //Execute: To execute the action, add the corresponding effect to the ActionSystem reaction chain
    public IEnumerator Execute(Ability ability, GameAction triggeredAction = null, ITargetable triggeredTarget = null)
    {
        GameAction gameAction = getGameAction(ability, triggeredAction);
        bool IsPerforming = true;
        if (gameAction != null) {
            ActionSystem.Instance.Perform(gameAction, () => {
                IsPerforming = false;
            });
        }
        while (IsPerforming) {
            yield return null;
        }
    }

    public GameAction getGameAction(Ability ability, GameAction triggeredAction = null) {

        List<ITargetable> targets = TargetSystem.Instance.GetTargets(targetDefinition, ability.owner, triggeredAction: triggeredAction);

        if (effect == AbilityEffectType.Draw) {
            return new DrawCardGA(amount.GetValue<int>(ability.owner, triggeredAction), ability.owner.ownedPlayer, source: ability.owner);
        }
        if (effect == AbilityEffectType.GainMaxEnergy) {
            return new GainMaxEnergyGA(ability, ability.owner.ownedPlayer, amount);
        }
        if (targets.Count == 0) return null;
        return (GameAction)Activator.CreateInstance(EffectMapper.AbilityEffectTypeMap[effect], ability, targets, amount);
    }
}