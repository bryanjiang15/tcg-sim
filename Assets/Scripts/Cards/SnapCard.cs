using System.Collections;
using CardHouse;
using UnityEngine;
using UnityEngine.Events;

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

    [HideInInspector] public bool revealed { get; private set; } = false;

    public UnityEvent CardRevealed = new UnityEvent();

    public void initCardStats(SnapCardStats stats) {
        this.stats = stats;
        SnapCurrencyCost currencyCost = GetComponent<SnapCurrencyCost>();
        currencyCost.SetAmount("Gems", stats.cost);
        
        Power power = GetComponent<Power>();
        power.SetPower(stats.power);
    }

    public IEnumerator revealCard() {
        CardRevealed.Invoke();
        yield return null;
    }
}