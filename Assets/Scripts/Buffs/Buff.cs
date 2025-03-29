
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
    public BuffType type;
    public SnapCard source;
    public OngoingAbility ongoingAbilitySource;
}

public class StatBuff : Buff
{
    public int amount;

    public StatBuff(BuffType type, SnapCard source=null, int amount=0, OngoingAbility ongoingAbilitySource=null)
    {
        this.type = type;
        this.amount = amount;
        this.source = source;
        this.ongoingAbilitySource = ongoingAbilitySource;
    }
}

public class UnplayableBuff : Buff
{
    public List<LocationPosition> locations;

    public UnplayableBuff(List<LocationPosition> locations, SnapCard source, OngoingAbility ongoingAbilitySource=null)
    {
        type = BuffType.Unplayable;
        this.locations = locations;
        this.source = source;
        this.ongoingAbilitySource = ongoingAbilitySource;
    }
}
