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

## Next Tasks

- Correct the player physics components and assign `Player Visuals`.
- Convert the player into a prefab.
- Create `WorldController` and a pause menu.
- Build a dedicated parkour testing area.
- Test and balance all movement values.
- Add milk-bottle throwing.
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

