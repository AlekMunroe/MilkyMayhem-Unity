# Milky Mayhem

Milky Mayhem is a first-person parkour delivery game. The player controls a milkman who must move quickly through the environment, deliver milk and avoid hazards while trying to achieve a high score.

The project is in the vertical slice development stage. The player uses a Rigidbody based `PlayerController`. The player has ground movement, jumping, airborne movement, wall movement, sliding and automatic mounting on small objects. Sprinting is now removed. The current development build is `v0.1.4`

A complete prototype of the delivery loop is now implemented. The player starts inside the milk store and the round begins when the player leaves. The player can throw milk with the left mouse button or place milk with the right mouse button. Milk crates bounce and are removed after their lifetime expires. Delivered crates do not expire. A testest checkpoint route is randomly chosen when the round starts. Deliveries add score and the round finishes when every delivery is completed or the timer ends.

The project includes a main menu, options menu, saved scores, gameplay UI, a pause menu and round completion screen. The gameplay UI has a compass which points to a general delivery area and spins when the player is inside the checkpoint search area.

## Requirements

- Unity Editor `6000.3.19f1`
- Universal Render Pipeline
- Unity Input System
- Git

Open the project using the exact Unity version above. Opening and saving it in a different editor version can change project and scene files for the rest of the group.

## Getting Started

1. Clone the repository.
2. Open the project folder through Unity Hub using Unity `6000.3.19f1`.
3. Allow Unity to restore the packages listed in `Packages/manifest.json`.
4. Open `Assets/Scenes/MainMenu.unity` to test the full main menu or open `Assets/Scenes/MainGame.unity` to test the current gameplay loop.
5. Enter Play Mode and test the controls.

`MainMenu` is the first scene in the build settings. `MainGame`, `DevMenu` and `MovementTest` are also included for the gameplay and dev testing.

Throwing, placing, jumping and sliding is disabled while the player is inside the milk store.

## Current Controls

| Action | Input |
| --- | --- |
| Move | W A S D |
| Look | Mouse |
| Jump or wall jump | Space |
| Throw milk | Left Mouse Button |
| Place milk without bouncing | Right Mouse Button |
| Slide | Left or Right Ctrl while moving on the ground |
| Pause or resume | Escape |

Sprinting is now removed from the `PlayerController`.

## Documentation

- [Programmer guide](Documentation/ProgrammerGuide.md)
- [Player setup](Documentation/PlayerSetup.md)
- [Project conventions](Documentation/ProjectConventions.md)
- [Known issues](Documentation/KnownIssues.md)
- [Development log](Documentation/DevelopmentLog.md)

## Current Priorities

1. Add custom key binds and add them to the settings menu
1. Create the player ability system
1. Fix the performance issues
1. Decide if the round time should change depending on the chosen route
1. Continue replacing temporary assets
1. Create final placements on checkpoint routes
1. Improve and test the wall running movement
1. Restore the broken milk system
1. Add LOD models