using System.Collections;
using CardHouse;
using UnityEngine;
using UnityEngine.Animations;


public class SnapPhaseManager : PhaseManager {
    public int Turn;
    public int maxTurn;

    public Phase StartPhase;
    public Phase EndPhase;
    

    public override Phase CurrentPhase => (CurrentPhaseIndex == -1) ? StartPhase : 
                                    (CurrentPhaseIndex == Phases.Count) ? EndPhase : 
                                    (CurrentPhaseIndex >= 0 && CurrentPhaseIndex < Phases.Count) ? Phases[CurrentPhaseIndex] : null;

    void Awake()
    {
        Instance = this;
    }

    IEnumerator Start()
    {
        CurrentPhaseIndex = -1;
        yield return new WaitForEndOfFrame();
        if (CurrentPhase != null)
        {
            StartCoroutine(CurrentPhase.Start());
        }
    }

    IEnumerator HardResetRoutine()
    {
        CurrentPhaseIndex = -1;
        yield return new WaitForEndOfFrame();
        if (CurrentPhase != null)
        {
            StartCoroutine(CurrentPhase.Start());
            OnPhaseChanged?.Invoke(CurrentPhase);
        }
    }

    IEnumerator PhaseTransition()
    {
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
        }

        yield return CurrentPhase.Start();
        OnPhaseChanged?.Invoke(CurrentPhase);
    }
}