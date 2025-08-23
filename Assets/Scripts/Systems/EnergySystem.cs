using System.Collections;
using System.Collections.Generic;
using CardHouse;
using UnityEngine;

public class EnergySystem : Singleton<EnergySystem>
{
    [SerializeField] CurrencyRefillOperator currencyRefillOperator;
    public int EnergyGainPerTurn = 1;

    // Temporary energy boost system
    private Dictionary<Player, int> tempEnergyBoosts = new Dictionary<Player, int>();

    private void OnEnable()
    {
        ActionSystem.AttachPerformer<RefillEnergyGA>(RefillEnergyPerformer);
        ActionSystem.SubscribeReaction<BeginPhaseGA>(TurnStartGainEnergyReaction, ReactionTiming.POST);
    }

    private void OnDisable()
    {
        ActionSystem.DetachPerformer<RefillEnergyGA>();
        ActionSystem.UnsubscribeReaction<BeginPhaseGA>(TurnStartGainEnergyReaction, ReactionTiming.POST);
    }

    //GAMEACTION PERFOMER
    private IEnumerator RefillEnergyPerformer(RefillEnergyGA action)
    {
        IncreaseMaxEnergy(Player.Player1, EnergyGainPerTurn);
        IncreaseMaxEnergy(Player.Player2, EnergyGainPerTurn);
        currencyRefillOperator.Activate();
        yield return new WaitForSeconds(0.1f);
        ApplyTemporaryEnergyBoost();
    }
    //GAMEACTION SUBSCRIBER
    private void TurnStartGainEnergyReaction(BeginPhaseGA action)
    {
        if (SnapPhaseManager.Instance.GetCurrentPhaseType() == SnapPhaseType.Preparation)
        {
            ActionSystem.Instance.AddReaction(new RefillEnergyGA());
        }
    }

    public void IncreaseMaxEnergy(Player player, int amount)
    {
        int playerIndex = (int)player;
        CurrencyRegistry.Instance.AdjustCurrencyRefillValue("Energy", playerIndex, amount);
    }

    public void DecreaseMaxEnergy(Player player, int amount)
    {
        int playerIndex = (int)player;
        CurrencyRegistry.Instance.AdjustCurrencyRefillValue("Energy", playerIndex, -amount);
    }

    // Temporary energy boost methods
    public void AddTemporaryEnergyBoost(Player player, int amount)
    {
        if (!tempEnergyBoosts.ContainsKey(player))
        {
            tempEnergyBoosts[player] = 0;
        }
        tempEnergyBoosts[player] += amount;
    }

    private void ApplyTemporaryEnergyBoost()
    {
        // Apply temporary energy boost for all players
        foreach (var kvp in tempEnergyBoosts)
        {
            if (kvp.Value > 0)
            {
                IncreaseMaxEnergy(kvp.Key, kvp.Value);
            }
        }
        
        // Reset all temporary energy boosts
        tempEnergyBoosts.Clear();
    }

    public int GetTemporaryEnergyBoost(Player player)
    {
        return tempEnergyBoosts.ContainsKey(player) ? tempEnergyBoosts[player] : 0;
    }
}