# Milky Mayhem

Milky Mayhem is a first-person parkour delivery game. The player controls a milkman who must move quickly through the environment, deliver milk and avoid hazards while trying to achieve a high score.

The project is currently in the grey box prototyping stage. Walking, camera movement, parkour jumping, sprint stamina, wall movement, sliding, Legacy slide animations and a sprint UI have been implemented. Delivery gameplay and overall game-state management are not yet implemented.

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
4. Open `Assets/Scenes/_testing/MovementTest.unity` manually.
5. Enter Play mode and test the controls below.

`MovementTest` is not currently enabled in Build Settings. The disabled Build Settings entry still points to the removed `SampleScene`, so do not rely on **Build and Run** until the scene list is corrected.

## Current Controls

| Action | Input |
| --- | --- |
| Move | W A S D |
| Look | Mouse |
| Sprint | Left or Right Shift |
| Jump or wall jump | Space |
| Slide | Left or Right Ctrl while moving on the ground |

## Documentation

- [Programmer guide](Documentation/ProgrammerGuide.md)
- [Player setup](Documentation/PlayerSetup.md)
- [Project conventions](Documentation/ProjectConventions.md)
- [Known issues](Documentation/KnownIssues.md)
- [Development log](Documentation/DevelopmentLog.md)

## Current Priorities

1. Correct and convert the player setup into a reusable prefab.
2. Build a dedicated parkour testing area and balance movement values.
3. Create a `WorldController` for pausing, cursor state, camera locking and shared game state.
4. Add milk-bottle throwing and delivery zones.
5. Add scoring, a round timer and hazards.

