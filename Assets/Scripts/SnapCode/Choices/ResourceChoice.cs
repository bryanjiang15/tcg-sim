using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;

public class ResourceChoice : IChoice<string>
{
    public string Prompt => "Choose a resource";

    public bool IsOptional => false;

    private List<string> options;
    private bool isActive;

    public ResourceChoice(List<string> options) {
        this.options = options;
        isActive = false;
    }

    public bool IsValidChoice(string choice)
    {
        return options.Contains(choice);
    }

    public IEnumerator MakeChoice(Action<IEnumerable<string>> onChoiceMade = null)
    {
        // For now, just select the first option automatically
        // In a real implementation, this would show a UI for the player to choose
        yield return new WaitForSeconds(0.1f); // Small delay to simulate choice time
        
        // Auto-select first option for now
        IEnumerable<string> selectedResources = new List<string>();
        if (options.Count > 0) {
            selectedResources = new List<string> { options[0] };
        }
        
        onChoiceMade?.Invoke(selectedResources);
    }

    public void ConfirmChoice()
    {
        // For ResourceChoice, confirmation is handled automatically in MakeChoice
        // This method is provided for interface compliance
    }
} 