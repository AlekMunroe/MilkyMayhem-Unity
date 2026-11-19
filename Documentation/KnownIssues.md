# Known Issues

Update this file when an issue is found, fixed or deliberately postponed.


## Rigidbody Player

### Sliding requires more testing

**Status:** Testing required

The reimplementation of sliding is back. It keeps the existing momentum. `PermCanSlide` is enabled on the player and CTRL starts a slide when the player is grounded.

**Next action:** Test the collider, camera movement, animations and cooldown.

### Wall movement requires testing in the city environment

**Status:** In active development

The current player has a vertical wall cling boost, wall sliding, wall detachment, retained wall momentum, air strafind and wall jumping. 

The old bouncing back problem which was caused by the old wall stick force calculation has been fixed. Wall running still feels too slow and sticky at lower speeds. the player falls too slowly while touching some walls.

**Next action:** Update and retest wall running, normal jumps next to walls, wall sliding, wall detachment, wall edges and movement between multiple colliders.

### Rigidbody gravity differs between the prefab and runtime

**Status:** Configuration cleanup

The player prefab currently stores `Use Gravity` as enabled, while `PlayerController` disables built in gravity at runtime because it applies custom gravity.

**Next action:** Disable `Use Gravity` on the prefab and check other prefabs for similar issues.

### Movement values are provisional

**Status:** Balancing required

Acceleration, deceleration, air control, jump forces, wall behaviour and custom gravity work but have not been balanced for the final environment.

### Automatic mounting needs its final testing and approval

**Status:** Testing and approval required

The player can automatically mount smaller objects below the `maxMountHeight`.

**Next action:** Test running into benches, bins and other mountable objects from multiple directions and confirm they are mounted correctly. Confirm they are never treated as walls.

## Milk Throwing and Placement

### Missing projectile references still need stronger handling

**Status:** Open

`MilkThrowing` depends on its projectile prefab and throw origin being assigned correctly. A broken prefab setup can prevent throwing or create a runtime error.

**Next action:** Check required references during startup, disable the component when they are missing and log an error.

### Throw physics

**Status:** Testing required

Crates now inherit the players velocity.
Throw strength, inherited player velocity, angular torque, bounce force, bounce count, safe landing speed and placement distance are prototype values. Current values work for testing but checks for the final world.

## Checkpoints, Scoring and Round Completion

### Round results are prototype UI

**Status:** Accepted for vertical slice

The gameplay UI shows completed deliveries, remaining deliveries and the remaining time. The results screen shows the completion time, deliveries completed, bottles broken and the final scre.

The current designs are temporary and will be replaced when the final UI designs are available.

### Checkpoint feedback is provisional

**Status:** Requires final versions.

Crates entering checkpoints now comlete deliveries without checking the impact speed. The old high speed function has now been removed. Successful feedback still work but failed fields exist with no way to fail.

**Next action:** Decide if we can fail deliveries in any way.

### Failed rounds are saved

**Status:** Open

`ScoreSaveSystem` listens to `RoundCompleted` and saves both successful and failed rounds.

**Next action:** Decide if failed attempts should be stored separately or saved at all.

## Hazards

### Car knockback needs balancing

**Status:** Testing required

`CarDamage` nor uses the cars forward direction and dot product threshold to detect the player from the front. The current knockback force is still too powerful.

A vehicle movement system not exists and knockback now needs testing along with this.

**Next action:** Add the cars movement system then test damage and knockback at different speeds and angles.

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

The gameplay UI shows the delivery progress, remaining rount time and compass. The round complete screen shows the results and round informaiton. The current UI needs more resting in multiple resolutions.

### Head bob requires more testing

**Status:** Testing required

Head bobbing is visual but movement seems different near some wall edges.

**Next action:** Compare movement with head bob set to zero and check if this is caused by the camera or the existing wall edge bug.

## Deprecated and Postponed Work

- The old CharacterController player prefab and script are found under `_Depreciated` folders and must not be used for any scenes.
- The empty `EventHandler` placeholder is also found under `_Depreciated` and is not part of any system as it is blank.
- Sliding is postponed rather than deprecated because its code is still part of the active player.

## Scenes and Builds

- `MainGame` is the current playable vertical slice.
- `DevMenu` and `MovementTest` remain development scenes and must be replaced from the final build.
- The `Assets` folder still require organisation.
- The final playable scenes, environment testing, hazards and production models are not complete.
- No automated gameplay tests currently exist.

### TutorialScene is missing GameManager

**Status:** Open

`CameraController` expects a `GameManager` to subscribe to `RoundCompleted`. The TutorialScene doesnt have one and gives an error.

**Next action:** Add the game state setup or stop the tutorial camera from requiring the event.

### Performance drops when viewing the environment from a distance

**Status:** Waiting for LOD models

The frame rate drops when a large portion of the city is visible from a distance. The environment uses the full detail models at every distance.

**Next action:** Setup LOD0, LOD1, and LOD2 models when the 3D designers provide them.