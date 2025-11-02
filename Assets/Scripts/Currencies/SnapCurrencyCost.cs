using CardHouse;
using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public class SnapCurrencyCost : CurrencyCost {
    private int BaseEnergyCost;
    public UnityEvent CostChanged = new UnityEvent();
    private SnapCard card;

    void Awake() {
        card = GetComponent<SnapCard>();
        card.BuffChanged.AddListener(UpdateCost);
    }

    public void SetAmount(string currencyName, int amount) {
        foreach (CostWithLabel currency in Cost) {
            if (currency.Cost.CurrencyType.Name == currencyName) {
                currency.Cost.Amount = amount;
                currency.Label.text = amount.ToString();
                return;
            }
        }
    }

    public void SetBaseEnergyCost(int baseCost) {
        BaseEnergyCost = baseCost;
        UpdateCost();
    }

    public void UpdateCost() {
        int totalCost = BaseEnergyCost;
        List<StatBuff> costBuffs = card.buffs.FindAll(buff => buff.statType?.Name == "Cost" && buff.buffModifierType == BuffModifierType.Add).ConvertAll(buff => (StatBuff)buff);
        foreach (StatBuff buff in costBuffs) {
            totalCost -= buff.amount;
        }
        SetAmount("Energy", totalCost);
        CostChanged.Invoke();
    }

    public int GetCost() {
        return Cost.Find(currency => currency.Cost.CurrencyType.Name == "Energy").Cost.Amount;
    }

    public int GetBaseCost() {
        return BaseEnergyCost;
    }
}