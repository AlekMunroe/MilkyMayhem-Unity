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

### Pause system is still a prototype

**Status:** Implemented with limitations

`WorldController` now controls the PlayerController, CameraController, cursor and pause menu. It does not change `Time.timeScale`, so future physics, animations, timers or gameplay scripts will continue running unless they are explicitly connected to the pause flow.

**Recommendation:** When adding throwing, delivery or timer systems, decide how each system behaves while paused and connect it through `WorldController`.

### Pause state is not reset explicitly

**Status:** Open

`isGamePaused` is static and does not reset when a scene starts. Going to a new scene while paused may leave the state out of place, the cursor and other controls may not work as intended.

**Recommendation:** Reset the pause state and any changed global state during scene startup or shutdown, then test pausing when changing scenes. A loading system may be required to confirm everything works before unloading and loading a new scene.

### WorldController assumes required references exist

**Status:** Open

`WorldController.Start` assumes that `PlayerController.Instance` and its CameraController exist. `Update` assumes a keyboard exists, and the pause flow assumes `Pause Menu UI` has been assigned. Missing requirements may produce a `NullReferenceException` error.

**Recommendation:** Confirm the player, camera, keyboard and pause-menu references before using them. Disable the affected behaviour and log a clear error when a reference is unavailable.

### Pause API contains unfinished helpers

**Status:** Prototype cleanup

The current public pause entry point is `UpdatePause()`, which toggles state. Separate `PauseGame`, `ResumeGame` and `TogglePause` methods have not been created. `TogglePauseMenu` is empty, while `SetGameSpeed` and `ResetGameSpeed` are currently unused.

**Recommendation:** Remove the unused helpers or complete and rename the pause API before more gameplay systems depend on it.

### Direct device input

**Status:** Accepted for prototype

Scripts read `Keyboard.current` and `Mouse.current` directly. This is simple but makes rebinding, controller support and automated input testing harder.

The development input visualiser assumes a keyboard is connected and does not have any checks. This is acceptable for a development side setup but may require a patch if other devices are unsupported.

## UI

### PlayerUIController does not automatically locate the player

**Status:** Planned

`PlayerUIController` now checks PlayerController and Slider references and disables itself if either is missing. It still requires the PlayerController reference to be assigned manually in each scene.

**Recommendation:** Either locate the player once when the UI starts or provide the reference through the future player spawning or world state system. Do not search every frame.

## Balancing and Testing

- Movement, jump, wall and slide values need structured playtesting.
- Air acceleration and air drag have been added but still require playtesting in the final environment.
- The scene stores some values that differ from script defaults, including sprint stamina and wall-cling time.
- Wall clinging activates automatically when the airborne player is close enough to a Wall-layer surface.
- No automated gameplay tests currently exist.
- Delivery gameplay, scoring, timer and hazards are not implemented.

