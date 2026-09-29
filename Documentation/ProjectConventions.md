# Project Conventions

## Unity Version

Use Unity `6000.3.19f1`. Discuss an editor upgrade with the group before opening and saving the project in another version. However, updating the project should not be necessary as this is the latest LTS.

## Folder Structure

Use the existing categories and add new folders deliberately:

```text
Assets
├── Animations
│   └── Character
├── Scenes
│   ├── _testing
│   └── Gameplay
├── Scripts
│   ├── Player
│   ├── UI
│   │   └── Gameplay
│   ├── World
│   └── Delivery
├── Prefabs
├── Materials
└── Audio
```

Test scenes belong in `Assets/Scenes/_testing`. Production scenes should not use the `_testing` folder.

## Naming

- Classes and filenames: `PascalCase`, for example `PlayerController.cs`
- Methods and properties: `PascalCase`
- Private fields and local variables: `camelCase`
- Boolean names should read like questions: `isSliding`, `canSprint`, `hasDelivered`
- GameObjects and prefabs: descriptive `PascalCase` names
- Avoid names such as `Manager2`, `NewScript` or `Cube (12)` in production content

The C# filename and class name must match.

## C# Style

- Use `private` explicitly.
- Use `[SerializeField] private` when a value must appear in the Inspector.
- Keep each script focused on one system.
- Use XML comments for public APIs and important classes.
- Use ordinary comments to explain why code exists, not to repeat the line below it.
- Remove unused imports, fields and commented-out code before merging.
- Check Inspector references for null before using optional objects.
- Prefer clear method names such as `UpdateSprint` and `PerformWallJump` not `DoStuff` and `Jumping1`.

## Scenes and Prefabs

- Avoid having two people edit the same `.unity` scene at the same time.
- Convert reusable objects, especially the player, into prefabs.
- Make prefab changes in Prefab Mode where possible.
- Keep experimental objects in test scenes.
- Do not rename layers, tags or script fields without telling the group because serialized references will break.

## Git Workflow

Create a branch for each feature or fix:

```text
feature/world-controller
feature/milk-throwing
feature/delivery-zones
fix/slide-standing-height
docs/player-setup
```

Write commit messages that describe the result:

```text
Add sprint stamina UI
Implement wall jump cooldown
Document player prefab setup
Fix slide direction input
```

Before sharing work:

1. Save the scene and assets.
2. Exit Play mode.
3. Check the Unity Console.
4. Review changed files.
5. Test the affected feature.
6. Update documentation and known issues.
7. Commit the relevant `.meta` files with their assets.

## Files That Must Not Be Committed

The repository `.gitignore` already excludes most generated Unity files. Do not commit:

- `Library`
- `Temp`
- `Logs`
- `obj`
- `.vs`
- Generated `.csproj` and `.sln` files
- Personal IDE settings such as `.idea`

You must commit Unity `.meta` files. Unity uses them to preserve asset GUIDs and references.

## What does being "done"

A feature is ready for the group when:

- It works in its intended scene.
- It shows no Console errors.
- The Inspector fields are assigned.
- Required layers, tags and prefabs have been documented.
- Another programmer can follow the setup instructions.
- Limitations are recorded in `KnownIssues.md`.

