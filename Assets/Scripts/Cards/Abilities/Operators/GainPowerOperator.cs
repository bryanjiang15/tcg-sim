using CardHouse;
using UnityEngine;

public class GainPowerOperator : Activatable
{
    public int amount;

    protected override void OnActivate()
    {
        GetComponent<Power>().AddPower(amount);
    }
    
}