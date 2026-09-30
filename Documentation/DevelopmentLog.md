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

## Entry Template

### Date and Feature

**Aim:** What was the intended result?

**Work completed:** What changed?

**Testing:** How was it tested and what happened?

**Problems:** What failed or remains unfinished?

**Next steps:** What should happen next?

