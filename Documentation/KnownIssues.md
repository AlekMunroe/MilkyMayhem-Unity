# Known Issues

Update this file when an issue is found, fixed or deliberately postponed.

## Player Setup

### Extra physics components on Player

**Status:** Needs correction

The `Player` in `MovementTest` currently has a CharacterController, CapsuleCollider and non-kinematic Rigidbody. The movement code is designed around the CharacterController. The extra physics components can cause conflicting collision or gravity behaviour.

**Recommendation:** Remove the CapsuleCollider and Rigidbody after confirming no other current feature depends on them.

### Player Visuals is unassigned

**Status:** Open

The `PlayerController.Player Visuals` field is unassigned in `MovementTest`. The CharacterController will shorten during a slide, but the visual object will not scale.

**Recommendation:** Assign the appropriate child visual Transform, probably the `CharacterObjects`, and verify the camera hierarchy is not unintentionally scaled.

### No overhead check after sliding

**Status:** Not implemented

The player restores the full controller height immediately when a slide finishes. Sliding under a low obstacle could make the player overlap the ceiling.

**Recommendation:** Add a standing-clearance check and keep the player lowered until there is enough space. We need to discuss this with the group.

## Scenes and Builds

### MovementTest is not in Build Settings

**Status:** Open

Build Settings contains a disabled reference to the removed `Assets/Scenes/SampleScene.unity`. `Assets/Scenes/_testing/MovementTest.unity` is not included. This is intentional however may be necessary to temporarily add for game testing using a build.

**Recommendation:** Add the intended playable scene before creating a build. Keep test scenes disabled for production builds.

### Player is not a prefab

**Status:** Open

The player setup currently exists directly in the test scene. Reusing this in other scenes may be difficult over time.

**Recommendation:** Complete the player systems then create a prefab.

## Input and Game State

### No WorldController or pause state

**Status:** Planned

The player and camera scripts update whenever the components are enabled. There is no shared state for pausing, movement or locking controls.

### Direct device input

**Status:** Accepted for prototype

Scripts read `Keyboard.current` and `Mouse.current` directly. This is simple but makes rebinding, controller support and automated input testing harder.

## UI

### PlayerUIController assumes references are assigned

**Status:** Open

`PlayerUIController` does not currently check for missing PlayerController or Slider references. An unassigned field will create a NullReferenceException.

### Sprint slider setup is scene-dependent

**Status:** Prototype

The slider expects values from 0 to 1 and is not interactable. These settings are enforced by the slider component directly.

## Camera

### CameraController assumes a mouse exists

**Status:** Open

`CameraController` reads `Mouse.current` without a null check. This is normally safe on the current PC target but should be checked before supporting other input configurations if this is decided in the future.

### Missing camera reference does not disable updates

**Status:** Open

`CameraController.Start` logs an error when `Cam Object` is missing, but the component continues updating and will later access the missing object.

## Balancing and Testing

- Movement, jump, wall and slide values need structured playtesting.
- The scene stores some values that differ from script defaults, including sprint stamina and wall-cling time.
- Wall clinging activates automatically when the airborne player is close enough to a Wall-layer surface.
- No automated gameplay tests currently exist.
- Delivery gameplay, scoring, timer and hazards are not implemented.

