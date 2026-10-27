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

### 15 October 2026 Player Controller Further Polishing

### Aim

Improve the player controller to give a smoother player experience.

### Work completed

- Changed the Wall Cling mechanic to include a small vertical boost upon clinging and reworked the gravity function when wall clingling.
- Changed Wall Jumping to give the player a boost in the held direction.
- `PlayerController.UpdateMovement` has been renamed to `PlayerController.UpdateGroundMovement` and mid air movement is in `PlayerController.UpdateAirMovement`.
- Added Air Strafing to let the player have more control in mid air.
- Added a Wall Climb for when the player reaches the top of a wall while wall clinging.

### Scene Integration

- Dev Playground now includes 2 long walls for more wall jumping testing.

### Testing

- Wall Climbing is confirmed to be functional but requires small tunning to be more smooth in gameplay.
- Air Movement is confirmed to be functional.

### Problems

- The Air Strafing code is mostly found from the internet, still needs refinement as features like Bunny Hopping and instantly stopping mid air exists.
- When Wall Jumping while facing the wall, facing vertically upwards or holding the direction into the wall results in the player able to repeatively jump into the wall.

### Next Steps

- Refine and bugfix the player controller to improve player experience.

### Further Wall Movement Bug Fixing

- Removed the Wall Climb as it made the player stick to the top edge of walls.
- Added wall detachment so the player can release from a wall without requiring a jimp.
- Added 'wallMomentumHalfLife' so the momentum will continue when attaching to a wall and will gradually slow down instead of instantly stopping.
- Stopped helf movement from continuing to add speed when the player is attached to a wall.
- Updated wall jumping to carry the any momentum the player has left to the next wall jump.
- Kept the final wall jump as one velocity to stop the extra force from the prototype version.
- Tested and found 'wallJumpForce' at '8' works well.

### 16 October 2026 Vertical Slice Gameplay Loop

### Aim
Create the first playable vertical slice by connecting the exiting mechanics and systems into one complete gameplay loop.

### Work completed

### Player Movement and Surface Detection
- Tested the updated forward momentum for wall jumping.
- Found the forward movmenet works but its too simple and the player has little control.
- Fixed the uncontrolled momentum when the player attaches to a wall.
- Added wall detachment when the player moves away from a wall.
- Added back the prototype vertical wall cling boost.
- Added the `Environment` layer for surfaces to be detected as ground or walls.

### Greybox and Scene Setup
- Imported the grey box environment.
- Scaled the grey box to `0.4, 0.4, 0.4` to work with the current player's scals.
- Added the `Environment` layers and added visible region borders for the vertical slice.
- Created `MainGame` as the complete gameplay scene.
- Created `RestartTrigger` to restart the scene if the player falls out of bounds.

### Checkpoint Navigation
- Added a temporary 3D directional arrow.
- Exposed the checkpoints list using `GameManager.Checkpoints` using `IReadOnlyList`.
- Created a `CheckpointArrow` to find the closest available checkpoint.
- Updated the arrow to rotate towards the closest checkpoint and hide it when no checkpoints are left.

### Gameplay UI and Round Completion
- Added UI for completed deliveries, remaining deliveries and the round timer.
- Added a configurable round timer to `GameManager`.
- Added success and failed round completion.
- Added a results screen which shows the completion time, completed deliveries, broken bottles and the player's final score.
- Connected `PlayerController`, `CameraController` and `PlayerUIController` to `GameManager.RoundCompleted`.
- Stopped the movement and camera when the round completes.
- Enabled the cursor when the results screen opens.
- Added a `RoundCompletionMenu` with restart and return to main menu buttons.

## Milk Crate Feedback
- Added seperate `DefaultObject` and `BrokenObject` objects to the milk crate prefab
- Added temporary throwing and bottle breaking sounds.
- Added broken bottle tracking to the `GameManager`.

### Testing
- The player can move through the grey box.
- The angle based ground and wall detection system works with the Environment later.
- The player can detach from walls.
- Wall momentum gradually decreases rather than instantly stopping.
- The arrow points towards the closest available checkpoint.
- Completing all deliveries stop the round.
- The round timer reaching zero before completing all deliveries stop the round.
- The results UI shows the current round information.
- The current scene can be restarted.

### Problems
- Forward wall jumping momentum requires testing for the grey box environment.
- The grey box scale and pivot were not set reletive to the player and requires manual adjustment.
- The restart trigger is a basic prototype and needs testing and more protection.
- The current UI, object and audio are temporary.
- Pause controls need testing properly with these updates.

### Next steps
- Test the complete gameplay loop repeatedly with different goals.
- Fix any bugs which prevent the game from completing.
- Testing and updating values.
- Explain the model scale and pivot requirements to the 3D designers.

## 20 October 2026 Main Menu and Settings

### Aim

To create the first complete main menu and add the start of the games settings

### Work completed

- Created the `MainMenu` scene.
- Added Play, Options, Scores, Credits and Quit to the main menu.
- Added options such as display, audio, camera, controls and gamplay options.
- Created version `0.1.1` for testing the main menu and options menu.

### Testing

- The main menu can load `MainGame` correctly
- Options saves successfully
- Version `0.1.1` can be built and run without errors.

### Problems

- Builds downloaded onto the University systems are blcoked and must be compiled directly on the systems.
- All settings save however, some are not yet connected to the systems.
- The Scores menu doesnt exist/
- The game doesnt clearly explain controls to players.


### Next steps

- Complete the main menu presentation
- Add score saving and the scores menu
- Connect all settings to the correct systems
- Add clearer UI to explain controls to users

## 22 October 2026 Playtesting Session

### Aim

Test version `0.1.1` to new players and find problems with the gameplay loop.

### Work completed

- Ran a playtesting session using the current main menu gameplay loop
- Note how players understand the game and attempt to play it.

### Testing

- Players understood the purpose of the checkpoint arrow.
- UI scaling worked correctly.
- Players understood the game better after their first attempt.
- Players were able to complete the gameplay loop after learning the controls.
- Milk crates broke too easy.
- The difference between throwing and placing milk crates is unclear to players.
- Player movement continues after round completion.
- The player can climb direcly up corners.
- Players attempted to climb up the visible boundaries.

### Problems

- The game doesnt explain controlls well enough.
- The environment doesnt have enough opportunities to show throwing, placing and parkour
- The game doesnt always reset when replaying
- The game has no consistent design in the UI
- Milk crates break too easy.

### Next steps

- Remove the unused `FakeObject` reference.
- Improve the player experience for new players.
- Fix the round completion and replaying states.
- Import the larger grey box.
- Continue developing UI.

## 23 October 2026

### Aim

Improve the game and move away from the grey box version.

### Work completed

- Added the game version and logos to the main menu
- Added the missing quit confirmation panel to the main menu
- Escape now closes secondary main menu panels
- Created reusable prefabs for buttons, dropdowns, sliders and toggles.
- Added hover animations to buttons.
- Updated the pause menu and gameplay UI.
- Updated the timer to format in `MM:SS` rather than seconds.
- Added a basic crosshair.
- Disabled sprinting and the UI.
- Removed the `FakeObject` reference in `PlayerUIController`
- Created an audio mixer with groups.
- Added background music to the main menu and gameplay.
- Completed the display and audio settings
- Added the save function to the frame rate option.
- Imported grey box v2.
- Updated the player movement values to work in the bigger environment.
- Created `SavedRoundResult`, `ScoreSaveSystem` and `ScoresMenuController`.
- Saved round results to PlayerPrefs as JSON.
- Added a scores menu.

### Testing

- The game version text changes between buids.
- The display and audio settings save and laod
- The audio mixer changes the volumes for music and sound effects.
- The movement works better in the larger grey box.
- The main menu buttons animate.

### Problems

- Settings save but some are not connected to the gameplay.
- The checkpoint arrow is hidden slightly by the updated gameplay UI.
- The final UI assets and fonts are yet to be agreed.
- The score saving system and replay systems need testing with many rounds.

### Next steps

- Connect the settings to their systems
- Update the round completion UI
- Test the saved scores across multiple rounds.
- Replace the 3D arrow with a UI compass.

## 26 October 2026

- Readded wall cling boost and air strafing from previous commits.
- Removed high speed checkpoint breaking from milk crates
- Removed projectile lifetime responsibility into `MilkCrate`.
- Adjusted and refined the `PlayerController` values.
- Seperated perminant ground, wall and air movement controls.

### Next steps

- Test movement in the alrger grey box
- Add abilities into the game

## 27 October 2026

### Aim

Merge the updated gameplay and new UI system projects into one combined project.

### Work completed

- Merged both projects into one.
- Removed the unnecessary debugs.
- Updated the round completion UI to match the design of other menus.
- Temporarily disabled the credits button.
- Updated the interruptionanimation states.
- Built `0.1.2` for testing.
- Fixed the game staying frozen once the gameplay scene loads a second time.
- Added game speed resetting when `MainMenuController` and `WorldController` starts.
- Refused `MaxAirSpeed` to `1` to stop the player immediately changing direction in the air.
- Created an unused basic compass UI for the replacement of `CheckpointArrow`

### Testing

- Tested game speed resetting.
- `MaxAirSpeed` gives the player less immediate control whilst airborne.
- Found BoxCollider warnings caused by negatively scaled greybox buildings.
- Found wall movement problem when moving back towards a wall after jumping away can bounce the player back towards the wall and vause them to move higher.
- Found the 3D checkpoint arrow interferes with wall jumping.

### Problems

- Negative scaled objects give errors with BoxColliders.
- Wall movement jumping bug.
- Floor only objects can sometimes be detected as walls.
- The UI compass is not created and does not yet replace the 3D arrow.
- The pause state can stop UI animations because it sets `Time.timeScale` to zero.
- MIlk throwing needs to be blocked when you gain focus to the game again.

### Next steps

- Replace the 3D checkpoint arrow with the UI compass.
- Decide how delivery crates should be removed.
- Add the round start trigger.
- Create and test multiple delivery routes.
- Add and test the complete round resetting.
- Test repeated rounds and score saving.

## Entry Template

### Date and Feature

### Aim
What was the intended result?

### Work completed
What changed?

### Testing
How was it tested and what happened?

### Problems
What failed or remains unfinished?

### Next steps
What should happen next?

