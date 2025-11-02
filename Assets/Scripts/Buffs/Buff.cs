
using System.Collections.Generic;
using UnityEditor.ShaderGraph.Serialization;

public enum BuffType
{
    AdditionalPower,
    AdditionalCost,
    AdditionEnergy,
    AdditionalMaxEnergy,
    SetPower,
    SetCost,
    Indestructible,
    Unmovable,
    Unplayable,
    Silenced,
    Created,
}
public class Buff
{
    public StatTypeModal statType;
    public BuffModifierType buffModifierType;
    public string value;
    public int sourceCardInstanceId;
}

public class StatBuff : Buff
{
    public int amount;

    public StatBuff(StatTypeModal statType, BuffModifierType modifierType, int amount, int sourceCardInstanceId = 0)
    {
        this.statType = statType;
        this.buffModifierType = modifierType;
        this.amount = amount;
        this.value = amount.ToString();
        this.sourceCardInstanceId = sourceCardInstanceId;
    }

}

public class UnplayableBuff : Buff
{
    public List<LocationPosition> locations;

    public UnplayableBuff(List<LocationPosition> locations, int sourceCardInstanceId = 0)
    {
        this.locations = locations;
        this.statType = null; // UnplayableBuff doesn't target a specific stat
        this.buffModifierType = BuffModifierType.Set;
        this.value = "true";
        this.sourceCardInstanceId = sourceCardInstanceId;
    }
}
