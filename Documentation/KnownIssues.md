# Known Issues

Update this file when an issue is found, fixed or deliberately postponed.


## Rigidbody Player

### Sliding requires more testing

**Status:** Testing required

The reimplementation of sliding is back. It keeps the existing momentum. `PermCanSlide` is enabled on the player and CTRL starts a slide when the player is grounded.

**Next action:** Test the collider, camera movement, animations and cooldown.

### Wall movement requires testing in the grey box

**Status:** Testing required

The current player has a vertical wall cling boost, wall sliding, wall detachment, retained wall momentum, air strafind and wall jumping. Ground, wall and air movement now have seperate perminant controls.

A problem exists when the player jumps away from a wall, they can move back towards it at a higher location.

Floor only objects can sometimes be trested as walls due to the `Environment` layer.

**Next action:** Investigate wall movement problems. Retest wall edges and corners for floor only object. Test wall jumping and airborne direction changes.

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

**Status:** Partially complete

`MilkCrate` now is the only script responsible for projectile lifetime. `MilkThrowing` doesnt schedule thrown or plaed crates for destruction.

Successful delviery crates are made kinematic with no velocity. The idle and max lifetime checks continue and could cause delivery crates to be broken or destroyed.

**Next action:** Decide if delivered crates should stay visible or disappear cleanly after a delay.

### Throw physics require balancing

**Status:** Balancing required

Throw strength, inherited player velocity, angular torque, bounce force, bounce count, safe landing speed and placement distance are prototype values. Current values work for testing but checks for the final world.

## Checkpoints, Scoring and Round Completion

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

Crates entering checkpoints now comlete deliveries without checking the impact speed. The old high speed function has now been removed. Successful feedback still work but failed fields exist with no way to fail.

**Next action:** Decide if we can fail deliveries in any way.

## Pause and Input

### WorldController assumes required references exist

**Status:** Open

`WorldController` expects an active `PlayerController`, its `CameraController`, the pause menu object and a keyboard. An incorrect scene setup might produce missing reference errors.

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

### Negative scale crate BoxCollider warnings

**Status:** Waiting for final environment model

Some grey box objects are on a negative scale. Unity forces the BoxCollider size to be positive and warns of this error.

**Next action:** 3D designers to send the correct scale and collider setup for the vianl environment moels.

Warnings can be ignored until we recieve the updated model.

### The checkpoint arrow interferes with wall movement

**Status:** Replacement planned

The temporary 3D checkpoint arrow is attached to the player and interfered with wall jumping. A UI compass is planned.

**Next action:** Replace the 3D arrow with the UI compass to point to a general delivery area and hide it when the player searches in the area.