# Project Overview
- Game Title: MobileSandbox (scene name; final title unspecified and unchanged).
- High-Level Concept: Existing third-person mobile movement sandbox with ramps, stairs, jump platforms and camera collision obstacles. This task repairs deleted gameplay and HUD assets, not game design or character art.
- Players: Existing single-player sandbox; no networking changes.
- Inspiration / Reference Games: Not specified; not relevant to this repair.
- Tone / Art Direction: Preserve existing capsule player, environment and dark/teal HUD.
- Target Platform: iOS; retain desktop controls for Editor testing.
- Screen Orientation / Resolution: Existing HUD reference resolution 1920x1080; preserve orientation settings and verify landscape/device safe areas.
- Render Pipeline: Built-in, as currently configured.

# Game Mechanics
## Core Gameplay Loop
Preserve movement through the existing obstacle sandbox: camera-relative walking/sprinting, jumping onto platforms, orbiting the camera and resetting after falling. Reconstruct original intent from serialized settings; exact original implementation is unavailable.
## Controls and Input Methods
- Project input backend verified as Both (activeInputHandler=2). Keep that setting; implement gameplay with event-driven UnityEngine.InputSystem InputActions, not legacy Input polling.
- Desktop: WASD movement, RMB-held mouse-delta look, Space jump, Shift sprint. Bind pause to Escape if consistent with the reconstructed action map.
- Mobile: existing bottom-left joystick, right-side drag-to-look region, Jump button, hold-to-sprint button and Pause/Resume/Reset buttons.
- Separate touch and desktop state, clamp movement magnitude, consume jump presses/look deltas once, and clear held states when paused, disabled, focus is lost or a touch ends/cancels. Independent pointer ownership must support simultaneous move/look/button input.

# UI
Preserve the Canvas-based hierarchy and visual layout; do not destroy/rebuild the HUD.
```
MobileHUD (Overlay Canvas, CanvasScaler 1920x1080, GraphicRaycaster)
└── SafeArea
    ├── LookArea (right-side drag surface, behind buttons)
    ├── ControlHints                         Pause [top-right]
    ├── MoveJoystick/Handle [bottom-left]
    ├── Sprint                         Jump [bottom-right]
    └── PauseOverlay (hidden during gameplay)
        ├── Title
        ├── Resume
        └── Reset
```
Repair missing safe-area behavior and font references. Preserve anchors, colors, dimensions and sibling order unless a verified input-blocking defect requires a targeted change. Preserve one EventSystem with InputSystemUIInputModule. Empty persistent button callbacks are not alone evidence of a defect: reconstruct SandboxSession runtime listeners with symmetric registration/removal.

# Key Asset & Context
## Confirmed findings
- Assets/MobileFoundation/Scenes/MobileSandbox.unity survives with serialized gameplay settings and broken script/asset GUIDs.
- Assets/MobileFoundation/test.cs is the only remaining project C# script and contains no gameplay logic. Leave it unchanged.
- Player has a CharacterController and two missing scripts: MobileFoundation.PlayerInputAdapter and ThirdPersonMotor.
- HUD has missing SafeAreaPanel, MobileJoystick, TouchLookArea and HoldInputButton components (Jump command=0, Sprint command=1).
- Main Camera has missing ThirdPersonOrbitCamera; SandboxSession is missing.
- InputActions asset GUID 965ad3a278b5a724f99282680547da8e does not resolve. Missing player/HUD prefab GUIDs also do not resolve; preserve surviving scene contents.
- HUD TextMeshPro components have missing fonts/materials.
- URP additional camera/light scripts are missing while the project uses Built-in. Remove only these confirmed orphaned package components if they cannot resolve in the actual installed package configuration; do not change pipelines or package source.
- Console initially had no project compile errors, only Input Manager deprecation and unrelated assistant account warnings. Missing scripts are independently confirmed by hierarchy inspection.
- Local checks found no recovery files in GeneratedAssets/Temp, no .git directory and no Unity Version Control workspace.

## Reconstructed files
Create separate matching MonoBehaviour files under Assets/MobileFoundation/Scripts/:
- PlayerInputAdapter.cs: action asset lifecycle, desktop event callbacks, touch injection, blocked-input state, one-shot jump/look consumption.
- ThirdPersonMotor.cs: cache CharacterController and input on Awake; camera-relative motion, acceleration, visual facing, gravity, grounded jump, coyote time and jump buffer, reset method.
- ThirdPersonOrbitCamera.cs: target/input references, orbit pitch limits, smoothing, collision sphere cast against environment mask and reset/snap method.
- MobileJoystick.cs: pointer-owned drag conversion using actual RectTransform center (existing joystick pivot is bottom-left), radius/deadzone, handle feedback and release/reset.
- TouchLookArea.cs: pointer-owned drag delta normalized to reference height, ignore unrelated pointer releases, clear on interruptions.
- HoldInputButton.cs: serialized enum values Jump=0/Sprint=1, correct press/release state, original graphic colors and focus/disable cleanup.
- SafeAreaPanel.cs: cache own RectTransform; apply Screen.safeArea normalized anchors; refresh on size/safe-area changes without division by zero.
- SandboxSession.cs: explicit player/camera/spawn/HUD links, pause/resume/reset listeners, kill-height reset and lifecycle cleanup of timescale/frame-rate overrides.
- Assets/MobileFoundation/Input/MobileSandbox.inputactions: Player actions and UI actions with mouse/touch pointer bindings, repaired action references in the UI module.
- TMP font resources: reuse a valid installed font asset if available; otherwise import standard TMP essentials noninteractively and assign a valid font/shared material to existing HUD labels. No typography redesign.
- Modify MobileSandbox.unity to restore missing component links and explicit references; retain original tuning, objects and serialized values.
- Missing prefab source links: inspect actual prefab status. Prefer preserving scene objects and restoring original script GUID identity where feasible. If scene instances cannot be saved reliably because source prefabs are deleted, unpack only the affected orphaned instances after preserving component data, and repair in place. Do not fabricate source-prefab identity or rebuild entire hierarchies.

## Identity and impact safety
Before edits, repeat dependency/impact checks using the available dependency query tools (or AssetDatabase.GetDependencies and a read-only reverse-reference scan if the tool is unavailable). Capture missing component GUIDs, fileIDs and original field values before changing references. Prefer recreating script assets with original GUIDs when no collision exists and identity can be maintained safely; otherwise replace only m_Script references and preserve existing component IDs/data. Never bulk-remove all missing scripts: that would discard the saved configuration.

# Implementation Steps
1. **Revalidate and preserve repair context.** Inspect current scene, package manifest/versions, input settings, missing script GUIDs and orphaned prefab state; capture values and dependency impacts. Confirm no original source has reappeared. Dependency: approval of this plan.
2. **Reconstruct the gameplay API and InputActions.** Create PlayerInputAdapter and matching action asset using installed Input System APIs. Validate action names/types, desktop binding callbacks, touch-state merge, blocked state and cleanup. UI actions must include independent touch point/press support. Dependency: Step 1.
3. **Restore motor and camera.** Implement ThirdPersonMotor and ThirdPersonOrbitCamera using existing CharacterController/camera references. Preserve motor speed 4, sprint 7, acceleration 20, turn speed 720, jump height 1.35, gravity -25, ground stick -2, coyote/buffer 0.12. Preserve camera distance 5.5, sensitivity 0.12, pitch -25..65, initial pitch 18, smooth time 0.06, recovery 8, collision radius 0.2 and padding 0.08. Dependency: Step 2. Verify compilation before wiring.
4. **Restore mobile controls and safe area.** Implement joystick, touch look, hold buttons and safe-area components against the adapter API; preserve joystick radius 90/deadzone 0.08 and look reference height 1080. Verify each control individually, then simultaneous pointers. Dependency: Step 2.
5. **Restore SandboxSession.** Reconnect pause/resume/reset, kill height -12, spawn at existing SpawnPoint and target frame rate 60. Pause blocks/clears gameplay input while leaving UI active; resume must not revive stale inputs. Reset clears motor/input and snaps camera; restore global overrides on teardown. Dependency: Steps 3–4.
6. **Repair scene wiring incrementally.** Restore each missing script link while preserving serialized IDs/data. Set player, camera, joystick, handle, button, session and action references. Resolve orphaned prefab links only if necessary for reliable saving. Verify one enabled EventSystem/InputSystemUIInputModule, repaired UI actions and raycast targeting. Restore TMP font/material references. Remove only confirmed unused orphaned URP extra-data components after inspecting their class identifiers. Dependency: Steps 2–5. Save/reopen scene and verify persistence.
7. **Run functional verification and report remaining unrelated issues.** Test desktop, mobile multitouch, pause/reset and lifecycle; inspect console for new errors. Report absent environment materials separately unless they block the player/HUD verification; do not expand into an environment/rendering redesign. Dependency: Step 6.

# Verification & Testing
- Compilation: scripts compile on Unity 6000.6.9f1 with installed packages; no package-source changes, fake version/platform defines or compatibility-guard bypasses.
- EditMode: zero missing gameplay/HUD scripts; all required input, camera, session, controller and UI references resolve; one EventSystem; valid UI actions and TMP font assets. Preserve CharacterController height 2, radius 0.35, slope limit 45, step offset 0.3 and skin width 0.04.
- Scene persistence: save, reopen and confirm no broken script links or lost serialized tuning; no unintended object destruction.
- Desktop PlayMode: WASD camera-relative move, Shift speed increase, RMB-gated look, Space jump; diagonal speed does not exceed intended magnitude. Coyote/buffer tests and no repeated jumps from one press.
- Mobile/device: joystick analog/deadzone response, correctly centered handle despite pivot; independent simultaneous movement/look/jump/sprint; unrelated finger release does not cancel another control; cancellation/focus loss/disable leaves no stuck input.
- UI: Pause opens overlay and blocks player/camera; Resume works at timescale zero; Reset returns to spawn and resets velocity/camera without unintended pause-state changes; no duplicate button subscriptions after enable/disable cycles.
- Physics/camera: stairs/ramps remain traversable with retained controller tuning; camera avoids environment, ignores player layer and recovers distance; falling below -12 resets.
- Safe area: landscape phone/tablet aspect ratios and notch/home-indicator layouts; controls remain usable without overlap or clipping.
- Runtime lifecycle: repeated play sessions with domain reload settings inspected; dispose owned actions/clones, unsubscribe events and restore timescale/frame-rate safely.
- Console: no NullReferenceException, missing-action, invalid input-backend or missing-script errors caused by repaired player/HUD. Legacy Input Manager deprecation warning may remain because Both is deliberately preserved. Unrelated account/network warnings are not gameplay defects.
- iOS device/build verification if build environment is available; otherwise explicitly report device testing as pending, not passed.

## Scope and tradeoff
Recommended approach is an in-place reconstruction matching surviving serialized intent, which preserves layout/tuning and avoids reference cascades. Alternative is a new controller/HUD architecture or hierarchy rebuild; this is broader, risks behavior drift and is not recommended for a repair request. Exact deleted behavior cannot be guaranteed; acceptance is based on the preserved scene intent and tests above. No new character mesh, multiplayer, monetization or general rendering redesign.
