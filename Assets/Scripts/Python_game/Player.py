from Card import Card
import random

class Player:
    def __init__(self, name):
        self.name = name
        self.deck = [Card(f"Card {i+1}", random.randint(1, 5), random.randint(1, 3)) for i in range(10)]
        self.hand = []
        self.played_cards = []

    def draw_cards(self, count):
        for _ in range(count):
            if self.deck:
                self.hand.append(self.deck.pop(0))

    def play_card(self, card, location):
        if card in self.hand:
            self.hand.remove(card)
            self.played_cards.append((card, location))
            print(f"{self.name} played {card.name} at {location.name}.")