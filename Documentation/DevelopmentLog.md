# Milky Mayhem Development Log

## Purpose

This is a working record of development decisions and progress. It can later be edited into the final university project documentation.

## 17 September 2026 Initial Concept

- Agreed on a first-person parkour game about a milkman completing deliveries.
- Chose the working title Milky Mayhem.
- Planned a vibrant, cartoon-style semi-open environment.
- Decided that successful deliveries add score and broken or misplaced bottles lose score.
- Chose one type of milk to keep the main game loop simple.
- Planned designed sets of delivery houses so routes can be tested and completed.
- Identified possible hazards including cars, roadworks and NPCs.

## Initial Player Prototype

- Updated `PlayerController` to use Unity's Input System.
- Created `CameraController` for first-person camera movement.
- Tested basic movement and camera controls.

## Parkour Movement

- Added jumping with forward movement.
- Added coyote time, jump buffering and variable jump height.
- Added sprinting with limited stamina and regeneration.
- Added wall detection and temporary wall clinging.
- Added wall sliding after the cling duration.
- Added wall jumping and a short reattachment delay.

## Sliding and Animation

- Added a timed ground slide which is activated when Ctrl is pressed while moving.
- Added a shorter CharacterController height during the slide to give the effect of ground sliding.
- Added optional visual scaling during the slide.
- Added Legacy slide animation playback that holds on the final frame.
- Added a Legacy reset animation to return animated objects to their normal pose.

## Player UI

- Created `PlayerUIController`.
- Exposed normalized sprint stamina through `PlayerController.SprintStaminaPercent`. This cannot be edited and will reset on the next frame.
- Added a Slider that displays the remaining sprint stamina.

## 29 September 2026 Documentation

- Added a project README and programmer documentation.
- Recorded player setup, system responsibilities and coding conventions.
- Recorded current setup problems and unfinished systems.

## 30 September 2026 Setup Fixes

- Removed the CapsuleCollider and Rigidbody from the Player and retested the player movement and other player systems.
- Converted the Player into `Assets/Prefabs/Player.prefab` to reuse between scenes.
- Tested visual scaling for sliding and decided to move away from scaling the entire root of the Player.
- Updated sliding visuals to lower the camera using `camSlidingHeight`, then reset the camera to its original height when sliding finishes.
- Decided not to support sliding under low obstacles, so a standing clearance check is not currently required.
- Added `DevMenu` and `MovementTest` to Build Settings, with `DevMenu` loading first.
- Created `Dev_MainMenu` to validate configured development scenes and load them through UI buttons.
- Added missing reference checks to `PlayerUIController` and made the sprint slider non interactable.
- Added missing camera and missing mouse checks to `CameraController`. The component disables itself when either component is missing.

## 30 September 2026 World State and Movement Fixes

- Created a `WorldController` instance and added it to `MovementTest` under the `Controllers` GameObject.
- Added the `isGamePaused` state and the Escape key is used for pause toggling.
- Added cursor locking, visibility control and direct coordination of PlayerController and CameraController during pausing.
- Added a PlayerController instance, a pause update method and a method that returns the CameraController.
- Added a PlayerController startup check that checks if a WorldController instance exists.
- Moved the WorldController's CameraController lookup from `Awake` to `Start` so the PlayerController instance is available first.
- Created `PauseMenuController` with resume, return confirmation and menu loading buttons.
- Added the pause menu to `MovementTest` and set up index `0` as the current main menu scene location.
- Added an unused time scale helper methods for possible future use. pausing does not currently change `Time.timeScale`.
- Fixed normal jumps next to walls by preventing wall clinging while the player is grounded or travelling upwards and by prioritising ground and coyote jumps over wall jumps.
- Allowed sprint movement to continue while the player is airborne.
- Added horizontal airborne momentum with customisable `airAcceleration` and `airDrag` values.
- Created the development only `Dev_InputVisualiser` prefab for displaying W, A, S, D, Space, Escape, Shift and Ctrl during recordings.
- Compiled the current C# project successfully with no compiler warnings or errors.

### Testing Notes

- The first pause test failed because WorldController had not been added to the scene.
- After adding the controller, the missing CameraController reference was traced to initialization order and fixed by performing the lookup in `Start`.
- The wall-jump, airborne sprint and momentum problems were recreated before their fixes were added.

## Next Tasks

- Receive a tested FBX export of Joe's greybox with the required textures, then import and organise it.
- Create and test the greybox environment scene.
- Add reference checks and scene change reset functions to the pause system.
- Update `PlayerUIController` so it can locate the active player when necessary.
- Replace the development Build Settings with the final scenes when they exist.
- Test and modify all the movement values to match the greybox.
- Add milk bottle throwing.
- Add milk placing mechanics.
- Add delivery zones.
- Add scoring and a round timer.
- Add hazards.

## 01 October 2026 Milk Projectile, Throwing Logic and Bases of Checkpoints
## These changes are found under feature/throwablemilk

- Created a MilkCrate projectile Prefab located in the Prefab folder.
- Created `MilkCrate` with logic of the milk projectile and added it to `MilkCrate` Prefab GameObject.
- Created `MilkThrowing` with logic of the milk throwing mechanic and added it to `MovementTest` under the `CamPos` GameObject.
- Created `MilkCheckpoint` with basic debug function for milk projectile detection and added it to `Checkpoint` Prefab GameObject.
- Added `Player` Layer and assigned it to the `Player` GameObject and its children GameObjects.
- Added `Projectile` Tag and assigned it to the `MilkCrate` Prefab GameObject.
- Added `Checkpoint` Tag and assigned it to the `Checkpoint` Prefab GameObject.

## Testing Notes

- The projectile bounces along one of the corners when it lands on its flat side when dropped vertically without rotation. The projectile spawned by the player will have randomised rotation and angular velocity.
- The projectile would quickly despawn if it was bounced in a corner. A cooldown was added to make the experience of using the bouncy projectile more forgiving.
- A customisable offset was added for future usage when first person animations are to be added to make the throwing animation more seemless.

## Bugs Noticed

- The world, while paused, does not freeze player interaction with throwing milk, allowing them to keep creating instances of milk projectiles as of the current implementation.
- The player is able to repeatively wall jump off the same wall when not holding any direction keys. Can be noted as mid air momentum is not changed by other means after the initial jump.

## Next Tasks

- Finalise and Polish milk thowing mechanic and milk projectile.
- Implement proper delivery zone logic and gameplaye events.


## 09 October 2026 Rigidbody Player and Delivery Prototype

### Aim

Assess the experimental Rigidbody branch and implement that and milk throwing into the main branch.

### Rigidbody PlayerController

- Reviewed the experimental Rigidbody controller and tested it's movements, physics and integration problems before updating it.
- Replaced the experimental RBPlayerController name with the active PlayerController name to make integration easier and help handle compatibility.
- Moved input collection into `Update` and Rigidbody movement into `FixedUpdate`
- Replaced the continuous force accumulation which was controlled with `Rigidbody.linearVelocity`, ground acceleration, ground decleration, air acceleration and air drag.
- Disabled the Rigidbody's gravity at runtime to allow for a custom gravity calculation.
- Updated jumping, jump release, wall clinging, wall sliding and wall jumping to use the Rigidbody's velocity.
- Restored the sprinting and regeneration along with `SprintStaminaPercent` for hte UI.
- Restored the forward parkour jump boost.
- Restored the `PlayerController.Instance`, `UpdatePause` and `GetCameraController` APIs used by the WorldController and the UI.
- Added a perminant feature switches. Useful for unstable systems.
- Added `Player_NoFriction` physics material to the player to allow the player to walk against walls without friction.
- Added the Rigidbody and CapsuleCollider component attributes to the `PlayerController`.

### Player and Scene Integration

- Replaced the Rigidbody player prefab name with `Player`
- Seperated the old CharacterController prefab and its scripts into `_Depreciated` folders so they can be found yet not used.
- Kept `WorldController` Connected to the new `PlayerController` singleton for pausing and finding the camera.
- Updated the development input visualiser to display the left and right mouse buttons.

### Milk Throwing and Placement

- Reworked `milkThrowing` to use the new Rigidbody `PlayerController` rather than `CharacterController.velocity`.
- Created a `ThrowOrigin` at the camera so milk will follow the camera's pitch.
- Added left click throwing using `ForceMode.Impulse`.
- Added right click milk placement which will place milk without movement, torque or bouncing.
- Added inherited player velocity, configurable throw strength, cooldown, spawn offset, random torque and projectile lifetime.
- Stopped spawned milk colliders from colliding with the player.
- Blocked throwing while the game is paused or while the cursor is unlocked.
- Refused random torque to `1` and set the MilkCrate Rigidbody angular damping to `1.5`.

### MilkCrate Physics and State

- Added a set bounce count, cooldown and kept it retaining linear velocity and a minimum bounce speed.
- Added a bounce surface LayerMask and seperate handling for playets, projectiles, checkpoints and normal surfaces.
- Prevented bounce counts fromo becoming a negative or contiuously repeating its count by one collision.
- Added a Rigidbody and Collider attribute and removed the unused components.
- Added a maximum lifetime cleanup for missed projectiles.
- Added `IsBroken` and `IsLanded`, current speed reporting, impact break detection and checkpoint landing support.
- Added`DisableBouncing` for milk which is placed.

### MilkCheckpoint and GameManager

- Replaced tag only projectile detection with `GetComponentInParent<MilkCrate>` so that child colliders are supported.
- Added a configurable safe delivery speed and score value.
- Added a `LandingPoint` that stops milk when it successfully lands, clears its velocity, makes it kinematic and aligns it to the checkport.
- Added failiure option which allows for retries.
- Added seperate success and fail visuals, sounds and a development only checkport representation.
- Added `CheckpointFeedbackVisualSpinner` for a visible prototype feedback.
- Updated `GameManager` to find all `MilkCheckpoint` components under a specified container.
- Added total checkpoint count, remaining checkpoint count, score tracking and duplicate completion protection.
- Added score, checkpoint progress and round complete events.
- Removed the duplicate scene `GameManager`.
- Enabled `Is Trigget` on the Checkpoint prefab.

### Deprecated Work

The following remains in the repository for references but is no longer used as part of the active prototype:

- `Assets/Prefabs/_Depreciated/Player.prefab`: the previous CharacterController player.
- `Assets/Scripts/Player/_Depreciated/Depreciated_PlayerController.cs`: the previous CharacterController movement implementation.
- `Assets/Scripts/World/_Depreciated/EventHandler.cs`: an unused empty placeholder from the experimental branch.
- The `RBPlayerController` filename and class name were replaced by the current Rigidbody based `PlayerController`.
- Sliding is not removed, but its feature is disabled because the Rigidbody version is currently unreliable.

### Testing Completed

- Confirmed the Rigidbody player can walk without continuing acceleration.
- Confirmed the frictionless player materal stops the player "sticking" to walls.
- Confirmed jumping, sprinting and camera movement all work after the conversion.
- Confirmed that milk will follow the cameras pitch when its thrown from `ThrowOrigin`.
- Confirmed left click throws and right click places milk.
- Confirmed that placed milk disabled the bouncing.
- Tuned initial torque and angular damping to reduce accessive spinning.
- Confirmed that failed deliveries can be attempted again with a new crate.
- Confirmed that successful deliveries will replace failure feedback.

### Not Yet Implemented

- `GameManager.CompleteRound` doesnt stop gameplay, pause the timer or disable the players input. It currently logs completiona and only invokes `RoundCompleted`.
- The round timer has not been implemented.
- A player facing score, remaining deliveries and end of round UI has not been implemented.
- Final score results for broken, misplaced or late milk have not been agreed on yet.
- Hazards and final delivery content have not been implemented.
- Final movement, throw, bounce and impact values have not been properly tested in the final environment.

### Known Bugs and Limitations

- Sliding is disabled because the Rigidbody behaviour is unstable.
- Wall clinging and wall jumping require complete testing with the new Rigidbody player.
- `MilkThrowing` doesnt disable itself safety when `Projectile Prefab` is missing.
- A successfully landed crate is still destroyed after its lifetime expires.
- `MilkCrate` checks breaking impact before ignoring the player so it may still collide unexpectedly and might still mark milk as broken.
- `MilkCheckpoint.Start` can still access a missing dev visual in a retail build.
- Checkpoints which are added outside the container, or added after `GameManager.Start` cannot be counted automatically.
- Controller scripts will still assume multiple scene references exist and need null checks.
- The project only supports keyboard and mouse directly and does not support rebinding or controllers.

### Next Steps

- Implement an actual game completion system and the round timer.
- Add score, checkpoint progress, timer and results UI.
- Test multiple checkpoints to completion.
- Choose one owner for projectile lifetime and decide if delivered milk will continue to be visible.
- Complete the missing reference safety checks.
- Re test and balance the Rigidbody player and delivery physics int he greybox environment.

--END UPDATE--

## Entry Template

### Date and Feature

**Aim:** What was the intended result?

**Work completed:** What changed?

**Testing:** How was it tested and what happened?

**Problems:** What failed or remains unfinished?

**Next steps:** What should happen next?

