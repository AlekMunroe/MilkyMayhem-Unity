# Milky Mayhem

Milky Mayhem is a first-person parkour delivery game. The player controls a milkman who must move quickly through the environment, deliver milk and avoid hazards while trying to achieve a high score.

The project is currently in the grey box prototyping stage. Walking, camera movement, parkour jumping, sprint stamina, wall movement, sliding, airborne momentum, Legacy slide animations, a sprint UI and prototype pause management have been implemented. Delivery gameplay is not yet implemented.

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
| Slide | Left or Right Ctrl while moving on the ground |
| Pause or resume | Escape |

## Documentation

- [Programmer guide](Documentation/ProgrammerGuide.md)
- [Player setup](Documentation/PlayerSetup.md)
- [Project conventions](Documentation/ProjectConventions.md)
- [Known issues](Documentation/KnownIssues.md)
- [Development log](Documentation/DevelopmentLog.md)

## Current Priorities

1. Import a working and tested FBX version of Joe's greybox, including its required textures.
2. Create the greybox environment scene and update movement to work inside it.
3. Complete the remaining pause-system safety checks and scene-change handling.
4. Update `PlayerUIController` so it can safely locate the active player when needed.
5. Add milk-bottle throwing and placing, delivery zones, scoring, a round timer and hazards.
