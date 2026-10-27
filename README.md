# Milky Mayhem

Milky Mayhem is a first-person parkour delivery game. The player controls a milkman who must move quickly through the environment, deliver milk and avoid hazards while trying to achieve a high score.

The project is in the vertical slice development stage. The player uses a Rigidbody based `PlayerController rather than the old CharacterController based one. The player has ground modement, jumping, airborne movement, wall movement and sliding. Sprinting is now depreciated.

A complete prototype of the delivery loop is now implemented. The player can throw milk with the left mouse button or place milk with the right mouse button. Milk crates bouce and are removed after their lifetime expires in `MilkCrate`. Checkpoints aff score when milk is successfully delivered and a round finishes once every delivery is completed or the timer ends.

The project now includes Main menu, options, scores and credits. It has updated UI to match the theme of the game. 

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

Sprinting is being depreciated.

## Documentation

- [Programmer guide](Documentation/ProgrammerGuide.md)
- [Player setup](Documentation/PlayerSetup.md)
- [Project conventions](Documentation/ProjectConventions.md)
- [Known issues](Documentation/KnownIssues.md)
- [Development log](Documentation/DevelopmentLog.md)

## Current Priorities

1. Decide how the crates should be removed.
2. Add a round start hub inside the incomplete Milk Store and gameplay will start once exited.
3. Create multiple checkpoint routes and randomly choose one when the round starts.
4. Reset the round state correctly when starting a new round.
5. Replace the 3D Checkpoint Arrow with a new UI compass. Include delivery search areas.
6. Test repeating rounds, saved scores and route randomisation.
7. Fix any final bugs relating to the player.
8. Finalise the UI design.