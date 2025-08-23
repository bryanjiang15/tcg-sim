using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;

public class LocationChoice : IChoice<Location>
{
    public string Prompt => "Choose a location";

    public bool IsOptional => false;

    private List<Location> options;
    private bool isActive;

    public LocationChoice(List<Location> options) {
        this.options = options;
        isActive = false;
    }

    public bool IsValidChoice(Location choice)
    {
        return options.Contains(choice);
    }

    public IEnumerator MakeChoice(Action<IEnumerable<Location>> onChoiceMade = null)
    {
        // For now, just select the first option automatically
        // In a real implementation, this would show a UI for the player to choose
        yield return new WaitForSeconds(0.1f); // Small delay to simulate choice time
        
        // Auto-select first option for now
        IEnumerable<Location> selectedLocations = new List<Location>();
        if (options.Count > 0) {
            selectedLocations = new List<Location> { options[0] };
        }
        
        onChoiceMade?.Invoke(selectedLocations);
    }

    public void ConfirmChoice()
    {
        // For LocationChoice, confirmation is handled automatically in MakeChoice
        // This method is provided for interface compliance
    }
} 