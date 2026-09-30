# Project Conventions

## Unity Version

Everyone working directly in the Unity project must use Unity `6000.3.19f1`. Do not open and save the shared project using another Unity version because this can change scenes, assets and project settings. Discuss any editor upgrade with the whole group before changing version.

## Model Creation and Handoff

Create 3D models in a dedicated modelling software such as Blender or Maya. ProBuilder can be used for quick experiments inside Unity, but submitted environment or character models should not depend on ProBuilder unless this has been agreed with the developers beforehand.

Submit each model as an FBX and include all texture image files needed by it. Materials created in modelling software may not transfer perfectly into Unity, so texture files must also be supplied separately.

Before sending a model to a developer:

1. Give the model, objects, materials and texture files clear names.
2. Organise related objects into a sensible hierarchy rather than leaving many unnamed objects at the root.
3. Apply or freeze transforms and check that the model uses a sensible scale, orientation and pivot point.
4. Remove unused objects, materials and test geometry.
5. Export the finished model as an FBX.
6. Include its required texture images in a clearly named folder.
7. Import the exported files into Unity `6000.3.19f1` and confirm that the model appears correctly.
8. Check that there are no missing meshes, pink materials or Console errors before handing the files to a developer.

### Submitting Model Files

Send models to a developer as a ZIP file separate from the Unity project. Do not add unreviewed model files directly to the project's `Assets` folder or commit them to GitHub. This allows a developer to check the files and organise them correctly before they become part of the project.

Name the ZIP file clearly using the asset name and version, for example `TownGreybox_v1.zip`. Its contents should be organised like this Blender example:

```text
TownGreybox_v1
├── Source
|   ├── TownGreyBox.Blend
|   ├── References
|   |   ├── TownLayout.png
|   |   └── BuildingReferences.jpg
|   └── Textures
|       ├── Road_BaseColour.png
|       └── Road_Normal.png
├── TownGreybox.fbx
├── Textures
│   ├── Road_BaseColour.png
│   └── Road_Normal.png
├── Preview.png
└── SubmissionNotes.txt
```

The preview image is optional but recommended because it helps the developer confirm how the model is expected to look. The original Blender or Maya source file should also be included in a clearly named `Source` folder when another team member may need to edit it.

Do not include an entire Unity project or generated folders such as `Library`, `Temp`, `Logs` or `obj`. Only include the model, its textures, any useful source file, a preview and the submission notes.

Use the following template in `SubmissionNotes.txt`:

```text
Asset name:
Created by:
Date:
Version:
Intended use or scene:
Files included:
Scale or measurement units:
Material and texture information:
Special setup requirements:
Unity version tested in: 6000.3.19f1
Known issues:
```

Before submitting the ZIP file, extract it into a separate folder and check that it contains everything listed in the notes. The developer will review the submission, import accepted files into the correct project folders and create or adjust Unity materials when required.

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

