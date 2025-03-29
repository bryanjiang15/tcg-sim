using System.Collections.Generic;
using UnityEngine;

public class GainPowerGA : GameAction, IPowerChangedEffect {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;
    public GainPowerGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class DiscardCardGA : GameAction, IHandUpdated {
    public SnapCard owner;
    public List<SnapCard> targets;

    public AbilityAmount amount;

    public DiscardCardGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class DestroyCardGA : GameAction, ILocationCardsUpdated {
    public SnapCard owner;
    public List<SnapCard> targets;

    public AbilityAmount amount;

    public DestroyCardGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class MoveCardGA : GameAction, ILocationCardsUpdated {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;

    public MoveCardGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class StealPowerGA : GameAction, IPowerChangedEffect {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;

    public StealPowerGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class CreateCardInLocationGA : GameAction, ILocationCardsUpdated {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;

    public CreateCardInLocationGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class CreateCardInHandGA : GameAction, IHandUpdated {
    public SnapCard owner;
    public AbilityAmount amount;
    public List<SnapCard> targets;

    public CreateCardInHandGA(SnapCard owner, AbilityAmount amount, List<SnapCard> targets) {
        this.owner = owner;
        this.amount = amount;
        this.targets = targets;
    }
}

public class CreateCardInDeckGA : GameAction, IDeckUpdated {
    public SnapCard owner;
    public AbilityAmount amount;
   public List<SnapCard> targets;

    public CreateCardInDeckGA(SnapCard owner, AbilityAmount amount, List<SnapCard> targets) {
        this.owner = owner;
        this.amount = amount;
        this.targets = targets;
    }
}

public class AddPowerToLocationGA : GameAction, IPowerChangedEffect {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;

    public AddPowerToLocationGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

//Reduce cost by setting amount as negative
public class IncreaseCostGA : GameAction, ICostChangedEffect {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;

    public IncreaseCostGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class AddCardToHandGA : GameAction, IHandUpdated, ILocationCardsUpdated, IDeckUpdated {
    public SnapCard owner;
    public List<SnapCard> targets;

    //Player
    public AbilityAmount amount;

    public AddCardToHandGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class AddCardToLocationGA : GameAction, IHandUpdated, ILocationCardsUpdated, IDeckUpdated  {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;

    public AddCardToLocationGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class MergeCardsGA : GameAction, ILocationCardsUpdated, IPowerChangedEffect {
    public SnapCard owner;
    public List<SnapCard> MergeTarget;
    public SnapCard RecieveTarget;

    public MergeCardsGA(SnapCard owner, List<SnapCard> MergeTarget, SnapCard RecieveTarget) {
        this.owner = owner;
        this.MergeTarget = MergeTarget;
        this.RecieveTarget = RecieveTarget;
    }
}

public class ReturnCardGA : GameAction, IHandUpdated, ILocationCardsUpdated {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;

    public ReturnCardGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class AddKeywordGA : GameAction {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;

    public AddKeywordGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

public class AddTemporaryAbilityGA : GameAction {
    public SnapCard owner;
    public List<SnapCard> targets;
    public AbilityAmount amount;

    public AddTemporaryAbilityGA(SnapCard owner, List<SnapCard> targets, AbilityAmount amount) {
        this.owner = owner;
        this.targets = targets;
        this.amount = amount;
    }
}

//Marker Interfaces
public interface IPowerChangedEffect { }
public interface ICostChangedEffect { }
public interface ILocationCardsUpdated { }
public interface IHandUpdated { }
public interface IDeckUpdated { }
