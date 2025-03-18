using System;
using System.Collections;
using System.Collections.Generic;
using CardHouse;
using UnityEngine;
using UnityEngine.Events;

public class Ability
{
    public AbilityDefinition definition;
    public GameAction abilityTrigger { get; private set; }
    public GameAction abilityEffect { get; private set; }
    public SnapCard owner { get; private set; }

    public void SetUpDefinition(AbilityDefinition abilityDefinition)
    {
        definition = abilityDefinition;
        switch(definition.effect){
            case AbilityEffect.GainPower:
                abilityEffect = new GainPowerGA(owner, definition.targetDefinition, definition.amount);
                break;
            //TODO: Implement other effects
        }
    }

    public void SetOwner(SnapCard owner){
        this.owner = owner;
    }

    private void OnDestroy() {
        AbilityManager.Instance.UnregisterAbility(this);
    }

}
