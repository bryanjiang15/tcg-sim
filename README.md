# TCG Simulator

A powerful Unity-based Trading Card Game (TCG) simulator that allows you to create, design, and play custom card games with AI-powered card generation capabilities.

## 🎮 Features

### Core Game Systems
- **Card Management System**: Complete card creation, editing, and management with customizable stats (power, cost, abilities)
- **Deck Building**: Intuitive deck builder with card library management
- **Game Engine**: Full-featured TCG engine with phases, turns, and rule systems
- **Targeting System**: Advanced targeting mechanics for card abilities and effects
- **Energy System**: Resource management for card costs and gameplay balance

### AI Integration
- **AI-Powered Card Generation**: Use OpenAI API to automatically generate card designs and abilities
- **AI Game Interface**: Python-based AI interface for automated gameplay and testing
- **Smart Card Balancing**: AI-assisted card balancing and power level assessment

### Gameplay Features
- **Location-Based Combat**: Strategic gameplay with multiple location zones
- **Ability System**: Complex card abilities with triggers, effects, and targets
- **Buff System**: Dynamic buff/debuff mechanics that affect card performance
- **Phase Management**: Turn-based gameplay with multiple phases (draw, play, combat, end)

### User Interface
- **Modern UI**: Clean, intuitive interface built with Unity UI Toolkit
- **Card Library**: Browse and manage your card collection
- **Training Mode**: Practice and test your decks against AI opponents
- **Card Creation Tool**: Visual card creation interface

## 🖼️ Screenshots

### Main Game Interface
![Main Game Interface](docs/images/game-interface.png)
*The main TCG gameplay interface showing cards, locations, and game state*

### Card Creation Tool
![Card Creation](docs/images/card-creation.png)
*AI-powered card creation interface for designing new cards*

### Deck Builder
![Deck Builder](docs/images/deck-builder.png)
*Intuitive deck building interface with card library integration*

### Card Library
![Card Library](docs/images/card-library.png)
*Comprehensive card library for managing your collection*

## 🛠️ Tech Stack

- **Unity 2022.3+**: Core game engine and rendering
- **C#**: Primary programming language for game logic
- **Python**: AI integration and automated gameplay
- **OpenAI API**: AI-powered card generation
- **Unity UI Toolkit**: Modern user interface components

## 📁 Project Structure

```
Assets/
├── Scripts/
│   ├── Systems/           # Core game systems
│   │   ├── GameSystem.cs
│   │   ├── TargetSystem.cs
│   │   ├── EnergySystem.cs
│   │   ├── ActionSystem.cs
│   │   └── RuleSystem.cs
│   ├── Cards/            # Card-related scripts
│   │   ├── SnapCard.cs
│   │   ├── LocationCard.cs
│   │   └── Abilities/
│   ├── Phases/           # Game phase management
│   ├── Events/           # Event system
│   ├── UI/              # User interface scripts
│   └── Python_game/     # AI integration
├── Scenes/
│   └── TCG/             # Game scenes
│       ├── Tcg.unity    # Main game scene
│       ├── CardCreation.unity
│       ├── CardLibrary.unity
│       └── Training.unity
└── UI/                  # UI assets and components
```

## 🚀 Getting Started

### Prerequisites
- Unity 2022.3 LTS or later
- Python 3.8+ (for AI features)
- OpenAI API key (for card generation)

### Installation
1. Clone this repository
2. Open the project in Unity
3. Install required Python packages: `pip install openai`
4. Configure your OpenAI API key in the settings
5. Open the main TCG scene and start playing!

### Basic Usage
1. **Create Cards**: Use the Card Creation tool to design new cards with AI assistance
2. **Build Decks**: Assemble your card collection into powerful decks
3. **Play Games**: Challenge AI opponents or other players in the main game
4. **Train**: Practice your strategies in Training mode

## 🎯 Key Systems

### Card System
Cards are the heart of the game, featuring:
- Customizable stats (power, cost, etc.)
- Complex ability systems
- Buff/debuff mechanics
- Location-based gameplay

### AI Integration
The Python AI interface provides:
- Automated gameplay testing
- Card generation assistance
- Game state analysis
- Strategic decision making

### Targeting System
Advanced targeting mechanics for:
- Card abilities
- Effect applications
- Strategic positioning
- Multi-target effects

## 🤝 Contributing

We welcome contributions! Please feel free to submit pull requests or open issues for bugs and feature requests.

### Development Guidelines
- Follow Unity coding standards
- Add comments for complex game logic
- Test AI features thoroughly
- Update documentation for new features

## 📄 License

This project is licensed under the MIT License - see the LICENSE file for details.

## 🙏 Acknowledgments

- Unity Technologies for the game engine
- OpenAI for AI integration capabilities
- The TCG community for inspiration and feedback

---

**Ready to create your own trading card game? Start building with TCG Simulator today!** 🃏
