using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.Events;

public class GenericChoice<T> : IChoice<T>
{
    public string Prompt => $"Choose from {typeof(T).Name} options";

    public bool IsOptional => false;

    private List<T> options;
    private bool isActive;

    public GenericChoice(List<T> options) {
        this.options = options;
        isActive = false;
    }

    public bool IsValidChoice(T choice)
    {
        return options.Contains(choice);
    }

    public IEnumerator MakeChoice(Action<IEnumerable<T>> onChoiceMade = null)
    {
        // For now, just select the first option automatically
        // In a real implementation, this would show a UI for the player to choose
        yield return new WaitForSeconds(0.1f); // Small delay to simulate choice time
        
        // Auto-select first option for now
        IEnumerable<T> selectedOptions = new List<T>();
        if (options.Count > 0) {
            selectedOptions = new List<T> { options[0] };
        }
        
        onChoiceMade?.Invoke(selectedOptions);
    }

    public void ConfirmChoice()
    {
        // For GenericChoice, confirmation is handled automatically in MakeChoice
        // This method is provided for interface compliance
    }
} 