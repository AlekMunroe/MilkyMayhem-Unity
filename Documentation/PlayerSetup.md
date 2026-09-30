# Player Setup

## Recommended Hierarchy

The player is stored in `Assets/Prefabs/Player.prefab`. Edit shared player behaviour in Prefab Mode so every scene updates the player.

```text
Player
├── Main Camera
│   ├── CharacterObjects
│   │   ├── L-Leg
│   │   └── R-Leg
│   └── CamPos
└── GroundCheck
```

If more visual objects are added, keep them below the child object `CharacterObjects`. The root Player must not be scaled during sliding as this would also scale the CharacterController, camera, GroundCheck and future held objects.

## Components on Player

The recommended root components are:

- `CharacterController`
- `PlayerController`
- `CameraController`
- Legacy `Animation`

Do not add a Rigidbody or separate CapsuleCollider. The `CharacterController` already provides the collision shape used by the movement script. The old extra components were removed and the player was retested and works as expected.

## PlayerController Inspector Setup

### Movement and Sprinting

The current script defaults are:

| Setting | Default |
| --- | ---: |
| Walk speed | 6 |
| Gravity | -20 |
| Air acceleration | 5 |
| Air drag | 1.5 |
| Sprint speed | 10 |
| Maximum sprint stamina | 2 |
| Sprint drain speed | 1 |
| Sprint regeneration speed | 0.75 |
| Sprint regeneration delay | 1 second |

Scene values override script defaults. The current `MovementTest` scene stores maximum sprint stamina as 5, even though the script default is 2.

`Air Acceleration` controls how quickly movement can change direction while the player is in the air. `Air Drag` controls the gradual horizontal momentum loss when the player releases WASD in the air. Ground movement still responds instantly.

### Ground Check

1. Create a child named `GroundCheck` near the capsule's feet. The default transform for this is -1 on the Y axis.
2. Assign it to `Ground Check Radius`.
3. Put walkable surfaces on the `Ground` layer.
4. Select `Ground` in the `Ground Mask` field.
5. Make sure all walkable surfaces have non-trigger Colliders and the ground layers assigned.

The project currently defines these gameplay layers:

- `Ground`
- `Wall`

### Wall Movement

1. Put climbable surfaces on the `Wall` layer.
2. Select `Wall` in the `Wall Mask` field.
3. Make sure walls have Colliders.

The scene currently stores `Max Wall Cling Time` as `0.1`, the scripts default is `1`. This is still being tuned and needs testing.

### Sliding

Assign the following:

- `Player Cam`: the camera object that should lower during a slide
- `Cam Sliding Height`: the amount the camera moves down while sliding
- `Player Anim`: the Legacy `Animation` component
- `Slide Anim Clip`: `Assets/Animations/Character/Slide.anim`
- `Reset Anim Clip`: `Assets/Animations/Character/Reset.anim`

Sliding shrinks the CharacterController to `Slide Height` and lowers the camera by `Cam Sliding Height`. When the slide finishes, the controller and camera return to their default standing values. This replaced the earlier method where it scaled the root Player object directly.

All animation clips must be set to Legacy in the debug settings of the animation. The slide clip plays once and holds its final pose. The reset clip returns animated objects to their normal pose.

## CameraController Setup

1. Add `CameraController` to `Player`.
2. Assign `Main Camera` to `Cam Object`.
3. Adjust `Mouse Sensitivity` and `Max Look Angle`.

The camera should be a child of the player so horizontal player rotation also turns the view. Vertical rotation is applied only to the camera object assigned with the `CameraController`.

## World and Pause Setup

1. Create or select the root `Controllers` GameObject in the scene.
2. Add `WorldController` to it.
3. Assign the pause-menu GameObject to `Pause Menu UI`.
4. Keep the pause-menu GameObject inactive when gameplay starts.
5. Add `PauseMenuController` to the pause-menu GameObject.
6. Set `Main Menu Scene Index` to the required menu scene. `MovementTest` currently uses index `0` for `DevMenu`.
7. Assign the return-confirmation panel to `Confirm Return UI`.

The gameplay scene must contain one active `PlayerController` before `WorldController.Start` runs. The `WorldController` finds the player's `CameraController` automatically, locks the cursor and then coordinates player, camera and menu state when the Escape key is pressed.

The optional input display used for development recordings is available at `Assets/Prefabs/_Dev/Dev_InputVisualiser.prefab`.

## Player UI Setup

1. Create or select the Canvas.
2. Add a UI Slider named `SprintSlider`.
3. Set the slider minimum to 0 and maximum to 1.
4. Disable slider interaction because it only displays stamina.
5. Add `PlayerUIController` to the a dedicated child object under the dedicated `Controllers` object.
6. Assign the Player's `PlayerController`.
7. Assign `SprintSlider`.

## Setup Verification

Before committing player changes, verify:

- WASD movement works and diagonal movement is not faster.
- Mouse look works and vertical rotation is clamped.
- Shift drains stamina and stamina later regenerates.
- Sprint speed continues while jumping when the Shift key and a movement key are held.
- Releasing WASD in the air keeps momentum to the player and slows it down using `Air Drag`.
- Space performs normal and wall jumps.
- Coyote time and short jumps work.
- A normal or coyote time jump beside a wall is not cancelled by wall clinging.
- Wall cling changes into a wall slide after the set duration.
- Ctrl starts a slide while moving on the ground.
- The controller height and camera position reset after sliding.
- Slide and reset animations both play.
- The sprint slider follows the stamina value.
- Escape opens and closes the pause menu.
- Pausing enables the cursor, freezes the player and the camera input.
- Resume and return confirmation buttons perform their assigned actions.
- The Console has no errors or warnings caused by missing references.

