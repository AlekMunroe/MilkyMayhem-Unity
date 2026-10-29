# Player Setup

## Active Player Prefab

The active Rigidbody player is stored in `Assets/Prefabs/Gameplay/Player.prefab`. Make any changes in Prefab Mode so every scene receives the same setup.

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
```

Keep any visible character models under `CharacterObjects`. `GroundCheck` is in the hierarchy but the angle based detection system uses the CapsuleCollider's bounds instead. `CamPos` is the camera's pivot. `ThrowOrigin` controls where milk crates appear. The checkpoint compass is part of the gameplay UI and is no longer attached directly to the player.

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
- Air straginf and retained airborne momentum
- Parkour jump boost, coyote time, jump buffering and variable jump height
- Wall detection, temporary wall clinging, wall sliding and wall jumping
- Wall detachment
- Pause and round completion integration
- Perminant controls to disable features

### Permanent Feature Controls

The `Perm Can` fields allow incomplete features to be disabled without deleting the code.

The prefab currently has:

- Sprinting disabled
- Normal jumping enabled
- Parkour jumping enabled
- Ground movement enabled
- Wall movement enabled
- Air movement enabled
- Sliding enabled

Do not change any values without testing the complete player prefab.

### Ground and Wall Checks

1. Put every surface ground or wall surface as the `Environment` layer.
2. Assign the `Environment` to the `PlayerController` envionment mask.
3. Make sure every surface has non trigger colliders.
4. Use `Max Ground Angle` to control the steepest surface which counts as ground.
5. Use `Min Wall Angle` and `Max Wall Angle` to control which surface counts as a wall.
6. Test the floor edges, wall corners and slopes after changing the detection distances or angles.

The current system uses the normal of the surface to choose if its a ground or a wall. This will allow one environment object to contain both walkable and climbable surfacine without requiring seperate Ground and Wall layers.

The current wall movement keeps mometum moving along the wall and gradually reduces it using `wallMomentumHalfLife`. `wallDetachSpeed` controls how long it takes the player to detach from a wall. `wallReattachDelay` stops the player from sttaching to a wall immediately after detaching.

Floor only objects are sometimes treated as walls when the collider has enough surgace angle. Do not add another ground only layer until the next update is completed.

## CameraController Setup

1. Keep `CameraController` on the root `Player` object.
2. Assign `CamPos` as the camera object.
3. Keep `Main Camera` below `CamPos`.
4. Assign the player Camera and adjust the maximum look angle in the inspector.
1. Mouse sensitivity, field of view, head bob strength and inverted vertica camera are laoded from the saved settings.

Horizontal look rotates the player. Vertical look rotates the camera's pivot. `WorldController` disables camera input while paused.


## Checkpoint Compass Setup

1. Add the `CheckpointComapss` to the gameplay UI controller.
1. Assign the compass visual GameObject
1. Assign the compass needle RectTransform.
1. Assign the Player Transform.
1. Give every checkpoint a `CheckpointSearchArea`.
1. Give the search area a SphereCollider set to Trigger.
1. Assign the route prefab to the `GameManager`.

The compass will point to the cloasest checkpoint search area. It will hide when the player is inside the milk store or inside a search area.

## Milk Throwing Setup

The milk throwing component belongs below the camera on `ThrowOrigin`, not on an unrelated scene object. This allows throwing and placement to follow the camera's pitch.

Required setup:

1. Keep `ThrowOrigin` in front of the camera so a new crate does not spawn inside the player.
2. Assign the milk crate projectile prefab.
3. Assign the player's Rigidbody when its required by the component.
4. Confirm spawned crates have `Rigidbody`, `Collider` and `MilkCrate` components.
5. Use left mouse to throw and right mouse to place.

`MilkThrowing` ignores collisions with other spawned crates and the player. It also blocks input while paused, when the player is inside the milk store, while the cursor is unlocked or while UI is being used.

## Player UI Setup

1. Create a Slider with a minimum of `0` and maximum of `1`.
2. Add `PlayerUIController` to the gameplay UI controller object.
3. Assign the active Rigidbody `PlayerController`.
4. Assign the sprint Slider.
5. Assign the delivery progress and remaining time TMP text objects.
6. Assign the round results and round summary TMP text objects.
7. Assign the delivery infomration, timer and compass GameObjects.
8. Keep `PlayerUIController` on an object which will stay active when `RoundCompleteUI` hides.

The controller listens to the GameManager's events. The UI is hidden until the round starts. The timer flashes red when the low time warning starts.

## World, Game and Pause Setup

The gameplay scene should contain one enabled `Controllers` object with:

- `WorldController` for pause state, time scale, cursor, player and camera control
- `GameManager` for route creation, checkpoint searching, score and the delivery progress
- `BackgroundMusicController` for changing music between game states
- Any UI controllers required by the scene.

Assign the pause menu object to the `WorldController`. Assign the parent with the all the `MilkCheckpoint` object and the `RoundCompleteUI` to `GameManager.

`GameManager` waits for the `MilkStoreExitTrigger` to call `StartRound` then creates a random checkpoint route. It then starts the timer and tracks the checkpoints, score and bottles broken. It completes the round when all checkpoints are completed or the timer reaches zero.

## Setup Verification

Before committing player or scene changes, verify:

- WASD movement accelerates and stops without unwanted sliding.
- The player can still move while touching a wall.
- Mouse look works and vertical rotation is clamped.
- Ctrl starts sliding when the player is grounded and moving.
- Sliding restores the capsule and camera values.
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
- The compass points towards the closest available checkpoint search area.
- The comapss hides when the player is inside the checkpoint search area.
- The delivery and timer UI show the correct values.
- Round completion stops the player and camera.
- Running out of time shows the failed round results.
- The result screen shows completion time, deliveries, broken bottles and the player's score.
- Restarting restarts the gameplay scene.
- Air input does not immediately change direction with full velocity.
- Wall cling vertical boost doesnt allow uncontrolled climbing.
- The player doesnt move higher when returning to a wall after jumping away.
- Starting another game after going back to the main menu resets `Time.timeScale`.
- The round doesnt start while the player is inside the milk store.
- Leaving the milk store closes the door and starts the round.
- Throwing and placing are blocked inside the milk store.
- The 30 second warning runs correctly.