using System;
using System.Collections;
using System.Collections.Generic;
using CardHouse;
using UnityEngine;
using UnityEngine.Events;

public class Ability : MonoBehaviour
{
    public AbilityDefinition definition;
    public AbilityTrigger trigger;

    public Func<IEnumerator> abilityEffect { get; private set; }

    private void Start()
    {
    }

    public void SetUpDefinition(AbilityDefinition abilityDefinition)
    {
        definition = abilityDefinition;
        trigger = definition.trigger;
        switch(definition.effect){
            case AbilityEffect.GainPower:
                if(definition.amount.type != AbilityAmountType.Constant){
                    Debug.LogError("Ability.SetupDefinition: AbilityAmountType is not Constant");
                    break;
                }
                abilityEffect = new Func<IEnumerator>(() => {
                    StatBuff buff = new StatBuff(BuffType.AdditionalPower, int.Parse(definition.amount.value));
                    GetComponent<SnapCard>().buffs.Add(buff);
                    return null;
                });
                break;
            //TODO: Implement other effects
        }
    }

    public IEnumerator ActivateAbility()
    {
        yield return abilityEffect();

        yield return null;
    }
}
