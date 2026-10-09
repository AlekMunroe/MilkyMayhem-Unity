# Milky Mayhem Programmer Guide

## Purpose

This guide explains what each current script does and how the main systems connect together. Read [PlayerSetup.md](PlayerSetup.md) before changing or rebuilding the player prefab.

## Current Architecture

```text
Keyboard and mouse
        |
        +--> PlayerController ------> Rigidbody movement and parkour
        |          |
        |          +---------------> SprintStaminaPercent
        |                                  |
        |                                  v
        |                           PlayerUIController
        |                                  |
        |                                  v
        |                            Sprint UI Slider
        |
        +--> CameraController ------> Player yaw and camera pitch
        |
        +--> MilkThrowing ----------> Thrown or placed MilkCrate
        |                                  |
        |                                  v
        |                           MilkCheckpoint
        |                                  |
        |                                  v
        |                             GameManager
        |                      Score, progress and round event
        |
        +--> WorldController -------> Pause state, time scale and cursor
                    |
                    +---------------> PlayerController
                    +---------------> CameraController
                    +---------------> Pause menu
```

The scripts currently read `Keyboard.current` and `Mouse.current` directly. We do not have an Input Actions asset or input rebinding system yet.

## PlayerController

**Path:** `Assets/Scripts/Player/PlayerController.cs`

**Responsibility:** Controls the first person Rigidbody player and its parkour movement.

Current features:

- Rigidbody movement during the physics update
- Ground acceleration, deceleration and a maximum speed
- Air acceleration, air drag and retained momentum
- Sprinting with stamina drain and regeneration
- A forward parkour jump boost
- Coyote time, jump buffering and variable jump height
- Wall detection, temporary wall clinging, wall sliding and wall jumping
- Pause support through `WorldController`
- Permanent controls which can disable incomplete features

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
- A `GroundCheck` child Transform
- The Ground and Wall layer masks
- A `CameraController` on the player

The old CharacterController version is depreciated. New scripts must use the current Rigidbody `PlayerController` and should not require a `CharacterController`.

Sliding code is still inside this script but `Perm Can Slide` is disabled on the prefab. Sliding is postponed because it is unreliable and not currently required.

## CameraController

**Path:** `Assets/Scripts/Player/CameraController.cs`

**Responsibility:** Rotates the player horizontally and the camera vertically.

`Cam Object` should use `CamPos`, with `Main Camera` placed underneath it. `WorldController` disables the component while the game is paused. The `WorldController` also controls the cursor rather than the `CameraController`.

## PlayerUIController

**Path:** `Assets/Scripts/UI/Gameplay/PlayerUIController.cs`

**Responsibility:** Updates UI connected to the player.

It currently reads `PlayerController.SprintStaminaPercent` and uses it to update the sprint Slider. It checks its references and makes the Slider non interactable. The player reference must still be assigned in each scene.

The score, remaining deliveries and timer can be added to the gameplay UI later. The pause menu and final results screen should remain as separate UI controllers.

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

Scoring should not be added to `MilkCrate`. This script only stores the condition of the crate. `MilkCheckpoint` decides if the delivery was successful.

## MilkCheckpoint

**Path:** `Assets/Scripts/Throwable/MilkCheckpoint.cs`

**Responsibility:** Checks a milk delivery and shows success or failure feedback.

It currently:

- Finds a `MilkCrate` from its collider or parent object
- Fails milk which is broken or moving above the safe delivery speed
- Shows failure feedback without clearing the checkpoint
- Allows the player to retry as many times as required
- Stops and moves successful milk onto the landing point
- Replaces failure feedback with success feedback
- Plays success or failure audio
- Prevents the checkpoint from completing more than once
- Sends the successful delivery and score amount to `GameManager`

The checkpoint Collider must have `Is Trigger` enabled. The success visual, failed visual, AudioSource, audio clips and landing point should all be assigned on its prefab.

## GameManager

**Path:** `Assets/Scripts/World/GameManager.cs`

**Responsibility:** Tracks checkpoint progress and the score for the current round.

Important public parts:

```csharp
public static GameManager Instance
public int Score
public int TotalCheckpoints
public int RemainingCheckpoints
public event Action<int> ScoreChanged
public event Action<int, int> CheckpointProgressChanged
public event Action RoundCompleted
public bool RegisterCheckpointCleared(MilkCheckpoint checkpoint, int scoreAmount)
```

When the scene starts, `GameManager` finds every checkpoint under its assigned checkpoint container. Each checkpoint can only report one successful delivery. When there are no checkpoints remaining, it raises `RoundCompleted`.

### Game Completion is Incomplete

The `RoundCompleted` event exists but the actual game complete system has not been made yet. Completing every checkpoint currently does not stop the player, stop throwing, stop a timer or show the results screen. A future game complete or results system should listen to this event and perform these actions.

## PauseMenuController

**Path:** `Assets/Scripts/UI/Menus/PauseMenuController.cs`

**Responsibility:** Controls the pause menu buttons.

It resumes the game through `WorldController`, shows or hides the return confirmation and loads the selected menu scene. It currently returns to the development menu. This will need changing when a final main menu exists.

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
