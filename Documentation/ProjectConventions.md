# Project Conventions

## Unity Version

Everyone working on the Unity project must use Unity `6000.3.19f1`. Do not open and save the shared project in another Unity version because it can change scenes, assets and project settings for the rest of the group.

Any Unity version change must be discussed with the group before the project is upgraded.

## Creating and Sending Models

Models should be created in a dedicated program such as Blender or Maya. ProBuilder can be used for quick tests in Unity but final or shared models should not require ProBuilder unless this has already been agreed with the developers.

Send each model as an FBX and include all textures required by it. Materials do not always transfer correctly into Unity so the original texture images must be included separately.

Before sending a model to a developer:

1. Give the model, objects, materials and textures clear names.
2. Put related objects into a clear hierarchy rather than leaving them at the root.
3. Apply or freeze the transforms.
4. Check the scale, direction and pivot point.
5. Remove unused objects, materials and test geometry.
6. Export the model as an FBX.
7. Include all textures in a clearly named folder.
8. Test the exported model in Unity `6000.3.19f1`.
9. Make sure there are no missing meshes, pink materials or Console errors before sending it.

### Submitting Model Files

Send models to a developer as a ZIP file which is separate from the Unity project. Do not place unreviewed models directly into the project's `Assets` folder or commit them to GitHub. This allows the developer to check and organise the files before adding them to the main project.

Give the ZIP file a clear name and version, for example `TownGreybox_v1.zip`. A Blender submission could use this structure:

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

The preview image is optional but recommended because it shows the developer what the model should look like. Include the original Blender or Maya file in `Source` when another group member might need to edit it later.

Do not send a full Unity project or generated folders such as `Library`, `Temp`, `Logs` or `obj`. Only send the model, textures, useful source files, a preview and the submission notes.

Use this template in `SubmissionNotes.txt`:

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

Before sending the ZIP file, extract it into a different folder and make sure it includes everything written in the notes. The developer will then review the files, move accepted assets into the correct project folders and create or fix Unity materials where required.

## Folder Structure

Use the existing folders and only create a new category when it is required:

```text
Assets
├── Animations
│   └── Character
├── Audio
├── Materials
├── Scenes
│   ├── _testing
│   └── Gameplay
├── Scripts
│   ├── _Dev
│   ├── Player
│   ├── Throwable
│   ├── UI
│   │   └── Gameplay
│   └── World
└── Prefabs
```

Test scenes should go in `Assets/Scenes/_testing`. Final gameplay scenes should not be stored in the `_testing` folder.

Milk throwing, crate and checkpoint scripts belong in `Assets/Scripts/Throwable`. Development only scripts and prefabs belong in `_Dev` folders and should not be used as final player UI or gameplay content.

## Depreciated Code and Assets

- Do not immediately delete an old system because another scene or branch might still require it.
- Move replaced scripts and prefabs into a clearly named `_Depreciated` folder through Unity so their `.meta` files move with them.
- Do not use anything inside a depreciated folder in new scripts or scenes.
- Document what replaced the old file and why it has been kept.
- Only remove it after the group confirms that nothing else requires it.

The old CharacterController player and the empty `EventHandler` are currently stored in `_Depreciated` folders. They are not part of the current game.

Sliding is postponed rather than depreciated. Its code is still part of the current Rigidbody `PlayerController` but `Perm Can Slide` is disabled.

## Naming

- Classes and filenames use `PascalCase`, for example `PlayerController.cs`.
- Methods and properties use `PascalCase`.
- Private fields and local variables use `camelCase`.
- Boolean names should read like a question, for example `isSliding`, `canSprint` or `hasDelivered`.
- GameObjects and prefabs should have clear `PascalCase` names.
- Do not use unclear names such as `Manager2`, `NewScript` or `Cube (12)` in final content.

The C# filename and class name must match.

## C# Style

- Write `private` rather than leaving it implied.
- Use `[SerializeField] private` when a private value needs to be changed in the Inspector.
- Give each script one clear purpose.
- Add XML comments to public APIs and important classes.
- Use normal comments to explain why something is being done rather than repeating the code.
- Remove unused imports, fields and commented out code before merging.
- Check Inspector references before using them.
- Use clear method names such as `UpdateSprint` and `PerformWallJump` rather than names such as `DoStuff` or `Jumping1`.

## Scenes and Prefabs

- Avoid having two people edit the same `.unity` scene at the same time.
- Turn objects used in multiple scenes into prefabs.
- Make shared prefab changes in Prefab Mode.
- Keep unfinished and experimental objects inside test scenes.
- Do not rename layers, tags or serialized script fields without telling the group because this can break saved references.
- Check that the correct controllers and managers are enabled before testing a scene.

## Git Workflow

Create a branch for each feature or fix rather than working directly on `main`:

```text
feature/world-controller
feature/milk-throwing
feature/delivery-zones
fix/slide-standing-height
docs/player-setup
```

Commit messages should explain what was changed:

```text
Add sprint stamina UI
Implement wall jump cooldown
Document player prefab setup
Fix slide direction input
```

Before sharing work:

1. Save the scene and assets.
2. Exit Play mode.
3. Check the Unity Console for errors.
4. Review the changed files.
5. Test the feature which was changed.
6. Update the documentation and known issues.
7. Commit the related `.meta` files with the assets.

## Files That Must Not Be Committed

The `.gitignore` file already excludes most generated Unity files. Do not commit:

- `Library`
- `Temp`
- `Logs`
- `obj`
- `.vs`
- Generated `.csproj` and `.sln` files
- Personal IDE settings such as `.idea`

Unity `.meta` files must be committed. Unity uses them to keep asset references and GUIDs working for the rest of the group.

## What Counts as Complete

A feature is ready to share with the group when:

- It works in its intended scene.
- It has been tested.
- It does not create Console errors.
- Its Inspector references are assigned.
- Any required layers, tags and prefabs are documented.
- Another programmer can follow its setup instructions.
- Any remaining problems are recorded in `KnownIssues.md`.
