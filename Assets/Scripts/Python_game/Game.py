from Player import Player
from Location import Location
from Phase import PreparationPhase, RevealPhase

class Game:
    def __init__(self):
        self.players = [Player("Player 1"), Player("Player 2")]
        self.locations = [Location(f"Location {i+1}") for i in range(3)]
        self.current_phase = None
        self.turn = 1
        self.max_turns = 10

    def start_game(self):
        # Initial draw
        for player in self.players:
            player.draw_cards(3)

        while self.turn <= self.max_turns:
            print(f"Turn {self.turn} begins.")
            self.current_phase = PreparationPhase(self.players, self.locations)
            self.current_phase.execute()

            self.current_phase = RevealPhase(self.players, self.locations)
            self.current_phase.execute()

            self.turn += 1

        self.determine_winner()

    def determine_winner(self):
        scores = [sum(location.get_player_power(player) for location in self.locations) for player in self.players]
        if scores[0] > scores[1]:
            print("Player 1 wins!")
        elif scores[1] > scores[0]:
            print("Player 2 wins!")
        else:
            print("It's a tie!")