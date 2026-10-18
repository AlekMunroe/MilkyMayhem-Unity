# Known Issues

Update this file when an issue is found, fixed or deliberately postponed.


## Rigidbody Player

### Sliding is deliberately disabled

**Status:** Postponed

The sliding code is still in `PlayerController` but `Perm_CanSlide` is disabled on the player prefab because sliding is currently unreliable and is not required for the prototype.

**Next action:** Leave sliding disabled unless it becomes necessary which we will then fix. If we continue using sliding, test the collider height, camera movement, animations and the standing clearance before officially enabling it.

### Wall movement requires testing in the grey box

**Status:** Testing required

The wall edge sticking, forced jumping to detach and wall cliding have all been fixed. The player can detach by moving away from the wall and it uses wall momentum which gradually decreases using `wallMomentumHalfLife`.

Ground and wall detection now uses angles and the `Environment` layer.

**Next action:** Test and update the values of the wall movement in the grey box environment.

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


### Throw physics require balancing

**Status:** Balancing required

Throw strength, inherited player velocity, angular torque, bounce force, bounce count, safe landing speed and placement distance are prototype values. Current values work for testing but checks for the final world.

## Checkpoints, Scoring and Round Completion

### Final game-complete behaviour is not implemented

**Status:** Testing required

`GameManager` now runs the round timer and completes the round when all checkpoints are cleared or the timer reaches zero. The round complete event stops the player and camera and shows the results UI.

**Next action:** Test the game in multiple scenarios. The round must be able to complete each time.

### Round results are prototype UI

**Status:** Accepted for vertical slice

The gameplay UI shows completed deliveries, remaining deliveries and the remaining time. The results screen shows the completion time, deliveries completed, bottles broken and the final scre.

The current designs are temporary and will be replaced when the final UI designs are available.

### Checkpoint discovery only occurs at startup

**Status:** Accepted for current prototype

`GameManager` finds checkpoints under its container during `Start`. Checkpoints created later will not automatically be included in the round total.

### Development visual null handling is incomplete

**Status:** Open

`MilkCheckpoint` warns when its development visual is missing, but one startup path can still try to disable that object without a null check.

### Checkpoint feedback is provisional

**Status:** Accepted for vertical slice

Success and failure visuals and audio work, a failed delivery can be retried. The current assets are development feedback and will be replaced by final effects, audio and UI.

## Pause and Input

### WorldController assumes required references exist

**Status:** Open

`WorldController` expects an active `PlayerController`, its `CameraController`, the pause menu object and a keyboard. An incorrect scene setup might produce missing reference errors.

### Static pause state needs scene-change testing

**Status:** Open

The pause state is static and `WorldController` changes `Time.timeScale`. Scene changes while paused should be tested to make sure when the next scene starts unpaused, at normal speed and with the correct cursor state.

## UI

### PlayerUIController still uses a scene reference

**Status:** Planned improvement

`PlayerUIController` uses the correct Rigidbody `PlayerController` API, but the player reference must still be assigned in every scene.

**Next action:** Have the player reference from a future spawning or scene initialisation system, or find it once during startup. Do not search every frame.

### Delivery UI requries final design

**Status:** Accepted for vertical slice

The gameplay UI shows the delivery progress, remaining round time and sprint stamina. The round complete screen shows the results and round informaiton. The assets are temporary and need a final UI design and testing in different resolutions.

## Deprecated and Postponed Work

- The old CharacterController player prefab and script are found under `_Depreciated` folders and must not be used for any scenes.
- The empty `EventHandler` placeholder is also found under `_Depreciated` and is not part of any system as it is blank.
- Sliding is postponed rather than deprecated because its code is still part of the active player.

## Scenes and Builds

- `MainGame` is the current playable vertical slice but only uses the grey box environment and temporary assets.
- `DevMenu` and `MovementTest` remain development scenes and must be replaced from the final build.
- The `Assets` folder still require organisation.
- The final playable scenes, environment testing, hazards and production models are not complete.
- No automated gameplay tests currently exist.

### RestartTrigger is a prototype
**Status:** Testing required

`RestartTrigger` restarts the current scene after a countdown when the player is out of bounds by triggering the collider. It assumes the references are assigned and doesnt prevent multiple countdowns from starting.

**Next action:** Add missing reference and repeated trigger protection.