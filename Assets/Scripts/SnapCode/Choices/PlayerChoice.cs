using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events;
using UnityEngine;
using System;

public class PlayerChoice : IChoice<Player>
{
    public string Prompt => "Choose a player";

    public bool IsOptional => false;

    private List<Player> options;
    private bool isActive;

    public PlayerChoice(List<Player> options) {
        this.options = options;
        isActive = false;
    }

    public bool IsValidChoice(Player choice)
    {
        return options.Contains(choice);
    }

    public IEnumerator MakeChoice(Action<IEnumerable<Player>> onChoiceMade = null)
    {
        // For now, just select the first option automatically
        // In a real implementation, this would show a UI for the player to choose
        yield return new WaitForSeconds(0.1f); // Small delay to simulate choice time
        
        // Auto-select first option for now
        IEnumerable<Player> selectedPlayers = new List<Player>();
        if (options.Count > 0) {
            selectedPlayers = new List<Player> { options[0] };
        }
        
        onChoiceMade?.Invoke(selectedPlayers);
    }

    public void ConfirmChoice()
    {
        // For PlayerChoice, confirmation is handled automatically in MakeChoice
        // This method is provided for interface compliance
    }
} 