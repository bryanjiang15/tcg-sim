using System;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Reflection;
using System.Collections;

public class AbilityManager : MonoBehaviour {
    public static AbilityManager Instance;

    //AbilityManager: sets up reactions for all abilities in game.
    //When reacting to game action, make sure any reactions are added in order of Owner, same player, enemy player
    private List<Ability> abilities = new List<Ability>();

    private void Awake() {
        Instance = this;
    }

    private void Start() {
        List<Location> locations = new List<Location>(FindObjectsOfType<Location>());
        ActionSystem.AttachPerformer<GainPowerGA>(GainPowerPerformer);
    }

    public List<Ability> GetAbilities() {
        return abilities;
    }

    public void RegisterAbility(Ability ability) {
        abilities.Add(ability);
    }
    public void UnregisterAbility(Ability ability) {
        abilities.Remove(ability);
    }

    //Call during performance of OnrevealGA
    public void ActivateOnRevealAbility(SnapCard card){
        foreach (Ability ability in abilities)
        {
            AbilityTrigger trigger = ability.definition.trigger;
            if ((trigger == AbilityTrigger.OnReveal || trigger == AbilityTrigger.Ongoing) && ability.owner == card)
            {
                ActionSystem.Instance.AddReaction(ability.abilityEffect);
            }
        }
    }

    //GAMEACTION PERFORMER

    private IEnumerator GainPowerPerformer(GainPowerGA action) {
        List<SnapCard> targets = TargetSystem.Instance.GetTargets(action.target, action.owner);
        foreach (SnapCard target in targets) {
            Buff powerBuff = new StatBuff(BuffType.AdditionalPower, action.amount.GetValue<int>());
            target.ApplyBuff(powerBuff);   
        }
        yield return null;
    }
}