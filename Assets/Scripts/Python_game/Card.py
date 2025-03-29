class Card:
    def __init__(self, name, power, cost):
        self.name = name
        self.power = power
        self.cost = cost
        self.abilities = []

    def add_ability(self, ability):
        self.abilities.append(ability)

    def activate_abilities(self, trigger, game_context):
        for ability in self.abilities:
            if ability.trigger == trigger:
                ability.activate(game_context)