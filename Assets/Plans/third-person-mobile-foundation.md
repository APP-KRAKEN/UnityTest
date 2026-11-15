# Project Overview
- **Game Title:** Third Person Mobile Foundation (working prototype name, not a final product title).
- **High-Level Concept:** A reusable third-person movement sandbox for Android. A capsule player explores a compact graybox obstacle course using camera-relative movement, sprinting, jumping, and a touch-controlled orbit camera. The goal is to establish reliable controls and modular systems before adding a genre-specific game.
- **Players:** Single player, offline.
- **Inspiration / Reference Games:** No specific reference requested; conventional two-thumb third-person mobile controls.
- **Tone / Art Direction:** Originally clean graybox. For the approved foundation's terrain revision, the user selected a grassy meadow and naturalized course: gentle rolling ground, grass/dirt surface blends, and rock ledges/slopes replacing artificial course shapes. Preserve the capsule, controls, and HUD. No purchased assets or character generation required.
- **Target Platform:** Android first. Current editor target is Windows Standalone; retain desktop testing and prepare an Android build profile. Android build execution requires installed Android Build Support, SDK, NDK, and JDK.
- **Screen Orientation / Resolution:** Landscape only, both landscape rotations. UI reference resolution 1920x1080 with responsive anchors and safe-area accommodation; native device rendering rather than a fixed physical resolution.
- **Render Pipeline:** Existing URP 17.6.0 on Unity 6000.6.9f1. Existing Mobile quality level already selects Mobile_RPAsset for Android; preserve the PC pipeline for desktop.

## Decisions and implementation approach
User selected movement sandbox, Android landscape, and clean graybox.
- **Recommended architecture:** CharacterController locomotion, a small custom follow/orbit camera, and uGUI touch controls backed by the installed New Input System. No additional packages needed; straightforward to tune and extend.
- **Alternative considered:** Rigidbody locomotion plus a package-based camera. Better suited to physics-heavy play and complex camera transitions, but adds dependencies and tuning unnecessary for this foundation.
- Movement/input, camera, UI, and session state will be separate components. The capsule is a replaceable visual child, not a dependency of movement logic.

# Game Mechanics
## Core Gameplay Loop
Move through the course, adjust the camera, sprint across open space, jump onto platforms, negotiate slopes and steps, and reset to try again. Immediate spatial feedback supports mastery and competence (SDT); mechanics remain intentionally small so movement feel can be iterated before content expands.
- Analog walk/run movement, acceleration/deceleration, smooth facing toward movement, and normalized diagonal speed.
- Grounding, gravity, configurable jump height, modest jump buffering and coyote time.
- Inspector-exposed move speed, sprint speed, rotation speed, slope limit, step offset, and gravity.
- A spawn point, manual reset, and automatic respawn below a kill height. Respawn clears vertical velocity and input transients and resets camera obstruction/smoothing state.
- Pause/resume freezes gameplay while keeping UI responsive. App backgrounding pauses and clears held inputs.
- Exclusions: combat, inventory, collectibles, enemy AI, networking, backend progression, saves, monetization, custom character generation, audio production, and animation systems.

## Controls and Input Methods
- **Mobile:** Fixed lower-left analog joystick; drag the unobstructed right-hand region to orbit; lower-right Jump and hold-to-Sprint buttons; top-right Pause.
- Track pointer ownership independently for joystick, camera drag, and buttons. Support moving, looking, and jumping together without one finger stealing another control. Decorative UI must not intercept raycasts.
- **Desktop testing:** WASD/arrows move, hold Shift to sprint, Space jumps, right-mouse drag looks, Escape pauses. Desktop clicking the touch controls is also supported.
- Use InputAction callbacks with UnityEngine.InputSystem, not legacy Input APIs. Custom touch controls emit events through the EventSystem using InputSystemUIInputModule and feed the same input adapter as desktop actions.
- Clone the template input asset into a dedicated foundation asset; leave Assets/InputSystem_Actions.inputactions unchanged. Restrict desktop look to Mouse delta with right-button gating: the template's generic Pointer delta must not rotate the camera from joystick touch motion. UI touch position/click bindings remain multi-pointer compatible.
- Enable/disable and unsubscribe actions consistently. Clear input and pointer ownership on cancellation, disable, pause, focus loss, and respawn; require a fresh touch after resume.
- Smooth camera follow in LateUpdate, configurable yaw/pitch sensitivity, pitch clamps, and sphere-cast obstruction against environment only. Retract to avoid walls, recover smoothly, and exclude the player and triggers. Separate per-frame pointer displacement scaling from any rate-based input to avoid frame-rate-dependent look.

# UI
Use uGUI: installed, no existing runtime UI to preserve, and suitable for multi-touch controls. Build a screen-space overlay Canvas with CanvasScaler, GraphicRaycaster, one EventSystem, and InputSystemUIInputModule.

```text
LANDSCAPE HUD (inside safe area)
+---------------------------------------------------------+
| Move • Look • Jump                            [Pause]   |
|                                                         |
|                   Third-person view                     |
|                                 Right-side look region  |
|   ( Move joystick )                    [Sprint] [Jump]  |
+---------------------------------------------------------+

PAUSED
+---------------------------------------------------------+
|                      Paused                             |
|                     [Resume]                            |
|                  [Reset player]                         |
+---------------------------------------------------------+
```
- Large thumb targets, high-contrast labels, restrained translucent backgrounds, and consistent padding.
- Camera drag zone sits behind action buttons so Jump/Sprint/Pause touches do not move the camera. Pause overlay blocks all gameplay input.
- Safe-area script responds to resolution/orientation/safe-area changes. Test 16:9, tall landscape phones, and tablets; avoid notch/system-gesture regions.
- Use a simple project-compatible font without introducing a new font workflow. HUD displays brief control hints; no irrelevant health bars or inventory.

# Key Asset & Context
## Verified project state
- Assets/Scenes/SampleScene.unity contains Main Camera, Directional Light, and Global Volume only; no player, environment, or runtime UI.
- Assets/InputSystem_Actions.inputactions has Player Move, Look, Jump, Sprint and other actions, plus a UI map. Generic Pointer/delta is bound to Look and requires isolation in the foundation copy.
- Input backend is New Input System only (activeInputHandler = 1), package 1.20.1. uGUI is 2.6.0. Cinemachine and Test Framework are not declared in the inspected manifest.
- Assets/Settings/Mobile_RPAsset.asset and Mobile_Renderer.asset exist. Android default quality is Mobile; PC default quality is PC. Current PlayerSettings allow portrait rotation.
- No gameplay scripts or player prefabs found. Existing C# files are tutorial/readme editor code.
- Scene/input forward dependencies inspected through AssetDatabase.GetDependencies because the dedicated dependency tool is unavailable. Before implementation, refresh forward/reverse impact checks for every existing asset or setting to be changed. Do not edit package source or add compatibility defines.

## Planned files
All foundation-specific assets under Assets/MobileFoundation/ unless noted:
- Scenes/MobileSandbox.unity: new authored scene; preserve SampleScene.unity.
- Input/MobileFoundation.inputactions: template copy tailored to Move/Look/Jump/Sprint/Pause and UI use.
- Scripts/Input/PlayerInputAdapter.cs: caches action state and merges UI events; source ownership and reset contract.
- Scripts/Player/ThirdPersonMotor.cs: CharacterController motion, grounding/jumping, replaceable visual reference.
- Scripts/Camera/ThirdPersonOrbitCamera.cs: target follow, orbit, pitch clamp, collision avoidance, reset.
- Scripts/UI/MobileJoystick.cs, TouchLookArea.cs, HoldInputButton.cs: pointer-aware event controls.
- Scripts/UI/SafeAreaPanel.cs: responsive safe-area layout.
- Scripts/Game/SandboxSession.cs: pause, resume, respawn, app lifecycle, configurable target frame rate.
- Prefabs/Player.prefab and MobileHUD.prefab: serialized reusable components and references.
- Materials/: small set of URP-compatible graybox materials.
- README.md: controls, scene entry point, tuning fields, architecture, Android prerequisites, extension points, and test checklist.
- ProjectSettings/PlayerSettings: landscape rotation settings only as needed; preserve identifiers/signing settings.
- Android build profile asset (path determined using installed Unity build-profile workflow during implementation): sandbox scene and existing Mobile quality setup. Preserve existing build configuration where possible; no silent platform switch or SDK install.

# Implementation Steps
1. **Preflight and dependency audit.** Refresh scene/input/pipeline/settings state and identify reverse dependents before edits. Confirm Android module availability and whether test packages already resolve. Preserve template assets and package sources. **Dependencies:** none.
2. **Input foundation.** Create the dedicated input asset and PlayerInputAdapter, callback lifecycle, UI-facing event methods, desktop bindings, and pause action. Remove unintended touch-to-desktop look routing from the copy. **Dependencies:** Step 1.
3. **Player prefab.** Implement ThirdPersonMotor and create CharacterController root with capsule visual child, directional marker, and camera target. Configure acceleration, turning, jump/gravity, slope/step limits and reset behavior. No Rigidbody or Animator required. **Dependencies:** Step 2.
4. **Camera.** Implement ThirdPersonOrbitCamera and wire follow target/input. Add pitch limits, environment-only obstruction mask, player exclusion, and instant reset. Validate collision/follow before visual polish. **Dependencies:** Steps 2–3.
5. **Touch HUD.** Create MobileHUD prefab and pointer-aware controls, safe area, Canvas scaling, input module, and pause overlay. All controls feed the shared adapter. Maintain independent fingers and complete cancellation behavior. **Dependencies:** Steps 2–4.
6. **Sandbox and session.** Author MobileSandbox scene with ground, ramp, short staircase, jump platforms, wall/camera test section, spawn point, player, camera, HUD, and SandboxSession. Use simple colliders/materials and conservative existing URP defaults. Do not change shared Volume profiles, lighting assets, or PC render settings. Wire pause/reset/background handling and 60 FPS target (aspirational until device measurement). **Dependencies:** Steps 3–5.
7. **Android preparation and documentation.** Configure landscape-only orientation and a sandbox Android build profile using existing Mobile quality. Retain ARM64; verify backend prerequisites before building. Do not overwrite signing credentials or invent a production application ID. Add README and completed/deferred verification results. **Dependencies:** Steps 1, 6.
8. **Validation and handoff.** Check serialized references and Console, run desktop/available automated checks, capture scene and HUD views, and perform Android development build/device checks where tooling is available. Report missing SDK/device prerequisites rather than claiming device success. **Dependencies:** Steps 1–7.

# Verification & Testing
- Compilation: no new errors; one active camera/audio listener and EventSystem, all serialized links assigned, correct input module and action references.
- Movement: cardinal/diagonal speed parity; joystick magnitude scales speed; smooth starts/stops; sprint release; slopes/steps near configured limits; stable grounding; jump buffer/coyote boundaries; no airborne double-jump; wall/ceiling collisions.
- Camera: 360-degree yaw, pitch clamps, stable follow at 30/60/120 FPS, no self-collision, wall retraction/recovery, reset without sweeping from old location.
- Touch: two-finger move+look and three-finger move+look+jump; finger release outside a control; touch cancel/focus loss; UI presses never orbit; no stuck movement after pause/resume or app backgrounding.
- Session: reset on ground/in air/while paused, kill-height respawn, velocity cleared, pause menu remains usable at timeScale zero, timeScale restored correctly on teardown.
- UI: safe-area/notch compliance, no overlapping joystick/buttons at phone/tablet aspect ratios, readable labels, desktop mouse simulation and action bindings.
- Automated checks, if the Test Framework is already available: input state reset/normalization, jump eligibility timing, and pause/respawn state checks. Otherwise use a documented manual checklist; adding a package requires separate dependency approval.
- Android: verify module/SDK availability, create development APK if available, check orientation and concurrent touches on an actual device. Measure frame time after warmup and repeated resets; check for recurring managed allocations and excessive thermal slowdown. 60 FPS is a target, not a promised result for unspecified hardware.
- Completion criteria: scene playable with desktop controls, functioning touch HUD and modular prefabs, pause/reset/respawn verified, Android landscape preparation complete, and any unperformed device/build tests explicitly documented.

# Terrain Redesign Revision

## Project Overview
- **Game Title:** Third Person Mobile Foundation (unchanged).
- **High-Level Concept:** Turn the existing movement sandbox into a compact grassy meadow with rolling ground and natural rock traversal features. Retain the movement foundation while making the environment feel organic rather than a grid-covered test slab.
- **Players:** Single player, offline.
- **Inspiration / Reference Games:** No specific reference requested; natural meadow terrain with readable third-person traversal.
- **Tone / Art Direction:** User selected grassy meadow and naturalized course. Muted greens, warm worn dirt, irregular gray/brown rocks, gentle hills; stylized natural scenery consistent with the simple player.
- **Target Platform:** Android first with existing Windows desktop testing retained.
- **Screen Orientation / Resolution:** Existing landscape, responsive 1920x1080 reference HUD; unchanged.
- **Render Pipeline:** Existing URP 17.6.0, Unity 6000.6.9f1; preserve Mobile and PC render settings.

## Game Mechanics
### Core Gameplay Loop
Explore the meadow, follow worn paths, sprint across clearings, ascend a natural slope, climb shallow rock terraces, and jump across rock ledges. Preserve readable traversal affordances and attainable challenges (SDT competence), while organic shapes and surface variation strengthen the exploration aesthetic (MDA).
- Keep the approximate arrangement of the current slope at x=-8, terraces near x=0, and jumping route near x=7. Replace their artificial appearance rather than introducing a new gameplay system.
- Replace CameraWall/CameraCorner with a natural rock outcrop that still exercises camera obstruction.
- Maintain a safe flat spawn clearing near (0, 0.08, -12), with ground at y=0 there. Preserve reset and fall-respawn behavior.
### Controls and Input Methods
Unchanged: existing New Input System adapter, CharacterController, desktop WASD/Shift/Space/right-mouse and mobile joystick/look/Jump/Sprint/Pause. No input or player code edits planned.

## UI
No UI modifications. Existing landscape HUD remains:
```text
[Move / Look / Jump hint]                         [Pause]
                  Meadow + player view
[Joystick]                              [Sprint] [Jump]
```
Keep routes readable through camera framing and dirt contrast, not additional labels or markers.

## Key Asset & Context
### Verified context
- Loaded scene: Assets/MobileFoundation/Scenes/MobileSandbox.unity. Current Ground is a 40x40 cube slab at (0,-0.5,0), top at y=0, with BoxCollider. Seven thin GroundMark objects create artificial parallel lines.
- No TerrainData or suitable meadow texture assets found. Existing foundation materials use Universal Render Pipeline/Lit.
- CharacterController: slope limit 45 degrees, step offset 0.3m, radius 0.35m, height 2m, skin 0.04m. ThirdPersonMotor defaults: jump height 1.35m, movement 4m/s, sprint 7m/s. Keep ordinary paths well below the slope limit and terrace rises at or below the current 0.24m step increment.
- ThirdPersonOrbitCamera sphere-casts against environment; new terrain and rock colliders must be included in its existing mask. SandboxSession resets to its serialized SpawnPoint and falls below kill height -12m.
- Scene dependencies were inspected. Preserve player/HUD prefabs, action asset, runtime scripts, shared graybox materials, and render pipeline assets. Refresh impact analysis before editing scene/README or any other existing asset.
### Approaches considered
1. **Recommended: native Unity Terrain plus reusable rock meshes.** Editable heights, blended terrain layers and TerrainCollider make later sculpting straightforward. Moderate setup cost, but appropriate for natural rolling land; use conservative resolutions and sparse decoration for Android.
2. **Alternative: custom sculpted mesh ground with MeshCollider.** Compact and good for a deliberately low-poly style; less convenient terrain painting/sculpting and requires more bespoke surface tooling. Not the default for this meadow revision.
### Assets to create or modify
- Modify Scenes/MobileSandbox.unity: replace slab/grid and artificial course geometry with meadow terrain and rock features; keep player, spawn, camera, HUD, EventSystem, session, and existing lighting connected.
- Create Environment/MeadowTerrain.asset (TerrainData), TerrainLayers/Grass.terrainlayer, Dirt.terrainlayer, Rock.terrainlayer, and Materials/MeadowTerrain.mat using an installed URP-compatible terrain shader.
- Create Textures/Terrain/Grass_Albedo.png, Dirt_Albedo.png, Rock_Albedo.png and appropriate normal maps if needed. Use seamless generated textures; no purchases, imports, or packages without separate permission.
- Create Environment/Meshes/ and Materials/NaturalRock.mat for a small reusable set of irregular rock meshes with simple matching colliders. Preserve flat usable landing tops while breaking up side silhouettes.
- Create Scripts/Editor/MeadowTerrainBuilder.cs if an authoring helper is useful: deterministic, editor-only, scoped to the meadow environment, idempotent and Undo-aware. Bake all geometry and textures to assets; no runtime terrain generation. Use an Editor folder rather than adding compatibility defines.
- Update README.md with terrain editing notes and actual verification results. Save captures under Tests/ where supported.

## Implementation Steps
1. **Preflight and baseline.** Reinspect active scene, dependency/dependent impact, serialized motor/camera values, environment layers, and available URP terrain shader. Capture baseline views. Keep modifications scoped and preserve original shared assets. **Dependencies:** none.
2. **Create meadow assets.** Generate seamless grass/dirt/rock textures, terrain layers, terrain material, and deterministic TerrainData. Start with a single roughly 60x60m terrain tile centered around the existing course, 257 heightmap resolution and 256 alphamap resolution; gentle height variation, smooth perimeter hills and a level spawn/course clearing. Terrain minimum must permit y=0 at spawn; hills must not cover traversal features. No expensive dense grass or forest. **Dependencies:** Step 1.
3. **Replace ground and paint routes.** Add Terrain and matching TerrainCollider on the camera's environment layer. Remove/disable Ground and GroundMark only once replacement collision is valid. Blend grass/dirt irregularly, paint a winding worn route from spawn to traversal features and stone onto steeper areas; avoid uniform noise, obvious tiling, and ruler-straight boundaries. **Dependencies:** Step 2.
4. **Naturalize the course.** Replace Ramp_15deg/RampLanding with a gentle stone/earth ascent; Step_1 through Step_6/StairLanding with irregular rock terraces; JumpPlatform_1 through JumpPlatform_3 with broad rock ledges; CameraWall/CameraCorner with an irregular outcrop. Preserve attainable landing heights/gaps and camera-test coverage; match colliders to walkable surfaces, keep sharp decoration off the main routes. Adjust local terrain under features to prevent gaps, sinking, or overlap. **Dependencies:** Step 3.
5. **Composition and integration.** Add a restrained scattering of rocks around—not across—the paths; vary size/rotation without excessive draw calls. Confirm spawn clearance, terrain edges, lighting readability, camera obstruction and serialized links. Keep existing lighting and Volume settings unless a separate adjustment is approved. Bake and save all assets/scene. **Dependencies:** Steps 3–4.
6. **Validate and document.** Check compilation, run traversal/respawn/camera tests, capture multi-angle scene and player-camera views, and inspect for floating/sunken objects, overlap, incorrect scale, and unnatural surface repetition. Revise the same authoring implementation for at most three capture/fix rounds. Update README with completed checks and explicitly deferred Android measurements. **Dependencies:** Steps 1–5.

## Verification & Testing
- Scene reopens with saved TerrainData/layers/textures/meshes; no missing references, pink shaders, runtime generator dependency, or Console errors.
- Visual: organic rolling silhouette, seamless grass/dirt transition, coherent scale, no grid/slab remains visible, paths and rock landings readable from player height. Inspect isometric/front/top/right and gameplay views.
- Collision: raycasts agree with visible ground/ledge tops; no hidden slab collider, floating rocks, embedded landings, snagging terrace edges or inaccessible path slopes. Terrain and solid rocks participate in camera obstruction; decorative noncolliding details do not create invisible walls.
- Traversal: spawn settles at y=0 clearing without penetration; walk/sprint across meadow, ascend slope/terraces, complete ledge jumps with existing controller values; camera retracts/recovers at outcrop and nearby hills.
- Session: reset from a hill, ledge, midair and pause returns to safe spawn; walking off terrain can still reach kill-height respawn. Existing input, pause, and HUD continue to work.
- Performance: keep single terrain tile, modest texture sizes (initially 512 per layer), shared rock meshes/materials, and sparse decoration. Compare desktop render stats against baseline; actual Android frame-time and thermal performance require device validation and must not be claimed without testing.
- Scope: no changes to shared player/HUD/input/runtime movement scripts, project/package compatibility defines, package sources, pipeline profiles, or build settings unless a demonstrated issue is separately approved.
