# Milky Mayhem Programmer Guide

## Purpose

This guide explains what each current script does and how the main systems connect together. Read [PlayerSetup.md](PlayerSetup.md) before changing or rebuilding the player prefab.

## Current Architecture

```text
Keyboard and mouse
        |
        +--> PlayerController ------> Rigidbody movement and parkour
        |
        +--> CameraController ------> Player yaw and camera pitch
        |
        +--> MilkThrowing ----------> Thrown or placed MilkCrate
                                           |
                                           v
                                    MilkCheckpoint
                                           |
                                           v
                                      GameManager
                         Checkpoints, score, timer and round state
                              |             |              |
                              v             v              v
                       CheckpointArrow  PlayerUIController  RoundCompleted
                                                          |       |     |
                                                          v       v     v
                                                   PlayerController   ScoreSaveSystem
                                                   CameraController   |
                                                                      v
                                                                    PlayerPrefs JSON
                                                                      |
                                                                      v
                                                                    ScoresMenuController

WorldController ------------> Pause state, time scale and cursor
        |
        +--------------------> PlayerController
        +--------------------> CameraController
        +--------------------> Pause menu
```

The scripts currently read `Keyboard.current` and `Mouse.current` directly. We do not have an Input Actions asset or input rebinding system yet.

## PlayerController

**Path:** `Assets/Scripts/Player/PlayerController.cs`

**Responsibility:** Controls the first person Rigidbody player and its parkour movement.

Current features:

- Rigidbody movement during the physics update
- Ground acceleration, deceleration and a maximum speed
- Air acceleration, air drag and retained momentum
- A forward parkour jump boost
- Coyote time, jump buffering and variable jump height
- Wall detection, temporary wall clinging, wall sliding and wall jumping
- Pause and round completion integration
- Momentum based sliding
- Permanent controls which can disable incomplete features

Sprinting is in the script but is disabled on the player prefab. Sprinting is being depreciated. Sliding has been reimplemented and `PermCanSlide` is enabled. The slide uses the player's current Rigidbody velocity.

Ground, wall and air movement use `UpdateGroundMovement`, `UpdateWallMovement` and `UpdateAirMovement`. Air direction changes are limited by `MaxAirSpeed`.

Important public parts:

```csharp
public static PlayerController Instance
public float SprintStaminaPercent
public void UpdatePause(bool isPaused)
public CameraController GetCameraController()
```

The script requires:

- A `Rigidbody` and `CapsuleCollider` on the same GameObject
- The frictionless player Physics Material on the CapsuleCollider
- The `Envionment` layer assigned to the environment mask
- Working ground and wall angle values
- A `CameraController` on the player
- An enabled `GameManager` when the round complete event is used

The old CharacterController version is depreciated. New scripts must use the current Rigidbody `PlayerController` and should not require a `CharacterController`.

Sliding code is still inside this script but `Perm Can Slide` is disabled on the prefab. Sliding is postponed because it is unreliable and not currently required.

Ground and wall detection now uses the `Environment` layer and checks angles of surfaces. When wall clinging, the `PlayerController` keeps movement along the wall and gradually refuxes it using `wallMomentumHalfLife`. When moving away from the wall, the wall detatches from the player and `PerformWallJump` starts with the player's remaining velocity before adding wall jump forces. The prototype wall cling boost as been added back. `WallClingClimb` still remains however, it is unused because it caused the player to stick to the top of a wall.

`PlayerController` listens to `GameManager.RoundCompleted` and when the event is raised, it stops the player's velocity and disables any movement.

## CameraController

**Path:** `Assets/Scripts/Player/CameraController.cs`

**Responsibility:** Rotates the player horizontally and the camera vertically.

`Cam Object` should use `CamPos` and `Main Camera` placed under it. `WorldController` disables it while the game is paused. `CameraController` listens to the `GameManager.RoundCompleted` state and disables itself when the round completes. The `WorldController` controls the cursor rather than the `CameraController`.

## PlayerUIController

**Path:** `Assets/Scripts/UI/Gameplay/PlayerUIController.cs`

**Responsibility:** Updates the gameplay and round results UI.

It reads `PlayerController.SprintStaminaPercent` and uses it to update the sprint slider. It listens for `CheckpointProgressChanged` and `TimeRemainingChange` to show the completed deliveries, remaining deliveries and time remaining.

It also listens to `RoundCompleted` and shows if the round was completed or the timer ran out.

## WorldController

**Path:** `Assets/Scripts/World/WorldController.cs`

**Responsibility:** Controls worldwide states such as pausing the game.

It currently:

- Provides `WorldController.Instance`
- Detects the Escape key and pauses or resumes the game
- Stores the pause state in `isGamePaused`
- Sets `Time.timeScale` to `0` while paused and back to normal when resumed
- Locks and hides the cursor during gameplay
- Unlocks and shows the cursor while paused
- Tells `PlayerController` when to stop or resume
- Disables or enables `CameraController`
- Shows or hides the pause menu

Any new system which reads player input must also check the pause state. `MilkThrowing` already blocks its input while paused, while the cursor is unlocked or while UI is being used.

## MilkThrowing

**Path:** `Assets/Scripts/Throwable/MilkThrowing.cs`

**Responsibility:** Creates thrown or placed milk crates from the player's camera direction.

The component is under `Main Camera` on the player's `ThrowOrigin` object.

- Left mouse throws a crate.
- Right mouse places a crate.
- A thrown crate can inherit some of the player's Rigidbody velocity.
- Thrown crates use an impulse and random torque.
- Placed crates have their controlled bouncing disabled.
- New crates ignore collisions with the player and other spawned crates.
- Throwing is blocked while paused or while UI is being used.

The throw and placement settings currently work for testing but still need balancing in the final world.

## MilkCrate

**Path:** `Assets/Scripts/Throwable/MilkCrate.cs`

**Responsibility:** Controls the physics and delivery state of each milk crate.

It currently includes:

- A limited number of controlled bounces
- A bounce cooldown
- Reflected velocity when it hits a valid surface
- Impact speed checks which can break the milk
- A maximum lifetime
- Separate broken and landed states
- A function which disables bouncing for placed crates
- A function which allows a checkpoint to stop and position the crate
- A seperate idle lifetime

`MilkCrate` is now the only owner if projectile lifetime. `MilkThrowing` no longer destroys the projectiles after a seperate delay.

When the lifetime or idle timer finishes, milk crates are marked as broken. This tells the `GameManager` it is destroyed and they will be destroyed after `breakDelay`. Successful delivery crates still become destroyed and this logic must be fixed for its removal behaviour.

## MilkCheckpoint

**Path:** `Assets/Scripts/Throwable/MilkCheckpoint.cs`

**Responsibility:** Registers a milk delivery and shows success or failure feedback.

It currently:

- Finds a `MilkCrate` from its collider or parent object
- Stops and moves successful milk onto the landing point
- Replaces failure feedback with success feedback
- Plays success or failure audio
- Prevents the checkpoint from completing more than once
- Sends the successful delivery and score amount to `GameManager`

The checkpoint Collider must have `Is Trigger` enabled. The success visual, failed visual, AudioSource, audio clips and landing point should all be assigned on its prefab.

## GameManager

**Path:** `Assets/Scripts/World/GameManager.cs`

**Responsibility:** Tracks checkpoint progress, score, broken bottles, round timer and round completion for the current round.

Important public parts:

```csharp
public static GameManager Instance
public int Score
public int TotalCheckpoints
public int RemainingCheckpoints
public IReadOnlyList<MilkCheckpoint> Checkpoints
public float TimeRemaining
public float CompletionTime
public int BottlesBroken
public event Action<int> ScoreChanged
public event Action<int, int> CheckpointProgressChanged
public event Action<float> TimeRemainingChanged
public event Action RoundCompleted
public bool RegisterCheckpointCleared(MilkCheckpoint checkpoint, int scoreAmount)
public void CountBrokenBottles()
```

When the scene starts, the `GameManager` checks every checkpoint assigned and starts the round timer. Each checkpoint can only give one successful delivery.

The round will finish when every checkpoint is complete or the time left reaches zero. `RoundCompleted` tells the player and camera that the game is complete. The `GameManager` then enables the `RoundCompleteUI` and tells the `WorldController` to enable the mouse.

`Checkpoints` are exposed as an `IReadOnlyList` so the `checkpointArrow` can read the checkpoints without editing the list.

## ScoreSaveSystem

**Path:** `Assets/Scripts/SavingSystem/ScoreSaveSystem.cs`

**Responsibility** Save the result when `GameManager.RoundCompleted` is called.

It saves the date, score, amount of completed deliveries, total deliveries, broken bottles, completion time and round number into PLayerPref with JSON using the `SavedRoundResults` key.

Results are stored by their total score and the stored list can have a maximum of 50 games. Route number is currently assigned in the inspecor and will only stay as `1` until the route selection system is created.

## ScoresMenuController

**Path:** `Assets/Scripts/SavingSystem/ScoresMenuController.cs`

**Responsibility:** Shows the saved results in the scores panel of the Main Menu.

The controller refreshes when the scores panel is active. It creates a score row prefab under a Vertical Layour Group and shows the highest results. It shows a no scores message when no scores are saved.

Each score row prefab requires these child objects:

- `DateText`
- `ScoreText`
- `DeliveriesText`
- `BrokenText`
- `TimeText`
- `RouteText`

## GameSettingsController

**Path:** `Assets/Scripts/MainMenu/GameSetingsController.cs`

**Responsibility:** Applys, cancel, resore and save the game settings

Displays all settings and saves the values as playerprefs.

Some values are not connected to the gameplay systems yet.

Audio uses the exposed mixer parameters:

- `MasterVolume`
- `MusicVolume`
- `SoundEffectsVolume`

## MainMenuController

**Path:** `Assets/Scripts/MainMenu/MainMenuController.cs`

**Responsibility:** Opens the main menu panels and starts the game.

It also resets `Time.timeScale` in the start function for when the player returns from the game, the paused state does not carry over.

## CheckpointArrow
**Path:** `Assets/Scripts/Player/CheckpointArrow.cs`

**Responsibility:** It points to the closest available checkpoint.

The script reads `GameManager.Checkpoints` and ignores the completed checkpoints. It rotates the assigned arrow object to the closest available checkpoint and hides when no checkpoints are available.

## RestartTrigger

**Path:** `Assets/Scripts/World/RestartTrigger.cs`

**Responsibility:** Restarting the current scene when the player enters the trigger.

It shows a five second countdown using TMP text and reloads the current scene. The countdown uses unscaled time so it can continue to work if the normal game has stopped.

This is currently a prototype safety system.

## RoundCompleteMenu

**Path:** `Assets/Scripts/UI/Menus/RoundCompletionMenu.cs`

**Responsibility:** Controls the buttons on the round coplete UI

It can reload the current scene or load the main menu. The main menu index must match the Build Settings.

### Round Completion and Resetting

The current round completion system stops the timer and calls `RoundCompleted`. It stops the player and camera then opens the results UI.

Restarting currently reloads `MainGame`. A dedicated reset system would work much better in the future.

## PauseMenuController

**Path:** `Assets/Scripts/UI/Menus/PauseMenuController.cs`

**Responsibility:** Controls the pause menu buttons.

It resumes the game through `WorldController`, shows or hides the return confirmation and loads the selected menu scene. It returns to the main menu scene. The `MainMenuController` and `WorldController` reset the game's speed when the scenes start.

## Development Tools

### Development Main Menu

**Path:** `Assets/Scripts/_Dev/Dev_MainMenu.cs`

This is the temporary menu used to open development scenes. It is not the final main menu.

### Input Visualiser

**Path:** `Assets/Scripts/_Dev/Dev_ShowKeysOnScreen.cs`

**Prefab:** `Assets/Prefabs/_Dev/Dev_InputVisualiser.prefab`

This shows keyboard and mouse button inputs while testing or recording development footage. It is only a development tool and should not be part of the final UI.

## Depreciated Systems

These files are kept for reference but are not part of the current systems:

- `Assets/Prefabs/_Depreciated/Player.prefab`: the old CharacterController player
- `Assets/Scripts/Player/_Depreciated/Depreciated_PlayerController.cs`: the old player movement script
- `Assets/Scripts/World/_Depreciated/EventHandler.cs`: an unused and empty placeholder

Do not use these files in new scripts or scenes. They should remain until the group confirms they are no longer needed by another branch or scene.

## Adding a Gameplay System

When adding a new system:

1. Give the script one clear purpose.
2. Use the current Rigidbody player and do not add new CharacterController requirements.
3. Check any required Inspector references before using them.
4. Decide what the system should do when the game is paused.
5. Add XML comments to public classes, methods, properties and events.
6. Test it in `MovementTest` or another dedicated test scene.
7. Update this guide, [KnownIssues.md](KnownIssues.md) and [DevelopmentLog.md](DevelopmentLog.md).
8. Record screenshots, videos, commits and test feedback as evidence.
