# Player Setup

## Active Player Prefab

The active Rigidbody player is stored in `Assets/Prefabs/Player.prefab`. Make any changes in Prefab Mode so every scene receives the same setup.

The old CharacterController player is deprecated and stored in `Assets/Prefabs/_Depreciated/Player.prefab`. Do not use it in any scenes and do not delete it until the group has reviewed the migration.

## Current Hierarchy

```text
Player
├── CharacterObjects
│   ├── L-Leg
│   └── R-Leg
├── GroundCheck
└── CamPos
    └── Main Camera
        └── ThrowOrigin
            ├── MilkThrowing
            └── CheckpointArrowOrigin
                └── ArrowObject
```

Keep any visible character models under `CharacterObjects`. `GroundCheck` is in the hierarchy but the angle based detection system uses the CapsuleCollider's bounds instead. `CamPos` is the camera's pivot which is lowerev by the sliding system (postponed). `ThrowOrigin` controls where milk crates appear. The checkpoint arrow object are stored in the player prefab and must be assigned to `CheckpointArrow`.

## Components on Player

The root `Player` object requires:

- `Rigidbody`
- `CapsuleCollider`
- `PlayerController`
- `CameraController`
- Legacy `Animation`

Do not add a `CharacterController`. Movement and collision are now handled by the Rigidbody and CapsuleCollider.

## Rigidbody Setup

Use these rules as the baseline:

- Mass: `1`
- Built in `Use Gravity`: disabled because `PlayerController` applies custom gravity
- Freeze rotation on X, Y and Z so collisions do not tip the capsule over
- Use interpolation if camera movement appears visibly stepped
- Use a continuous collision mode if fast movement passes through thin colliders
- Keep damping low and tune movement through the controller's acceleration and deceleration fields

The script currently disables the built in gravity at runtime as a safety measure. It should also be disabled on the prefab directly so the Inspector matches the intended behaviour.

## CapsuleCollider Setup

Assign the project's frictionless player Physics Material to the CapsuleCollider. This prevents collider friction from stopping the player while against a wall.

The CapsuleCollider should fit the standing player and be centred on the root object. Retest ground and wall checks after changing its radius, height or centre.

## PlayerController Setup

`PlayerController` contains the active Rigidbody movement system. Its main features are:

- Controlled ground acceleration, deceleration and maximum speed
- Air acceleration, air drag and retained momentum
- Sprinting with stamina drain and regeneration
- Parkour jump boost, coyote time, jump buffering and variable jump height
- Wall detection, temporary wall clinging, wall sliding and wall jumping
- Pause integration through `WorldController`
- Public stamina data for `PlayerUIController`

### Permanent Feature Controls

The `Perm Can` fields allow incomplete features to be blocked. The current prefab should have normal movement, sprinting and jumping enabled. `Perm Can Slide` is currently disabled because sliding is postponed and unreliable.

Do not enable sliding simply to test another feature. If it is restored, document the change and retest collider height, camera height, animation reset and standing clearance.

### Ground and Wall Checks

1. Put every surface ground or wall surface as the `Environment` layer.
2. Assign the `Environment` to the `PlayerController` envionment mask.
3. Make sure every surface has non trigger colliders.
4. Use `Max Ground Angle` to control the steepest surface which counts as ground.
5. Use `Min Wall Angle` and `Max Wall Angle` to control which surface counts as a wall.
6. Test the floor edges, wall corners and slopes after changing the detection distances or angles.

The current system uses the normal of the surface to choose if its a ground or a wall. This will allow one environment object to contain both walkable and climbable surfacine without requiring seperate Ground and Wall layers.

The current wall movement keeps mometum moving along the wall and gradually reduces it using `wallMomentumHalfLife`. `wallDetachSpeed` controls how long it takes the player to detach from a wall. `wallReattachDelay` stops the player from sttaching to a wall immediately after detaching.

## CameraController Setup

1. Keep `CameraController` on the root `Player` object.
2. Assign `CamPos` as the camera object.
3. Keep `Main Camera` below `CamPos`.
4. Adjust mouse sensitivity and maximum look angle in the Inspector.

Horizontal look rotates the player. Vertical look rotates the camera's pivot. `WorldController` disables camera input while paused.


`CheckpointArrow` controls a temporary 3D arrow attached to the player.

1. Keep the arrow model attached.
2. Add the arrow object to `ArrowObjectOrigin.`
3. Use `Model Rotation Offset` if the model does not point correctly on the Z axis.
4. Make sure the scene's `GameManager` is enabled.
5. Make sure the checkpoint is assigned in the `GameManager`.

The arrow reads the checkpoints using `GameManager.Checkpoints`. It then points to the closes available checkpoint and hides when there are no checkpoints available.

## Milk Throwing Setup

The milk throwing component belongs below the camera on `ThrowOrigin`, not on an unrelated scene object. This allows throwing and placement to follow the camera's pitch.

Required setup:

1. Keep `ThrowOrigin` in front of the camera so a new crate does not spawn inside the player.
2. Assign the milk crate projectile prefab.
3. Assign the player's Rigidbody when its required by the component.
4. Confirm spawned crates have `Rigidbody`, `Collider` and `MilkCrate` components.
5. Use left mouse to throw and right mouse to place.

`MilkThrowing` ignores collisions with other spawned crates and the player. It also blocks input while paused, while the cursor is unlocked or while UI is being used.

## Player UI Setup

1. Create a Slider with a minimum of `0` and maximum of `1`.
2. Add `PlayerUIController` to the gameplay UI controller object.
3. Assign the active Rigidbody `PlayerController`.
4. Assign the sprint Slider.
5. Assign the delivery progress and remaining time TMP text objects.
6. Assign the round results and round summary TMP text objects.
7. Keep `PlayerUIController` on an object which will stay active when `RoundCompleteUI` hides.

The controller makes the Slider non interactable and updates it using `PlayerController.SprintStaminaPercent`. It listens to the GameManager's checkpoint, timer and round complete events to update the UI. The player reference is still scene dependent and should be checked whena new gameplay scene is created.

## World, Game and Pause Setup

The gameplay scene should contain one enabled `Controllers` object with:

- `WorldController` for pause state, time scale, cursor, player and camera control
- `GameManager` for checkpoint discovery, score and delivery progress
- Any dedicated UI controllers required by the scene

Assign the pause menu object to `WorldController`. Assign the parent containing the round's `MilkCheckpoint` objects to `GameManager` before testing.

Assign the pause menu object to the `WorldController`. Assign the parent with the all the `MilkCheckpoint` object and the `RoundCompleteUI` to `GameManager.

`GameManager` starts the round timer, tracks the checkpoints, the score, broken bottles. It completes the round when every checkpoint is completed or the timer reaches zero. `PlayerController`, `CameraController` and the `PlayerUIController` all listen to `RoundCompleted` to stop the gameplay and show the final results.

## Setup Verification

Before committing player or scene changes, verify:

- WASD movement accelerates and stops without unwanted sliding.
- The player can still move while touching a wall.
- Mouse look works and vertical rotation is clamped.
- Shift increases movement speed, drains stamina and later regenerates it.
- Sprinting continues correctly during a jump.
- Air momentum remains when movement input is released.
- Normal, coyote time, buffered and shortened jumps behave correctly.
- A jump beside a wall is not incorrectly cancelled by wall clinging.
- Wall cling, wall slide and wall jump work against all intended wall shapes.
- Moving away from a wall detaches the player without requiring them to jump.
- Momentum along the wall gradually reduces instead of stopping instantly.
- The player can exit at the top edge of a wall instead of getting stuck.
- A wall jump keeps momentum without creating too much force force.
- Left mouse throws a crate from `ThrowOrigin`.
- Right mouse places a crate without bounce behaviour.
- Pausing stops player, camera and throwing input and shows the cursor.
- The sprint Slider follows the active player's stamina.
- Successful deliveries update score and checkpoint progress only once.
- Failed deliveries show feedback and can be retried successfully.
- The Console contains no errors caused by missing references.
- Every floor and wall uses the `Environment` layer.
- The checkpoint arrow points to the closest available checkpoint.
- The delivery and timer UI show the correct values.
- Round completion stops the player and camera.
- Running out of time shows the failed round results.
- The result screen shows completion time, deliveries, broken bottles and the player's score.
- Restarting restarts the gameplay scene.