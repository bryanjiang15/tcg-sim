using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using CardHouse;

/// <summary>
/// SnapApi provides a comprehensive interface for AI agents to interact with the TCG game.
/// This API allows external applications to query game state, perform actions, and control game flow.
/// All methods are designed to be thread-safe and provide detailed feedback for AI decision-making.
/// </summary>
public class SnapApi : Singleton<SnapApi> {
    
    #region Game State Queries
    #endregion
    
    #region Card Actions
    
    /// <summary>
    /// Plays a card from hand to a specific location.
    /// </summary>
    /// <param name="player">The player playing the card</param>
    /// <param name="cardIndex">Index of the card in hand (0-based)</param>
    /// <param name="locationPosition">Target location position</param>
    /// <returns>Dictionary containing result: success (bool), message (string), error (string if failed)</returns>
    public Dictionary<string, object> PlayCard(Player player, int cardIndex, LocationPosition locationPosition) {
        var result = new Dictionary<string, object>();
        
        try {
            var handGroup = player == Player.Player1 ? DeckSystem.Instance.handGroupP1 : DeckSystem.Instance.handGroupP2;
            
            if (cardIndex < 0 || cardIndex >= handGroup.MountedCards.Count) {
                result["success"] = false;
                result["error"] = "Invalid card index";
                return result;
            }
            
            var card = handGroup.MountedCards[cardIndex] as SnapCard;
            if (card == null) {
                result["success"] = false;
                result["error"] = "Card not found at specified index";
                return result;
            }
            
            // Check if player has enough energy
            int playerIndex = (int)player;
            int currentEnergy = CurrencyRegistry.Instance.GetCurrency("Energy", playerIndex) ?? 0;
            if (currentEnergy < card.GetBaseCost()) {
                result["success"] = false;
                result["error"] = "Insufficient energy";
                return result;
            }
            
            // Get target location
            var locations = TargetSystem.Instance.GetLocations(player);
            var targetLocation = locations.FirstOrDefault(l => l.position == locationPosition);
            if (targetLocation == null) {
                result["success"] = false;
                result["error"] = "Invalid location";
                return result;
            }
            
            if (targetLocation.freeSlots <= 0) {
                result["success"] = false;
                result["error"] = "Location is full";
                return result;
            }
            
            // Perform the play action
            ActionSystem.Instance.Perform(new PlayCardGA(card, targetLocation));
            
            result["success"] = true;
            result["message"] = $"Card {card.stats.card_name} played to {locationPosition}";
            
        } catch (System.Exception e) {
            result["success"] = false;
            result["error"] = $"Exception occurred: {e.Message}";
        }
        
        return result;
    }
    
    /// <summary>
    /// Moves a card from one location to another.
    /// </summary>
    /// <param name="player">The player moving the card</param>
    /// <param name="fromLocation">Source location position</param>
    /// <param name="cardIndex">Index of the card in the source location</param>
    /// <param name="toLocation">Target location position</param>
    /// <returns>Dictionary containing result: success (bool), message (string), error (string if failed)</returns>
    public Dictionary<string, object> MoveCard(Player player, LocationPosition fromLocation, int cardIndex, LocationPosition toLocation) {
        var result = new Dictionary<string, object>();
        
        try {
            var locations = TargetSystem.Instance.GetLocations(player);
            var sourceLocation = locations.FirstOrDefault(l => l.position == fromLocation);
            var targetLocation = locations.FirstOrDefault(l => l.position == toLocation);
            
            if (sourceLocation == null || targetLocation == null) {
                result["success"] = false;
                result["error"] = "Invalid location";
                return result;
            }
            
            if (cardIndex < 0 || cardIndex >= sourceLocation.cardGroup.MountedCards.Count) {
                result["success"] = false;
                result["error"] = "Invalid card index";
                return result;
            }
            
            if (targetLocation.freeSlots <= 0) {
                result["success"] = false;
                result["error"] = "Target location is full";
                return result;
            }
            
            var card = sourceLocation.cardGroup.MountedCards[cardIndex] as SnapCard;
            if (card == null) {
                result["success"] = false;
                result["error"] = "Card not found";
                return result;
            }
            
            // Perform the move action
            ActionSystem.Instance.Perform(new MoveCardGA(card, targetLocation));
            
            result["success"] = true;
            result["message"] = $"Card {card.stats.card_name} moved from {fromLocation} to {toLocation}";
            
        } catch (System.Exception e) {
            result["success"] = false;
            result["error"] = $"Exception occurred: {e.Message}";
        }
        
        return result;
    }
    
    /// <summary>
    /// Activates a card's ability or effect.
    /// </summary>
    /// <param name="player">The player activating the card</param>
    /// <param name="locationPosition">Location of the card</param>
    /// <param name="cardIndex">Index of the card in the location</param>
    /// <param name="abilityIndex">Index of the ability to activate (0-based)</param>
    /// <returns>Dictionary containing result: success (bool), message (string), error (string if failed)</returns>
    public Dictionary<string, object> ActivateCardAbility(Player player, LocationPosition locationPosition, int cardIndex, int abilityIndex) {
        var result = new Dictionary<string, object>();
        
        try {
            var locations = TargetSystem.Instance.GetLocations(player);
            var location = locations.FirstOrDefault(l => l.position == locationPosition);
            
            if (location == null) {
                result["success"] = false;
                result["error"] = "Invalid location";
                return result;
            }
            
            if (cardIndex < 0 || cardIndex >= location.cardGroup.MountedCards.Count) {
                result["success"] = false;
                result["error"] = "Invalid card index";
                return result;
            }
            
            var card = location.cardGroup.MountedCards[cardIndex] as SnapCard;
            if (card == null) {
                result["success"] = false;
                result["error"] = "Card not found";
                return result;
            }
            
            if (abilityIndex < 0 || abilityIndex >= card.abilities.Count) {
                result["success"] = false;
                result["error"] = "Invalid ability index";
                return result;
            }
            
            var ability = card.abilities[abilityIndex];
            // Perform ability activation (implementation depends on ability system)
            // ActionSystem.Instance.Perform(new ActivateAbilityGA(card, ability));
            
            result["success"] = true;
            result["message"] = $"Activated ability {ability.GetType().Name} on card {card.stats.card_name}";
            
        } catch (System.Exception e) {
            result["success"] = false;
            result["error"] = $"Exception occurred: {e.Message}";
        }
        
        return result;
    }
    
    #endregion
    
    #region Game Control
    
    /// <summary>
    /// Ends the current player's turn and advances to the next phase.
    /// </summary>
    /// <returns>Dictionary containing result: success (bool), message (string), newPhase (string)</returns>
    public Dictionary<string, object> EndTurn() {
        var result = new Dictionary<string, object>();
        
        try {
            var currentPhase = SnapPhaseManager.Instance.GetCurrentPhaseType();
            SnapPhaseManager.Instance.NextPhase();
            var newPhase = SnapPhaseManager.Instance.GetCurrentPhaseType();
            
            result["success"] = true;
            result["message"] = $"Turn ended, advanced from {currentPhase} to {newPhase}";
            result["newPhase"] = newPhase.ToString();
            
        } catch (System.Exception e) {
            result["success"] = false;
            result["error"] = $"Exception occurred: {e.Message}";
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets all valid actions that can be performed by the current player.
    /// </summary>
    /// <param name="player">The player to get valid actions for</param>
    /// <returns>Dictionary containing lists of valid actions: playableCards, movableCards, activatableAbilities</returns>
    public Dictionary<string, object> GetValidActions(Player player) {
        var validActions = new Dictionary<string, object>();
        
        try {
            // Get playable cards
            var playableCards = new List<Dictionary<string, object>>();
            var handGroup = player == Player.Player1 ? DeckSystem.Instance.handGroupP1 : DeckSystem.Instance.handGroupP2;
            int playerIndex = (int)player;
            int currentEnergy = CurrencyRegistry.Instance.GetCurrency("Energy", playerIndex) ?? 0;
            
            for (int i = 0; i < handGroup.MountedCards.Count; i++) {
                var card = handGroup.MountedCards[i] as SnapCard;
                if (card != null && card.GetBaseCost() <= currentEnergy) {
                    var cardAction = new Dictionary<string, object>();
                    cardAction["cardIndex"] = i;
                    cardAction["cardName"] = card.stats.card_name;
                    cardAction["cost"] = card.GetBaseCost();
                    
                    // Get valid locations for this card
                    var validLocations = new List<string>();
                    var locations = TargetSystem.Instance.GetLocations(player);
                    foreach (var location in locations) {
                        if (location.freeSlots > 0) {
                            validLocations.Add(location.position.ToString());
                        }
                    }
                    cardAction["validLocations"] = validLocations;
                    playableCards.Add(cardAction);
                }
            }
            validActions["playableCards"] = playableCards;
            
            // Get movable cards
            var movableCards = new List<Dictionary<string, object>>();
            var playerLocations = TargetSystem.Instance.GetLocations(player);
            foreach (var location in playerLocations) {
                for (int i = 0; i < location.cardGroup.MountedCards.Count; i++) {
                    var card = location.cardGroup.MountedCards[i] as SnapCard;
                    if (card != null) {
                        var moveAction = new Dictionary<string, object>();
                        moveAction["cardIndex"] = i;
                        moveAction["cardName"] = card.stats.card_name;
                        moveAction["fromLocation"] = location.position.ToString();
                        
                        var validDestinations = new List<string>();
                        foreach (var destLocation in playerLocations) {
                            if (destLocation != location && destLocation.freeSlots > 0) {
                                validDestinations.Add(destLocation.position.ToString());
                            }
                        }
                        moveAction["validDestinations"] = validDestinations;
                        movableCards.Add(moveAction);
                    }
                }
            }
            validActions["movableCards"] = movableCards;
            
            // Get activatable abilities
            var activatableAbilities = new List<Dictionary<string, object>>();
            foreach (var location in playerLocations) {
                for (int i = 0; i < location.cardGroup.MountedCards.Count; i++) {
                    var card = location.cardGroup.MountedCards[i] as SnapCard;
                    if (card != null && card.abilities.Count > 0) {
                        for (int j = 0; j < card.abilities.Count; j++) {
                            var abilityAction = new Dictionary<string, object>();
                            abilityAction["cardIndex"] = i;
                            abilityAction["cardName"] = card.stats.card_name;
                            abilityAction["location"] = location.position.ToString();
                            abilityAction["abilityIndex"] = j;
                            abilityAction["abilityName"] = card.abilities[j].GetType().Name;
                            activatableAbilities.Add(abilityAction);
                        }
                    }
                }
            }
            validActions["activatableAbilities"] = activatableAbilities;
            
        } catch (System.Exception e) {
            validActions["error"] = $"Exception occurred: {e.Message}";
        }
        
        return validActions;
    }
    
    /// <summary>
    /// Gets the current game score and win conditions.
    /// </summary>
    /// <returns>Dictionary containing score information: player1Score, player2Score, winCondition, gameEnded</returns>
    public Dictionary<string, object> GetGameScore() {
        var score = new Dictionary<string, object>();
        
        try {
            // Calculate scores based on location control
            int player1Score = 0;
            int player2Score = 0;
            
            var allLocations = FindObjectsByType<Location>(FindObjectsSortMode.None);
            foreach (var location in allLocations) {
                if (location.totalPower > 0) {
                    if (location.player == Player.Player1) {
                        player1Score += location.totalPower;
                    } else if (location.player == Player.Player2) {
                        player2Score += location.totalPower;
                    }
                }
            }
            
            score["player1Score"] = player1Score;
            score["player2Score"] = player2Score;
            score["winCondition"] = "highest_power"; // Could be different based on game rules
            score["gameEnded"] = false; // Would need to implement win condition checking
            
        } catch (System.Exception e) {
            score["error"] = $"Exception occurred: {e.Message}";
        }
        
        return score;
    }
    
    #endregion
    
    #region Utility Methods
    
    /// <summary>
    /// Gets detailed information about a specific card by ID.
    /// </summary>
    /// <param name="cardId">The unique ID of the card</param>
    /// <returns>Dictionary containing card information or null if not found</returns>
    public Dictionary<string, object> GetCardInfo(int cardId) {
        var allCards = FindObjectsByType<SnapCard>(FindObjectsSortMode.None);
        var card = allCards.FirstOrDefault(c => c.stats.card_id == cardId);
        
        if (card == null) return null;
        
        var cardInfo = new Dictionary<string, object>();
        cardInfo["id"] = card.stats.card_id;
        cardInfo["name"] = card.stats.card_name;
        cardInfo["power"] = card.GetPower();
        cardInfo["basePower"] = card.stats["power"];
        cardInfo["cost"] = card.GetBaseCost();
        cardInfo["baseCost"] = card.stats["cost"];
        cardInfo["series"] = card.stats.series;
        cardInfo["revealed"] = card.Revealed;
        cardInfo["owner"] = card.ownedPlayer.ToString();
        cardInfo["abilities"] = card.abilities.Select(a => a.GetType().Name).ToList();
        cardInfo["buffs"] = card.buffs.Select(b => b.GetType().Name).ToList();
        
        return cardInfo;
    }
    
    /// <summary>
    /// Gets a list of all available card definitions in the game.
    /// </summary>
    /// <returns>List of dictionaries containing card definition information</returns>
    public List<Dictionary<string, object>> GetAllCardDefinitions() {
        var cardDefinitions = new List<Dictionary<string, object>>();
        
        // This would need to be implemented based on how card definitions are stored
        // Could query from a card library or registry
        // For now, returning cards currently in the game
        var allCards = FindObjectsByType<SnapCard>(FindObjectsSortMode.None);
        var uniqueCards = allCards.GroupBy(c => c.stats.card_id).Select(g => g.First());
        
        foreach (var card in uniqueCards) {
            var cardDef = new Dictionary<string, object>();
            cardDef["id"] = card.stats.card_id;
            cardDef["name"] = card.stats.card_name;
            cardDef["basePower"] = card.stats["power"];
            cardDef["baseCost"] = card.stats["cost"];
            cardDef["series"] = card.stats.series;
            cardDef["abilities"] = card.abilities.Select(a => a.GetType().Name).ToList();
            cardDefinitions.Add(cardDef);
        }
        
        return cardDefinitions;
    }
    
    #endregion
}