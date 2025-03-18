using System.Collections.Generic;
using UnityEngine;

public class GainPowerGA : GameAction {
    public SnapCard owner;
    public AbilityTargetDefinition target;
    public AbilityAmount amount;
    public GainPowerGA(SnapCard owner, AbilityTargetDefinition target, AbilityAmount amount){
        this.owner = owner;
        this.target = target;
        this.amount = amount;
    }
}