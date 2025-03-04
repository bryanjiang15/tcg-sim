using CardHouse;
using UnityEngine;

public struct SnapCardStats {
    public int power;
    public int cost;
    public string card_name;
    public int series;

    public SnapCardStats(int power, int cost, string name, int series) {
        this.power = power;
        this.cost = cost;
        this.card_name = name;
        this.series = series;
    }
}

public class SnapCard : Card {
    public SnapCardStats stats { get ; private set; }

    public void initCardStats(SnapCardStats stats) {
        this.stats = stats;
        SnapCurrencyCost currencyCost = GetComponent<SnapCurrencyCost>();
        //currencyCost.SetAmount("Power", stats.power);
        currencyCost.SetAmount("Gems", stats.cost);
    }
}