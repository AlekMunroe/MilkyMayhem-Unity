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
| Sprint speed | 10 |
| Maximum sprint stamina | 2 |
| Sprint drain speed | 1 |
| Sprint regeneration speed | 0.75 |
| Sprint regeneration delay | 1 second |

Scene values override script defaults. The current `MovementTest` scene stores maximum sprint stamina as 5, even though the script default is 2.

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
3. Ensure walls have Colliders.

The scene currently stores `Max Wall Cling Time` as `0.1`, while the script default is `1`. This is still being tuned and needs testing.

### Sliding

Assign the following:

- `Player Cam`: the camera object that should lower during a slide
- `Cam Sliding Height`: the amount the camera moves down while sliding
- `Player Anim`: the Legacy `Animation` component
- `Slide Anim Clip`: `Assets/Animations/Character/Slide.anim`
- `Reset Anim Clip`: `Assets/Animations/Character/Reset.anim`

Sliding does not reduce the CharacterController height and only lowers the camera by `Cam Sliding Height`. When the slide finishes, the camera returns to its original standing position. This replaced the earlier setup which scaled the root Player object directly.

All animation clips must be set to Legacy in the debug settings of the animation. The slide clip plays once and holds its final pose. The reset clip returns animated objects to their normal pose.

## CameraController Setup

1. Add `CameraController` to `Player`.
2. Assign `Main Camera` to `Cam Object`.
3. Adjust `Mouse Sensitivity` and `Max Look Angle`.

The camera should be a child of the player so horizontal player rotation also turns the view. Vertical rotation is applied only to the assigned camera object.

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
- Space performs normal and wall jumps.
- Coyote time and short jumps work.
- Wall cling changes into a wall slide after the configured duration.
- Ctrl starts a slide while moving on the ground.
- The controller height and camera position reset after sliding.
- Slide and reset animations both play.
- The sprint slider follows the stamina value.
- The Console has no errors or warnings caused by missing references.

