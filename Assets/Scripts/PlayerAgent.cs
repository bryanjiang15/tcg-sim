using UnityEngine;
 using Unity.MLAgents; 
 using Unity.MLAgents.Sensors; 
 using Unity.MLAgents.Actuators;
using CardHouse;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

public class PlayerAgent : Agent
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] private Player playerIndex = Player.Player1;
    void Start()
    {
        
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // Target and Agent positions
        sensor.AddObservation((CurrencyRegistry.Instance.GetCurrency("Energy", playerIndex==Player.Player1 ? 1 : 0) ?? 0f) / 6f);
        sensor.AddObservation(SnapPhaseManager.Instance.Turn / 6f);

        //Locations
        List<Location> playerLocations = FindObjectsByType<Location>(FindObjectsSortMode.None).Where(location => location.player == playerIndex).ToList();
        foreach (var location in playerLocations)
        {
            LoadLocationObservations(sensor, location);
            //Cards
            if (location.cardGroup.MountedCards.Count > 0)
            {
                foreach (var card in location.cardGroup.MountedCards)
                {
                    if (card is SnapCard snapCard)
                    {
                        LoadCardObservations(sensor, snapCard);
                        sensor.AddObservation(snapCard.Revealed ? 1f : 0f);
                    }
                }
            }
        }
    }

    void LoadLocationObservations(VectorSensor sensor, Location playerLocations)
    {
        //Location power
        sensor.AddObservation(playerLocations.totalPower / 50f);
        sensor.AddObservation(playerLocations.freeSlots / 4f);
        //Enemy Location
        Location enemyLocation = TargetSystem.Instance.GetEnemyLocation(playerLocations);
        sensor.AddObservation(enemyLocation.totalPower / 50f);
        sensor.AddObservation(enemyLocation.freeSlots / 4f);
        //Location card
        if (playerLocations.cardGroup.MountedCards.Count > 0)
        {
            foreach (var card in playerLocations.cardGroup.MountedCards)
            {
                if (card is SnapCard snapCard)
                {
                    sensor.AddObservation(snapCard.stats.power / 6f);
                    sensor.AddObservation(snapCard.stats.cost / 6f);
                }
            }
        }
    }

    void LoadCardObservations(VectorSensor sensor, SnapCard card)
    {
        //Card power
        sensor.AddObservation(card.GetPower() / 15f);
        //Original power
        sensor.AddObservation(card.stats.power / 15f);
        //Card cost
        sensor.AddObservation(card.stats.cost / 6f);
        sensor.AddObservation(card.GetBaseCost() / 6f);
        
    }

    public override void OnActionReceived(ActionBuffers actionBuffers)
    {
        //Action, size = 
        // 0: end turn
        if (actionBuffers.ContinuousActions[0] == 0f)
        {
            PhaseManager.Instance.NextPhase();
        }else{
            // action[1]: 0: play card, 1: move card, 2: activate...
            int CardIndex = (int)actionBuffers.ContinuousActions[2];
        }

        
    }

    void PlayCardAction(int cardIndex, LocationPosition locationIndex)
    {
        //Play card
        var locations = TargetSystem.Instance.GetLocations(playerIndex);
        Location playerLocations = locations.Where(location => location.position == locationIndex).ToList()[0];

        if (!playerLocations.isFull())
        {
            var hand = GroupRegistry.Instance.Get(GroupName.Hand, playerIndex == Player.Player1 ? 1 : 0);
            SnapCard card = hand.MountedCards[cardIndex] as SnapCard;
            if (CurrencyRegistry.Instance.GetCurrency("Energy", playerIndex == Player.Player1 ? 1 : 0) >= card.stats.cost)
            {
                hand.UnMount(cardIndex);
                playerLocations.cardGroup.Mount(card, 0);
            }

        }
    }

}
