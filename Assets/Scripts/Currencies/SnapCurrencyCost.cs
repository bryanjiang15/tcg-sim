using CardHouse;
using UnityEngine;

public class SnapCurrencyCost : CurrencyCost {
    
    public void SetAmount(string currencyName, int amount) {
        foreach (CostWithLabel currency in Cost) {
            if (currency.Cost.CurrencyType.Name == currencyName) {
                currency.Cost.Amount = amount;
                currency.Label.text = amount.ToString();
                return;
            }
        }
    }
}