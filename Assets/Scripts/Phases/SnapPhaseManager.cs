using System;
using System.Collections;
using CardHouse;
using UnityEngine;
using UnityEngine.Animations;

public enum SnapPhaseType
{
    Start,
    Preparation,
    Reveal,
    End
}

public class SnapPhaseManager : PhaseManager {
    public int Turn;
    public int maxTurn;
    

    public Phase StartPhase;
    public Phase EndPhase;
    
    public override Phase CurrentPhase => (CurrentPhaseIndex == -1) ? StartPhase : 
                                    (CurrentPhaseIndex == Phases.Count) ? EndPhase : 
                                    (CurrentPhaseIndex >= 0 && CurrentPhaseIndex < Phases.Count) ? Phases[CurrentPhaseIndex] : null;
    public static new SnapPhaseManager Instance;
    void Awake()
    {
        PhaseManager.Instance = this;
        Instance = this;
    }

    private void OnEnable() {
        ActionSystem.AttachPerformer<BeginPhaseGA>(BeginPhasePerformer);
        ActionSystem.AttachPerformer<EndPhaseGA>(EndPhasePerformer);

    }
    private void OnDisable() {
        ActionSystem.DetachPerformer<BeginPhaseGA>();
        ActionSystem.DetachPerformer<EndPhaseGA>();
    }

    //GAMEACTION PERFORMERS
    private IEnumerator EndPhasePerformer(EndPhaseGA action) {
        yield return CurrentPhase.End();

        if(CurrentPhaseIndex == -1){
            CurrentPhaseIndex = 0;
        } else if(CurrentPhaseIndex == Phases.Count-1){
            if(Turn >= maxTurn){
                CurrentPhaseIndex = Phases.Count;
            }else{
                CurrentPhaseIndex = 0;
                Turn++;
            }
        } else {
            CurrentPhaseIndex++;
        }
    }

    //Phase events on start/end should be mainly UI changes. Any actions such as drawing card/refilling energy should be handled by GameActions.
    private IEnumerator BeginPhasePerformer(BeginPhaseGA action) {
        yield return CurrentPhase.Start();
    }
    //END GAMEACTION PERFORMERS

    IEnumerator Start()
    {
        CurrentPhaseIndex = -1;
        yield return new WaitForEndOfFrame();
        if (CurrentPhase != null)
        {
            ActionSystem.Instance.Perform(new BeginPhaseGA(false));
        }
    }

    IEnumerator HardResetRoutine()
    {
        CurrentPhaseIndex = -1;
        yield return new WaitForEndOfFrame();
        if (CurrentPhase != null)
        {
            ActionSystem.Instance.Perform(new BeginPhaseGA());
            while (ActionSystem.Instance.IsPerforming) yield return null;
            OnPhaseChanged?.Invoke(CurrentPhase);
        }
    }

    public override IEnumerator PhaseTransition()
    {
        ActionSystem.Instance.Perform(new EndPhaseGA());

        while (ActionSystem.Instance.IsPerforming) yield return null;
        OnPhaseChanged?.Invoke(CurrentPhase);
    }

    //TODO: more abstract way of handling phases in case we want flexible phase transitions in the future
    public SnapPhaseType GetCurrentPhaseType()
    {
        if (CurrentPhaseIndex == -1)
        {
            return SnapPhaseType.Start;
        }
        else if (CurrentPhaseIndex == Phases.Count)
        {
            return SnapPhaseType.End;
        }
        else if (CurrentPhaseIndex==0)
        {
            return SnapPhaseType.Preparation;
        }
        else
        {
            return SnapPhaseType.Reveal;
        }
    }
}