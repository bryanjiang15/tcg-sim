using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;

public class CardChoice : IChoice<SnapCard>
{
    public string Prompt => "Choose a target card";

    public bool IsOptional => false;

    private List<SnapCard> options;
    private bool isConfirmed;

    public CardChoice(List<SnapCard> options) {
        this.options = options;
        isConfirmed = false;
    }

    public bool IsValidChoice(SnapCard choice)
    {
        return options.Contains(choice);
    }

    public IEnumerator MakeChoice(Action<IEnumerable<SnapCard>> onChoiceMade = null)
    {
        ChoiceSystem.Instance.ShowChoice(this);
        
        foreach (SnapCard option in options) {
            option.SetSelectable(true);
        }
        while (!isConfirmed) {
            yield return null;
        }
        
        onChoiceMade?.Invoke(options.FindAll(option => option.IsSelected));
    }

    public void ConfirmChoice()
    {
        isConfirmed = true;
    }
}