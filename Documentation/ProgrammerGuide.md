# Milky Mayhem Programmer Guide

## Purpose

This guide describes the current code structure and the responsibilities of each system. Read [PlayerSetup.md](PlayerSetup.md) before rebuilding or changing the player hierarchy.

## Current Architecture

```text
Keyboard and mouse
        |
        +--> PlayerController ------> CharacterController (movement)
        |          |
        |          +---------------> SprintStaminaPercent
        |                                  |
        |                                  v
        |                           PlayerUIController
        |                                  |
        |                                  v
        |                              UI Slider
        |
        +--> CameraController ------> Player yaw and camera pitch
        |
        +--> WorldController -------> Pause state and cursor control
                    |
                    +---------------> PlayerController (freeze input updates)
                    +---------------> CameraController (enable or disable)
                    +---------------> Pause menu (show or hide)

PauseMenuController --------------> Resume or return to the development menu
```

The current scripts read devices directly through `Keyboard.current` and `Mouse.current`. There is no Input Actions asset or central input wrapper yet however, this is to be implemented in the future.

## PlayerController

**Path:** `Assets/Scripts/Player/PlayerController.cs`

**Responsibility:** Controls the entire first person player with movement and animation

Implemented features:

- WASD movement through a `CharacterController`
- Sprinting with stamina drain, a regeneration delay and regeneration over time
- Parkour jump with a slight forward boost
- Coyote time (a short time for the player to jump after leaving the platform), jump buffering and variable jump height
- Wall detection on all four sides
- Temporary wall clinging, wall sliding and wall jumping
- Ground sliding with a reduced controller height
- Camera lowering during a slide using `camSlidingHeight`
- Legacy slide and reset animation playback
- A PlayerController instance used by `WorldController`
- Airborne momentum with configurable acceleration and drag
- Sprint movement while in the air
- Ground and coyote time jumps taking priority near walls
- A pause function in the `WorldController` script which freezes player updates

Important public API:

```csharp
public static PlayerController Instance
public float SprintStaminaPercent
public void UpdatePause(bool isPaused)
public CameraController GetCameraController()
```

This value is normalized between 0 and 1 and is currently used by the `PlayerUIController`.

Important dependencies:

- `CharacterController` on the same GameObject
- A `GroundCheck` child Transform
- Ground and Wall layer masks
- A camera GameObject assigned to `Player Cam`
- A Legacy `Animation` component
- Legacy slide and reset clips

## CameraController

**Path:** `Assets/Scripts/Player/CameraController.cs`

**Responsibility:** To rotate the player object horizontally and the camera object vertically.

Required reference:

- `Cam Object`: A parent object inside the main player object with a camera component.
```text 
Player object (PlayerController.cs)
|
V
Camera object
|
V
Camera (CameraController.cs, camera component)
```

The vertical angle is clamped using `Max Look Angle` to limit how far the player can look without acting "unnatural".

The cursor locking and pausing are not manage by the `CameraController`. The `WorldController` disables the entire component while the game is paused and enables it again when gameplay is resumed.

## PlayerUIController

**Path:** `Assets/Scripts/UI/Gameplay/PlayerUIController.cs`

**Responsibility:** Update the player related UI elements.

It currently reads `PlayerController.SprintStaminaPercent` each frame and assigns it to the sprint `Slider` value.

Required references:

- `Player Controller`: the scene's `PlayerController`
- `Sprint Slider`: the UI Slider used to display stamina

Future player HUD elements such as health or interaction prompts can be added here. Pause menus, settings and end-of-round screens should use separate UI controllers.

`PlayerUIController` now validates both references in `Awake`, disables itself when a necessary reference is missing and makes the sprint slider non-interactable automatically. It does not locate the player automatically yet. This is a future development.

## WorldController

**Path:** `Assets/Scripts/World/WorldController.cs`

**Responsibility:** Owns all gameplay states and updates controls for worldwide events such as pause states.

Current behaviour:

- Provides a scene-level `WorldController.Instance`.
- Listens for the Escape key using Unity's Input System.
- Holds the pause state in `isGamePaused`.
- Locks and hides the cursor when the game is active.
- Unlocks and shows the cursor when the game is paused.
- Calls `PlayerController.UpdatePause` to freeze or resume the player's updates such as movement and gravity.
- Disables `CameraController` while the game is paused and enables it when the game resumes.
- Toggles the pause menu UI GameObject.

The controller currently coordinates these systems directly rather than broadcasting a pause event. New gameplay systems that accept input must therefore be connected to the pause flow deliberately.

The private `SetGameSpeed` and `ResetGameSpeed` helpers exist but are not currently used by pausing. The current pause implementation does not change `Time.timeScale`.

## PauseMenuController

**Path:** `Assets/Scripts/UI/Menus/PauseMenuController.cs`

**Responsibility:** Handles buttons part of the pause menu.

It can resume the game through the `WorldController`, show or hide the return confirmation panel and load the configured main menu scene index. In `MovementTest`, index `0` currently returns to the development menu and should be the default menu for the final main menu.

## Dev Main Menu

**Path:** `Assets/Scripts/_Dev/Dev_MainMenu.cs`

**Responsibility:** Provides a temporary entry point for development builds.

`DevMenu` is the first scene in Build Settings. `Dev_MainMenu` it checks if the scene names exist and logs a warning when one does not exist. Its public `LoadScene` method is used by menu buttons to load into development scenes.

This is a development only system. When a real main menu exists, the development scene list and startup behaviour must be changed and hidden to public non development builds.

## Development Input Visualiser

**Path:** `Assets/Scripts/_Dev/Dev_ShowKeysOnScreen.cs`

**Prefab:** `Assets/Prefabs/_Dev/Dev_InputVisualiser.prefab`

**Responsibility:** Visualises keyboard inputs on screen during development testing and recording demonstrations.

This is a development-only tool and should not be included in the final player-facing UI.

## Legacy Animation Flow

`PlayerController` registers both slide and reset clips with the assigned Legacy `Animation` component.

1. Sliding begins and the slide clip plays with `WrapMode.ClampForever`.
2. The clip plays once and holds its final frame to simulate sliding.
3. Sliding finishes and the reset clip plays.
4. The reset clip holds its final default pose.

All clips must be marked as Legacy. They must all be attached to an Animation component of each object in the hierarchy.


The reusable methods are:

```csharp
SetupAnimationClip(AnimationClip animationClip, WrapMode wrapMode)
PlayAnimation(AnimationClip animationClip, WrapMode wrapMode)
```

## Adding a New Gameplay System

When adding a system:

1. Give the script one clear responsibility.
2. Inspector references are preferred.
3. Document all required components in the relevant files linked from the [Read Me](../README.md).
4. Add XML documentation to public classes, methods and properties.
5. Test the feature in `MovementTest` or a dedicated test scene.
6. Update this guide, [KnownIssues.md](KnownIssues.md) and [DevelopmentLog.md](DevelopmentLog.md).
7. Record all your progress.
