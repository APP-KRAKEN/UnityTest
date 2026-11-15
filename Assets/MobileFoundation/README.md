# Third-person mobile foundation

Open `Assets/MobileFoundation/Scenes/MobileSandbox.unity` and press Play.

## Controls
| Action | Desktop | Mobile |
|---|---|---|
| Move | WASD / arrow keys | Lower-left joystick |
| Orbit camera | Hold right mouse and drag | Drag right side away from buttons |
| Jump | Space | Jump button |
| Sprint | Hold Shift | Hold Sprint |
| Pause | Escape | Pause button |
| Respawn | Pause > Reset Player | Pause > Reset Player |

Reset preserves the pause state; press Resume afterwards. Falling below -12 respawns automatically. Loss of application focus pauses and releases held input. Move, look and jump have independent finger ownership. Mouse clicks can also exercise the touch controls.

## Architecture
- `Scripts/Input/PlayerInputAdapter.cs`: callback-driven New Input System, isolated action asset instance per player, desktop/touch state, cancellation, pause requests.
- `Scripts/Player/ThirdPersonMotor.cs`: CharacterController, camera-relative analog movement, acceleration, sprint, turning, gravity, jump buffer/coyote time and reset.
- `Scripts/Camera/ThirdPersonOrbitCamera.cs`: smooth orbit/follow, pitch clamps and environment-only sphere-cast obstruction.
- `Scripts/UI/`: pointer-owned joystick/look/buttons, input release on cancellation and safe-area adaptation.
- `Scripts/Game/SandboxSession.cs`: pause, reset/kill-height respawn, app lifecycle, target frame rate and scene UI links.
- `Prefabs/Player.prefab`: controller root, replaceable `Visual` child, camera target. No rig or Animator required.
- `Prefabs/MobileHUD.prefab`: uGUI Canvas with TMP labels. Player input is supplied by scene overrides; pause/resume/reset buttons are connected by SandboxSession.
- `Input/MobileFoundation.inputactions`: template copy. Original project action asset remains unchanged. Look uses Mouse delta only and is right-button gated, so joystick touches cannot rotate the desktop camera.

Player is on Ignore Raycast (layer 2); environment is Default (layer 0). Camera obstruction mask includes environment only. Maintain this separation when adding content. Do not add a Rigidbody to this CharacterController player.

### Tunables
Select Player > ThirdPersonMotor: movement/sprint speed, acceleration, turn speed, jump height, gravity, coyote time and jump buffering. CharacterController exposes step offset, slope limit, capsule size and skin width. Select Main Camera > ThirdPersonOrbitCamera: distance, sensitivity, pitch limits, collision radius/mask and smoothing. Select SandboxSession: kill height and FPS target. The 60 FPS setting is a goal, not a guarantee on unspecified hardware.

To replace the capsule, change only the `Visual` child's artwork and retain the controller root, camera target and references. To instantiate another player/HUD pair in a new scene, assign motor camera heading, camera target/input, each touch control's input field and all SandboxSession fields. InputSystemUIInputModule uses persistent UI action references from the foundation action asset.

## Android landscape preparation
- New Input System is already active; no package or compatibility-define changes were made.
- Both landscape rotations enabled; portrait rotations disabled.
- Existing Android default quality is Mobile, using the existing Mobile_RPAsset. Desktop PC quality is preserved.
- Existing ARM64 and application/signing settings are preserved.
- **This editor does not have Android Build Support installed.** A usable native Android build profile/APK cannot be validated here. No placeholder profile is left behind and the active platform was not silently switched.

After installing **Android Build Support**, SDK/NDK and OpenJDK for this exact editor version through Unity Hub:
1. Open File > Build Profiles, add Android and create an `AndroidSandbox` profile.
2. Enable scene-list override, with only `Assets/MobileFoundation/Scenes/MobileSandbox.unity` enabled.
3. Use the existing Mobile quality tier. Choose ARM64/IL2CPP when the backend toolchain is available. Set your own production application ID and signing settings before release.
4. Switch to this profile and build a Development APK. Install on a real device with USB debugging enabled.
5. Check concurrent move/look/jump, notches/system gestures, rotation, background/resume, frame time and thermal stability.

No actual device or Android APK verification has been performed. See `Tests/Verification.md` for completed checks and remaining manual coverage.

## Scope
Movement sandbox only: ground, ramp, stairs, jump platforms and camera obstruction walls. No combat, health, inventory, persistence, AI, multiplayer, monetization or custom generated assets. Original SampleScene, shared URP profiles and package source are preserved. TMP essential font resources were imported non-interactively for the new HUD.
