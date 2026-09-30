# Known Issues

Update this file when an issue is found, fixed or deliberately postponed.

## Scenes and Builds

### Development scene list is temporary

**Status:** Planned update

`DevMenu` and `MovementTest` are currently included in Build Settings for development builds. `DevMenu` loads first and has buttons to load into development scenes.

**Recommendation:** Replace the scene with a final menu when a Main Menu is built.

### DevMenu depends on manually configured scene names

**Status:** Prototype

`Dev_MainMenu` warns when a scene is not in Build Settings, but menu buttons must still be configured with the correct scene names.

**Recommendation:** Update its scene list whenever development scenes change. Update the system when a real main menu is created.

## Input and Game State

### No WorldController or pause state

**Status:** Planned

The player and camera scripts update whenever the components are enabled. There is no shared state for pausing, movement or locking controls.

### Direct device input

**Status:** Accepted for prototype

Scripts read `Keyboard.current` and `Mouse.current` directly. This is simple but makes rebinding, controller support and automated input testing harder.

## UI

### PlayerUIController does not automatically locate the player

**Status:** Planned

`PlayerUIController` now validates its PlayerController and Slider references and disables itself if either is missing. It still requires the PlayerController reference to be assigned manually in each scene.

**Recommendation:** Either locate the player once when the UI starts or provide the reference through the future player-spawning or world-state system. Do not search every frame.

## Balancing and Testing

- Movement, jump, wall and slide values need structured playtesting.
- The scene stores some values that differ from script defaults, including sprint stamina and wall-cling time.
- Wall clinging activates automatically when the airborne player is close enough to a Wall-layer surface.
- No automated gameplay tests currently exist.
- Delivery gameplay, scoring, timer and hazards are not implemented.

