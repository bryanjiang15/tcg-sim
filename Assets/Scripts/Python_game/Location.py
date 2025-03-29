class Location:
    def __init__(self, name):
        self.name = name
        self.player_powers = {}

    def add_card(self, player, card):
        if player not in self.player_powers:
            self.player_powers[player] = 0
        self.player_powers[player] += card.power

    def get_player_power(self, player):
        return self.player_powers.get(player, 0)