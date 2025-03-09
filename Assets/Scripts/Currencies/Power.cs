using System.Collections.Generic;
using CardHouse;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

//Type: Card or Location
public class Power : MonoBehaviour {
    public TextMeshPro PowerText;
    public int powerlevel { get; private set; } = 0;

    private int BasePower;

    public List<Buff> Buffs = new List<Buff>();

    void Start()
    {
        UpdatePowerText();
    }

    void UpdatePowerText()
    {
        PowerText.text = powerlevel.ToString();
    }

    public void AddPower(int diff)
    {
        powerlevel += diff;
        UpdatePowerText();
    }

    public void DecreasePower(int diff)
    {
        powerlevel -= diff;
        UpdatePowerText();
    }
    
    public void SetPower(int amount){
        powerlevel = amount;
        UpdatePowerText();
    }

}