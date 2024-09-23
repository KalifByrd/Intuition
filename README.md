# Intuition

## Overview

**Intuition** is a competitive two-player card game developed in Unity. This project was created over one semester as a senior project, focusing on networking and multiplayer aspects in Unity. The goal of the game is to utilize strategic inference skills and a bit of luck to outwit your opponent by predicting their moves and countering them with your own.

## Game Concept

Intuition is designed to test your ability to foresee and counter your opponent’s moves. Players take turns drawing cards from two distinct decks: the Sight Deck and the Fate Deck. By predicting your opponent's plays and responding with the appropriate counters, the objective is to be the first to collect a winning set of four cards, each from a different category. The game is simple in its mechanics but deep in its strategic potential.

## Gameplay Rules

### Card Categories and Symbols
- **Heart (❤)**: Represents emotion and feeling.
- **Moon (☾)**: Represents soft and timid traits.
- **Star (★)**: Represents sharpness and fierceness.
- **Fate (∞)**: A powerful and rare card that can trump all others under specific conditions.

### Card Mechanics
- **Heart (❤)** beats Moon (☾)
- **Moon (☾)** beats Star (★)
- **Star (★)** beats Heart (❤)
- **Fate (∞)** beats Heart (❤) and Moon (☾), but is beaten by Star (★)

### Gameplay Flow
1. Players start with two decks: the Sight Deck and the Fate Deck.
2. Draw three cards from the Sight Deck to form your hand.
3. Each turn, players choose one card from their hand and place it face down.
4. Both players reveal their chosen cards simultaneously, and the winner is determined based on the rules above.
5. Winning cards are displayed, while losing cards are discarded and replaced from the Sight Deck.
6. Players draw a Fate Card if they win three rounds with the same card symbol.
7. The first player to collect a set of four winning cards, one from each category (❤, ☾, ★, ∞), wins the game.

### Winning Conditions
- **Winning Set**: Achieve a set of four cards, each from a different symbol category (❤, ☾, ★, ∞).

## Project Objectives
This project was primarily focused on exploring and implementing multiplayer networking in Unity. Key objectives included:
- **Researching and implementing networking solutions** for synchronous multiplayer gameplay.
- **Designing and developing a rule-based card game system** that could support multiplayer interactions.
- **Creating an intuitive user interface** and a fluid game experience that supports strategic gameplay.

## Features
- **Multiplayer Networking**: Play against other players online, testing your strategic thinking in real-time.
- **Dynamic Card Mechanics**: A deep and engaging card mechanic system with four distinct categories, each with its own strengths and weaknesses.
- **Strategic Gameplay**: Predict your opponent’s next move and counter it with the right card to outmaneuver them.

## Download and Play

You can download and play **Intuition** by visiting the [official game demo page](https://toxicteddie.com/Demos/Intuition/).

### How to Play
1. Download the game from the link above and run it on your system.
2. **Press "Join with Code"**:
   - If you **have a code**, enter it and join your friend's game.
   - If you **don't have a code**, press "Create" to start a new game and receive a code.
3. **Share the code** with your friend so they can join your game.
4. Begin the game and enjoy predicting and outsmarting your opponent!

## Known Bug
- **Screen Resizing Issue**: If you resize the game window while animations are playing, the game may break. It is recommended to avoid resizing the window during gameplay.

## Development Insights
This project served as an excellent learning opportunity to delve into the complexities of Unity’s networking capabilities. It challenged us to think critically about how to synchronize game state across multiple players, manage real-time interactions, and provide a seamless user experience. The research into multiplayer frameworks and protocols was a core component of this development process.

## Future Enhancements
- **Expanded Multiplayer Options**: More player slots and enhanced matchmaking.
- **AI Opponent**: Implement a challenging AI for solo play.
- **Card Customization**: Allow players to customize their decks with unique cards and abilities.

## Credits
- **Kalif Byrd** - Game Design, Networking Implementation, and Development.

## License
This project is licensed under the **Modified BSD 3-Clause License (Non-Commercial)**. See the [LICENSE](LICENSE) file for more details.

## Contact
For any questions or suggestions, please feel free to contact me through [GitHub Issues](https://github.com/KalifByrd/Intuition/issues).
