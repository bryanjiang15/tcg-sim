class PreparationPhase:
    def __init__(self, players, locations):
        self.players = players
        self.locations = locations

    def execute(self):
        for player in self.players:
            print(f"{player.name}'s turn to play cards.")
            while True:
                if not player.hand:
                    print("No cards left to play.")
                    break
                # Simulate playing a card
                card = player.hand[0]
                location = self.locations[0]
                player.play_card(card, location)
                break  # End turn after one card for simplicity

class RevealPhase:
    def __init__(self, players, locations):
        self.players = players
        self.locations = locations

    def execute(self):
        for player in self.players:
            print(f"Revealing {player.name}'s cards.")
            for card, location in player.played_cards:
                location.add_card(player, card)
                card.activate_abilities("OnReveal", {"location": location, "player": player})
            player.played_cards.clear()