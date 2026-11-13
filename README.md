# Underpin Slot Machine

A classic-style slot machine game developed using Unity 6.6 and C#. The project recreates the experience of a traditional slot machine with interactive reels, a betting system, credit management, symbol-based payouts, and a special bonus feature.

## Game Overview

The objective is to match three identical symbols across the reels to win credits. Players can select their bet, activate the machine using the lever, and wait for the reels to stop.

The game combines a retro-inspired visual presentation with smooth reel animations and interactive gameplay systems.

## Features

- Three independently animated spinning reels.
- Randomized symbol selection for every spin.
- Smooth reel movement with staggered stopping.
- Three betting options: 10, 50, and 100 credits.
- Credit balance management.
- Automatic bet deduction when a spin starts.
- Win detection based on matching symbols.
- Different payout multipliers for each regular symbol.
- Interactive lever with idle and pulled states.
- Audio feedback for buttons, spinning, reel stops, and winning.
- Automatic win popup displaying the reward.
- Golden Star bonus feature.

## Payout System

| Symbol | Payout Multiplier |
|---|---:|
| Seven | 10x |
| Cherries | 5x |
| Bell | 3x |
| BAR | 2x |
| Golden Star | 5x bonus |

Matching three Golden Star symbols awards five times the current bet.

## How to Play

1. Launch the game.
2. Select one of the available betting options: 10, 50, or 100 credits.
3. Pull the lever to start the machine.
4. Wait for all three reels to stop.
5. Match three identical symbols to receive a payout.
6. Try to match three Golden Stars to activate the bonus reward.

### Controls

| Action | Input |
|---|---|
| Select Bet | Click a betting option |
| Start Spin | Click the slot machine lever |
| View Reward | Win popup appears automatically |

## Running the WebGL Build

The WebGL build is included in the repository:

`Build/WebGL Build/`

To run the game:

1. Clone or download this repository.
2. Navigate to the `Build/WebGL Build/` directory.
3. Serve the directory using a local web server or compatible web hosting service.
4. Open `index.html` through the server in a supported browser.

Note: Opening `index.html` directly from the file system may not work because browsers restrict certain local file operations required by Unity WebGL.

## Development Approach

The project was developed using a modular approach, with gameplay responsibilities separated into individual systems.

### GameManager
Coordinates the overall gameplay flow, initiates spins, determines reel results, checks winning combinations, calculates payouts, and manages win feedback.

### ReelController
Controls individual reel behaviour, including symbol movement, spinning animations, and stopping at the selected result.

### SlotEconomy
Manages the player's credit balance, betting options, bet deductions, and winnings.

### SlotLeverController
Handles lever interaction and switches between its idle and pulled visual states to initiate gameplay.

### AudioManager
Manages sound effects for gameplay interactions, reel movement, reel stopping, and winning events.

The reel animations use coroutine-based movement and easing to create smooth spinning and stopping behaviour. Random symbol selection determines the outcome of each spin.

Separating these responsibilities makes the code easier to maintain, debug, and extend.

## Technology

- Game Engine: Unity 6.6
- Programming Language: C#
- UI: Unity UI and TextMesh Pro
- Audio: Unity Audio System
- Target Platform: WebGL

## Project Structure

```text
UnderpinSlotMachine/
|
|-- Assets/
|   |-- Art/
|   |-- Audio/
|   |-- Prefabs/
|   |-- Scenes/
|   |-- Scripts/
|   |-- Settings/
|   |-- TextMesh Pro/
|
|-- Build/
|   |-- WebGL Build/
|       |-- Build/
|       |-- TemplateData/
|       |-- index.html
|
|-- Data/
|-- Packages/
|-- ProjectSettings/
|
|-- .gitignore
|-- .gitattributes
|-- README.md
```

## Author

Developed by Tanmoy Deka as part of the Unity Game Developer assignment for Underpin Technology Services.

**Project:** Underpin Slot Machine  
**Engine:** Unity 6.6  
**Platform:** WebGL
