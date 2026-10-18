# Milky Mayhem

Milky Mayhem is a first-person parkour delivery game. The player controls a milkman who must move quickly through the environment, deliver milk and avoid hazards while trying to achieve a high score.

The project is in the grey box prototyping stage. The player now uses a Rigidbody based `PlayerController` rather than the old CharacterController based one. With walking jumping, sprinting, airborne movement and prototype wall jumping. The previous CharacterController player has been depreciated. The sliding code is available but deliverately disabled.

A complete prototype of the delivery loop is now implemented. The player can throw milk with the left mouse button or place milk with the right mouse button. Milk crates bouce and break from large impacts and show temporary visual and audio to the player. Successful checkpoints now show their score and the delivery progress is shared to the `GameManager`. The round will finish by completing every delivery or running out of time. The results screen will now show the completion time, delvieries complete, bottles broken and the player's final score.

The Rigidbody wall movement has not been updated and tested. The player will now detach from the wall when the player moves away. The momentum moving along the wall will gradually reduce and any momentum that is left is stored and used in the next wall jump. Ground and wall detection now uses angles over the seperate Ground and Wall layers. Surfaces the player will walk or climb on must use the new `Environment` layer.

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
4. Open `Assets/Scenes/_testing/DevMenu.unity` to test scene loading, or open `Assets/Scenes/MainGame.unity` to test the prototype gameplay loop. Use `Assets/Scenes/_testing/MovmenetTest.unity' to work directly on movement.
5. Enter Play mode and test the controls below.

`DevMenu`, `MainGame` and `MovementTest` are included in Build Settings. `DevMenu` is first and therefore opens when a development build starts. Its buttons load the available scenes.

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

1. Test the complete gameplay loop and fix any bugs which stop a round from being completed.
2. Stop the pause controls from resuming the gameplay after the round has finished.
3. Update the Rigidbody's values for movement in the grey box.
4. Test the `MainGame` scene in multiple scenarios.
5. Create a working development build for the game demonstration.
6. Continue organising the imported models and UI.
7. Replace the temporary UI, models and audio with the final assets.
8. Add hazards and other optional gameplay features.