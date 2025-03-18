using UnityEngine;

public class EndPhaseGA : GameAction
{
    public EndPhaseGA()
    {
        this.PostReactions.Add(new BeginPhaseGA());
    }
    
}

public class BeginPhaseGA : GameAction
{
    public bool PhaseChanged;
    public BeginPhaseGA(bool PhaseChanged = true)
    {
        this.PhaseChanged = PhaseChanged;
    }

}