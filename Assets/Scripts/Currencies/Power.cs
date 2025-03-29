using System.Collections.Generic;
using CardHouse;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

//Type: Card
public class Power : MonoBehaviour {
    public TextMeshPro PowerText;
    public int powerlevel { get; private set; } = 0;

    private int BasePower;
    public UnityEvent PowerChanged = new UnityEvent();

    void Awake()
    {   
        UpdatePowerText();
        SnapCard card = GetComponent<SnapCard>();
        card.BuffChanged.AddListener(UpdatePower);
    }

    void UpdatePowerText()
    {
        PowerText.text = powerlevel.ToString();
        if(powerlevel > BasePower){
            PowerText.color = Color.green;
        }else if(powerlevel < BasePower){
            PowerText.color = Color.red;
        }else{
            PowerText.color = Color.white;
        }
    }
    
    public void UpdatePower(){
        int oldPower = powerlevel;
        powerlevel = BasePower;
        List<StatBuff> powerBuffs = GetComponent<SnapCard>().buffs.FindAll(buff => buff.type == BuffType.AdditionalPower).ConvertAll(buff => (StatBuff)buff);
        foreach (StatBuff buff in powerBuffs)
        {
            powerlevel += buff.amount;
        }
        UpdatePowerText();
        if (oldPower != powerlevel)
            PowerChanged.Invoke();
    }

    //Call only on initialization
    public void SetBasePower(int basePower){
        BasePower = basePower;
        powerlevel = basePower;
        UpdatePowerText();
    }


}