class AIInterface:
    def __init__(self, controlled_player, game):
        self.controlled_player = controlled_player
        self.game = game

    def get_game_state(self):
        state = {
            "hand": [
                {
                    "name": card.name,
                    "cost": card.cost,
                    "power": card.power,
                    "abilities": [
                        {
                            "trigger": ability.trigger.name,
                            "effect": ability.effect.name,
                            "target": ability.target.name,
                            "amount": ability.amount,
                        }
                        for ability in card.abilities
                    ],
                }
                for card in self.controlled_player.hand
            ],
            "locations": [
                {
                    "name": location.name,
                    "cards": [
                        {
                            "power": card.power,
                            "abilities": [
                                {
                                    "trigger": ability.trigger.name,
                                    "effect": ability.effect.name,
                                    "target": ability.target.name,
                                    "amount": ability.amount,
                                }
                                for ability in card.abilities
                            ],
                        }
                        for card in location.player_powers.get(self.controlled_player, [])
                    ],
                }
                for location in self.game.locations
            ],
        }
        return state

    def apply_ai_input(self, action):
        if action["type"] == "end_turn":
            print(f"{self.controlled_player.name} ends their turn.")
            return True

        elif action["type"] == "play_card":
            card_index = action.get("card_index")
            location_index = action.get("location_index")

            if card_index is None or location_index is None:
                print("Invalid input: card_index and location_index are required.")
                return False

            if card_index < 0 or card_index >= len(self.controlled_player.hand):
                print("Invalid input: card_index out of range.")
                return False

            if location_index < 0 or location_index >= len(self.game.locations):
                print("Invalid input: location_index out of range.")
                return False

            card = self.controlled_player.hand[card_index]
            location = self.game.locations[location_index]

            self.controlled_player.play_card(card, location)
            return True

        else:
            print("Invalid input: unknown action type.")
            return False
