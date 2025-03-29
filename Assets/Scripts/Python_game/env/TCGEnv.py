import numpy as np
from collections import OrderedDict
from rlcard.envs import Env
from rlcard.utils import seeding

from Game import Game
from AIInterface import AIInterface

class TCGEnv(Env):
    def __init__(self, config):
        self.name = "tcg"
        self.default_game_config = {"game_num_players": 2}
        self.game = Game()
        super().__init__(config)
        self.state_shape = [self._get_state_shape() for _ in range(self.num_players)]
        self.action_shape = [None for _ in range(self.num_players)]
        self.ai_interface = AIInterface(self.game.players[0], self.game)

    def _extract_state(self, state):
        game_state = self.ai_interface.get_game_state()
        obs = self._encode_state(game_state)
        legal_actions = self._get_legal_actions()
        extracted_state = {
            "obs": obs,
            "legal_actions": legal_actions,
            "raw_obs": game_state,
            "raw_legal_actions": list(legal_actions.keys()),
            "action_record": self.action_recorder,
        }
        return extracted_state

    def _encode_state(self, game_state):
        # Encode the game state into a numpy array for RL
        hand_size = len(game_state["hand"])
        location_count = len(game_state["locations"])
        obs = np.zeros((hand_size + location_count, 3), dtype=int)

        # Encode hand cards
        for i, card in enumerate(game_state["hand"]):
            obs[i, 0] = card["cost"]
            obs[i, 1] = card["power"]
            obs[i, 2] = len(card["abilities"])  # Number of abilities

        # Encode locations
        for i, location in enumerate(game_state["locations"]):
            obs[hand_size + i, 0] = len(location["cards"])  # Number of cards at location
            obs[hand_size + i, 1] = sum(card["power"] for card in location["cards"])  # Total power
            obs[hand_size + i, 2] = sum(len(card["abilities"]) for card in location["cards"])  # Total abilities

        return obs

    def _decode_action(self, action_id):
        # Decode the action ID into a meaningful action
        if action_id == 0:
            return {"type": "end_turn"}
        else:
            card_index = (action_id - 1) // len(self.game.locations)
            location_index = (action_id - 1) % len(self.game.locations)
            return {"type": "play_card", "card_index": card_index, "location_index": location_index}

    def _get_legal_actions(self):
        # Generate legal actions for the current state
        legal_actions = {"end_turn": 0}
        for i, card in enumerate(self.ai_interface.controlled_player.hand):
            for j, location in enumerate(self.game.locations):
                action_id = 1 + i * len(self.game.locations) + j
                legal_actions[action_id] = None
        return OrderedDict(legal_actions)

    def get_payoffs(self):
        # Get the payoffs for the players
        scores = [sum(location.get_player_power(player) for location in self.game.locations) for player in self.game.players]
        return np.array(scores)

    def _get_state_shape(self):
        # Define the shape of the state representation
        max_hand_size = 10  # Maximum number of cards in hand
        max_locations = 3  # Number of locations
        return [max_hand_size + max_locations, 3]

    def get_perfect_information(self):
        # Get the perfect information of the current state
        state = {
            "num_players": self.num_players,
            "hand_cards": [[card.name for card in player.hand] for player in self.game.players],
            "played_cards": [[card.name for card, _ in player.played_cards] for player in self.game.players],
            "locations": [
                {
                    "name": location.name,
                    "cards": [{"name": card.name, "power": card.power} for card in location.player_powers.get(player, [])],
                }
                for location in self.game.locations
                for player in self.game.players
            ],
            "current_player": self.game.players[0].name,
        }
        return state
