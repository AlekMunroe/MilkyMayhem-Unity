# Milky Mayhem

Milky Mayhem is a first-person parkour delivery game. The player controls a milkman who must move quickly through the environment, deliver milk and avoid hazards while trying to achieve a high score.

The project is in the grey box prototyping stage. The player now uses a Rigidbody based `PlayerController` rather than the old CharacterController based one. With walking jumping, sprinting, airborne movement and prototype wall jumping. The previous CharacterController player has been depreciated. The sliding code is available but deliverately disabled.

A prototype of the delivery loop is now implemented. The player can now throw milk with the left mouse button or place milk with the right mouse button. Milk crates can bounce and break from a large impact. The successful checkports report its score and remaining delivery progress to the `GameManager`. The final round completion is currently incomplete yet it only writes a console message.

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
4. Open `Assets/Scenes/_testing/DevMenu.unity` to test scene loading, or open `Assets/Scenes/_testing/MovementTest.unity` to work directly on movement.
5. Enter Play mode and test the controls below.

`DevMenu` and `MovementTest` are both included in Build Settings. `DevMenu` is first and therefore opens when a development build starts. Its buttons load the available development scenes.

## Current Controls

| Action | Input |
| --- | --- |
| Move | W A S D |
| Look | Mouse |
| Sprint | Left or Right Shift |
| Jump or wall jump | Space |
| Throw milk | Left Mouse Button |
| Place milk without bouncing | Right Mouse Button |
| Slide | Left or Right Ctrl while moving on the ground (currently disabled) |
| Pause or resume | Escape |

## Documentation

- [Programmer guide](Documentation/ProgrammerGuide.md)
- [Player setup](Documentation/PlayerSetup.md)
- [Project conventions](Documentation/ProjectConventions.md)
- [Known issues](Documentation/KnownIssues.md)
- [Development log](Documentation/DevelopmentLog.md)

## Current Priorities

1. Finish testing and balancing the Rigidbody player, wall movement and milk physics.
2. Implement final game-complete behaviour when all checkpoints are cleared.
3. Add the round timer and player-facing score, delivery-progress and completion UI.
4. Remove duplicated projectile-lifetime ownership and decide how long delivered crates remain visible.
5. Complete the remaining pause-system safety checks and scene-change handling.
6. Test multiple checkpoints and the full fail, retry and success flow.
7. Continue organising imported models, prefabs, materials and deprecated assets.
8. Add the greybox environment, final delivery content and hazards.

1. Finish testing and refining the Rigidbody player along with wall movement and milk physics.
2. Implement a game completion behaviour system when all the checkpoints are cleared.
3. Add the round timer and player facing score to show delivery progress.
4. Coplete the remaining pause system safety checks and scene change handling.
5. Test multiple checkpoints for any errors.
6. Add the greybox environment and start working on a final concept showing the correct world.
7. Add final delivery content and hazards.