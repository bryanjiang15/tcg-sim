using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CardHouse;
using System.Collections;
using System;

/// <summary>
/// SnapApi provides a comprehensive interface for AI agents to interact with the TCG game.
/// This API allows external applications to query game state, perform actions, and control game flow.
/// All methods are designed to be thread-safe and provide detailed feedback for AI decision-making.
/// </summary>
public class SnapApi : Singleton<SnapApi> {
    
    #region Game State Queries
    #endregion
    
    #region Card Actions
    
    /// <summary>
    /// Plays a card from hand to a specific location.
    /// </summary>
    /// <param name="player">The player playing the card</param>
    /// <param name="cardIndex">Index of the card in hand (0-based)</param>
    /// <param name="locationPosition">Target location position</param>
    /// <returns>Dictionary containing result: success (bool), message (string), error (string if failed)</returns>
    public IEnumerator UpdateStats(string statName, int amount, IEnumerable<ITargetable> targets, Ability ability = null, GameAction triggeredAction = null) {
        var result = new Dictionary<string, object>();
        
        //validation

        StatType statType = StatTypeRegistry.Instance.GetStatTypeByName(statName);
        if (statType == null) {
            result["success"] = false;
            result["error"] = $"Stat type {statName} not found";
            yield return result;
        }
        if (statType.StatValueType != StatValueType.ValueStat) {
            result["success"] = false;
            result["error"] = $"Stat type {statName} is not a value stat";
            yield return result;
        }

        List<IBuffObtainable> targetsList = targets.Cast<IBuffObtainable>().ToList();
        
        UpdateStatGA updateStatGA = new UpdateStatGA(statName, amount, targetsList);

        yield return Execute(updateStatGA);
        
    }
    
    #endregion

    #region Utility Functions
    public IEnumerator Execute(GameAction gameAction)
    {
        while (ActionSystem.Instance.IsPerforming) yield return null;

        if (gameAction != null) {
            ActionSystem.Instance.Perform(gameAction);
        }

        while (ActionSystem.Instance.IsPerforming) yield return null;
    }

    public GameAction getGameAction(AbilityEffectType effect, ITargetable target, int amount, Ability ability, GameAction triggeredAction = null) {

        if (effect == AbilityEffectType.Draw) {
            return new DrawCardGA(amount, ability.owner.ownedPlayer, source: ability.owner);
        }
        if (effect == AbilityEffectType.GainMaxEnergy) {
            return new GainMaxEnergyGA(ability, ability.owner.ownedPlayer, new AbilityAmount { amountType = AbilityAmountType.Constant, value = amount.ToString() });
        }
        if (target == null) return null;
        return (GameAction)Activator.CreateInstance(EffectMapper.AbilityEffectTypeMap[effect], ability, target, new AbilityAmount { amountType = AbilityAmountType.Constant, value = amount.ToString() });
    }

    #endregion
}