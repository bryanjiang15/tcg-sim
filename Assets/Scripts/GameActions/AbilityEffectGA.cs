using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class AbilityEffectGA : GameAction {
    public SnapCard owner; // The card that owns this ability effect
    public List<ITargetable> targets; // The list of targets affected by this ability
    public AbilityAmount amount; // The amount associated with this ability effect
    public Ability ability;

    public AbilityEffectGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) {
        this.ability = ability;
        this.owner = ability.owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class GainPowerGA : AbilityEffectGA, IPowerChangedEffect {
    public GainPowerGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class DiscardCardGA : AbilityEffectGA, IHandUpdated {
    public DiscardCardGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class DestroyCardGA : AbilityEffectGA, ILocationCardsUpdated {
    public DestroyCardGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class MoveCardGA : AbilityEffectGA, ILocationCardsUpdated
{
    public MoveCardGA(Ability ability, List<ITargetable> targets, AbilityAmount amount)
        : base(ability, targets, amount)
    {
    }
}

public class GainMaxEnergyGA : GameAction
{
    public SnapCard owner;
    public Player player;
    public AbilityAmount amount;

    public GainMaxEnergyGA(Ability ability, Player player, AbilityAmount amount)
    {
        this.owner = ability.owner;
        this.player = player;
        this.amount = amount;
    }
}

public class StealPowerGA : AbilityEffectGA, IPowerChangedEffect
{
    public StealPowerGA(Ability ability, List<ITargetable> targets, AbilityAmount amount)
        : base(ability, targets, amount)
    {
    }
}

public class CreateCardGA : AbilityEffectGA
{
    public List<SnapCard> createdCards;
    public CreateCardGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
        createdCards = new List<SnapCard>();
    }
}

public class CreateCardInLocationGA : CreateCardGA, ILocationCardsUpdated {

    public CreateCardInLocationGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class CreateCardInHandGA : CreateCardGA, IHandUpdated {
    public CreateCardInHandGA(Ability ability, List<ITargetable> targets, AbilityAmount amount)
        : base(ability, targets, amount)
    {
    }
}

public class CreateCardInDeckGA : CreateCardGA, IDeckUpdated {
    public CreateCardInDeckGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class AddPowerToLocationGA : AbilityEffectGA, IPowerChangedEffect {
    public AddPowerToLocationGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class IncreaseCostGA : AbilityEffectGA, ICostChangedEffect {
    public IncreaseCostGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class AddCardToHandGA : AbilityEffectGA, IHandUpdated, ILocationCardsUpdated, IDeckUpdated {
    public AddCardToHandGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class AddCardToLocationGA : AbilityEffectGA, IHandUpdated, ILocationCardsUpdated, IDeckUpdated {
    public AddCardToLocationGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class SetPowerGA : AbilityEffectGA, IPowerChangedEffect {
    public SetPowerGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class MergeCardsGA : AbilityEffectGA, ILocationCardsUpdated, IPowerChangedEffect {
    public List<SnapCard> MergeTarget;
    public SnapCard RecieveTarget;

    public MergeCardsGA(Ability ability, List<SnapCard> MergeTarget, SnapCard RecieveTarget) 
        : base(ability, MergeTarget.Cast<ITargetable>().ToList(), new AbilityAmount()) {
        this.MergeTarget = MergeTarget;
        this.RecieveTarget = RecieveTarget;
    }
}

public class ReturnCardGA : AbilityEffectGA, IHandUpdated, ILocationCardsUpdated {
    public ReturnCardGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class AddKeywordGA : AbilityEffectGA {
    public AddKeywordGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

public class AddTemporaryAbilityGA : AbilityEffectGA {
    public AddTemporaryAbilityGA(Ability ability, List<ITargetable> targets, AbilityAmount amount) 
        : base(ability, targets, amount) {
    }
}

//Marker Interfaces
public interface IPowerChangedEffect { }
public interface ICostChangedEffect { }
public interface ILocationCardsUpdated { }
public interface IHandUpdated { }
public interface IDeckUpdated { }
