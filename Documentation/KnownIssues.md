# Known Issues

Update this file when an issue is found, fixed or deliberately postponed.


## Rigidbody Player

### Sliding is deliberately disabled

**Status:** Postponed

The sliding code is still in `PlayerController` but `Perm_CanSlide` is disabled on the player prefab because sliding is currently unreliable and is not required for the prototype.

**Next action:** Leave sliding disabled unless it becomes necessary which we will then fix. If we continue using sliding, test the collider height, camera movement, animations and the standing clearance before officially enabling it.

### Wall movement needs regression testing

**Status:** Testing required

A frictionless Physics Material fixed the issue where standing next to a wall stopped normal walking. The Rigidbody conversion also changed jumping, wall clinging and wall jumping.

**Next action:** Test these features with floors, corners, slopes and differently shaped walls. Check and confirm that a normal jump next to a wall is not replaced by a wall cling and that a wall jump will move away from the wall.

### Rigidbody gravity differs between the prefab and runtime

**Status:** Configuration cleanup

The player prefab currently stores `Use Gravity` as enabled, while `PlayerController` disables built in gravity at runtime because it applies custom gravity.

**Next action:** Disable `Use Gravity` on the prefab and check other prefabs for similar issues.

### Movement values are provisional

**Status:** Balancing required

Acceleration, deceleration, air control, sprinting, jump forces, wall behaviour and custom gravity work but have not been balanced for the final environment.

## Milk Throwing and Placement

### Missing projectile references still need stronger handling

**Status:** Open

`MilkThrowing` depends on its projectile prefab and throw origin being assigned correctly. A broken prefab setup can prevent throwing or create a runtime error.

**Next action:** Check required references during startup, disable the component when they are missing and log an error.

### Projectile lifetime has two owners

**Status:** Cleanup required

Both the `MilkThrowing` and `MilkCrate` scripts can schedule the spawned crate for destruction. This makes the lifetime behaviour harder to trace and means a crate landed successfully can disappear later.

**Next action:** Give `MilkCrate` ownership of its lifetime. Cancel or change its removal behaviour after a successful delivery.

### Impact checks occur before ignored-collision filtering

**Status:** Open

`MilkCrate` can evaluate an impact as broken before a collision with the player, another projectile or a checkpoint should be ignored.

**Next action:** Ignore some collisions before applying break or bounce logic.

### Throw physics require balancing

**Status:** Balancing required

Throw strength, inherited player velocity, angular torque, bounce force, bounce count, safe landing speed and placement distance are prototype values. Current values work for testing but checks for the final world.

## Checkpoints, Scoring and Round Completion

### Final game-complete behaviour is not implemented

**Status:** Not implemented

`GameManager` finds the round's checkpoints, counts the successful deliveries, updates the score then raises `RoundCompleted` when no checkpoints are left. Its completion method currently only logs the result and invokes the event. It doeesnt stop the players movement, stop throwing, pause the round timer or show a results screen.

**Next action:** Create the final round complete system and connect it to `GameManager.RoundCompleted`. The completion state should stop any further gameplay and show the final score and time without being treated as the pause menu.

### Round timer and gameplay results UI are missing

**Status:** Not implemented

There is no round timer, delivery progress display, score display or end round results screen yet.

### Checkpoint discovery only occurs at startup

**Status:** Accepted for current prototype

`GameManager` finds checkpoints under its container during `Start`. Checkpoints created later will not automatically be included in the round total.

### Development visual null handling is incomplete

**Status:** Open

`MilkCheckpoint` warns when its development visual is missing, but one startup path can still try to disable that object without a null check.

### Checkpoint feedback is provisional

**Status:** Prototype

Success and failure visuals and audio work, a failed delivery can be retried. The current assets are development feedback and will be replaced by final effects, audio and UI.

## Pause and Input

### WorldController assumes required references exist

**Status:** Open

`WorldController` expects an active `PlayerController`, its `CameraController`, the pause menu object and a keyboard. An incorrect scene setup might produce missing reference errors.

### Static pause state needs scene-change testing

**Status:** Open

The pause state is static and `WorldController` changes `Time.timeScale`. Scene changes while paused should be tested to make sure when the next scene starts unpaused, at normal speed and with the correct cursor state.

### Direct device input limits future input support

**Status:** Accepted for prototype

The gameplay scripts currently read `Keyboard.current` and `Mouse.current` directly. This is suitable for the keyboard and mouse prototype but makes input rebinding and controller support more difficult.

## UI

### PlayerUIController still uses a scene reference

**Status:** Planned improvement

`PlayerUIController` uses the correct Rigidbody `PlayerController` API, but the player reference must still be assigned in every scene.

**Next action:** Have the player reference from a future spawning or scene initialisation system, or find it once during startup. Do not search every frame.

### Delivery UI is not implemented

**Status:** Not implemented

The sprint slider works, but the player cant yet see their score, remaining deliveries, round time or final result in the gameplay UI.

## Deprecated and Postponed Work

- The old CharacterController player prefab and script are found under `_Depreciated` folders and must not be used for any scenes.
- The empty `EventHandler` placeholder is also found under `_Depreciated` and is not part of any system as it is blank.
- Sliding is postponed rather than deprecated because its code is still part of the active player.

## Scenes and Builds

- `DevMenu` and `MovementTest` remain development scenes and must be replaced from the final build.
- The `Assets` folder still require organisation.
- The final playable scenes, environment testing, hazards and production models are not complete.
- No automated gameplay tests currently exist.
