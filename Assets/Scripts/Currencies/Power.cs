using TMPro;
using UnityEngine;
using UnityEngine.Events;

public class Power : MonoBehaviour {
    public TextMeshPro PowerText;
    public int powerlevel { get; private set; } = 0;

    void Start()
    {
        UpdatePowerText();
    }

    void UpdatePowerText()
    {
        PowerText.text = powerlevel.ToString();
    }

    public void Change(int diff)
    {
        powerlevel += diff;
        UpdatePowerText();
    }
    
    public void SetPower(int amount){
        powerlevel = amount;
        UpdatePowerText();
    }

}