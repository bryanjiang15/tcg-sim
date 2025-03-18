using System.Collections;
using CardHouse;
using UnityEngine;

public class EnergySystem : MonoBehaviour {
    [SerializeField] CurrencyRefillOperator currencyRefillOperator;
    public int EnergyGainPerTurn = 1;

    private void OnEnable() {
        ActionSystem.AttachPerformer<RefillEnergyGA>(RefillEnergyPerformer);
        ActionSystem.SubscribeReaction<BeginPhaseGA>(TurnStartGainEnergyReaction, ReactionTiming.POST);
    }

    private void OnDisable() {
        ActionSystem.DetachPerformer<RefillEnergyGA>();
        ActionSystem.UnsubscribeReaction<BeginPhaseGA>(TurnStartGainEnergyReaction, ReactionTiming.POST);
    }

    //GAMEACTION PERFOMER
    private IEnumerator RefillEnergyPerformer(RefillEnergyGA action) {
        CurrencyRegistry.Instance.AdjustCurrencyRefillValue("Energy", 0, EnergyGainPerTurn);
        CurrencyRegistry.Instance.AdjustCurrencyRefillValue("Energy", 1, EnergyGainPerTurn);
        currencyRefillOperator.Activate();
        yield return new WaitForSeconds(0.1f);
    }
    //GAMEACTION SUBSCRIBER
    private void TurnStartGainEnergyReaction(BeginPhaseGA action) {
        if (SnapPhaseManager.Instance.GetCurrentPhaseType() == SnapPhaseType.Preparation) {
            ActionSystem.Instance.AddReaction(new RefillEnergyGA());
        }
    }

}