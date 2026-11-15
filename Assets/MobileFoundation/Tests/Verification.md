# Verification record

## Passed
- Every foundation C# file compiled successfully after final edits; no missing scene scripts or missing serialized foundation references found.
- Exactly one Camera, AudioListener and EventSystem. CharacterController height 2, radius 0.35, step offset 0.3, slope limit 45, player layer 2. Environment mask excludes player.
- Saved scene/action references checked on disk. UI action references are scene-serialized InputActionReference objects pointing to the dedicated persistent action asset (not the original template).
- Multi-angle scene inspection: player, ground, ramp, staircase, landings, jump platforms and camera corner present; scales and spacing consistent for the graybox.
- Final runtime attempt: player grounded at spawn (0, 0.04, -12); analog cardinal speed 4.00 units/s; normalized diagonal speed 3.94 units/s (measured against simulation time).
- An earlier runtime attempt completed checks for pause freezing player position, reset while timeScale=0, resume button, independent move/look/jump pointer callbacks, canceled pointer ownership, camera yaw response (30 degrees), corner obstruction retraction (3.60 units), kill-height respawn, and held-sprint cancellation.
- Static joystick centering after correction: center emits (0,0), 90-unit right displacement emits (1,0), matching handle movement regardless of RectTransform pivot.
- Original domain-reload-disabled setting restored after user-approved temporary testing. Editor is out of Play Mode; timeScale=1. Temporary test scripts removed.

## Verification limitations / failures
Three runtime attempts were made; the complete test suite did **not** pass end to end.
1. Initial tests started too early, before initialization/frame settling, producing invalid duration comparisons.
2. Frame-synchronized diagnostic run confirmed the editor was stalling: a wall-clock second contained 0.59 seconds of simulation in one segment and 1.32 seconds in another. Test measurements were corrected to simulation time. The diagnostic timed out during pause testing.
3. Final run passed grounding/cardinal/diagonal movement and then timed out at the sprint stage. Stopped retrying rather than increasing timeouts or changing timeScale.

The jump test revealed coarse-frame gravity integration reduced observed height. Motor displacement was corrected to v*dt + 0.5*g*dt^2. **That correction has compiled but has not been verified by a completed runtime jump test.** Joystick pivot centering was corrected afterwards and statically checked; no further runtime pass was performed.

Sprint, jump buffering/coyote boundaries, slopes/steps, ceiling collisions and low/high frame-rate camera behavior require additional manual play testing. Pointer callback checks establish ownership logic, not a full hardware multi-touch/UI-module integration test. App focus/background behavior requires device testing.

## UI previews
- `HUD_1280x720.png`, `HUD_1600x720.png`, `HUD_1024x768.png`: edit-mode camera/Canvas render previews. Temporary camera/Canvas changes were restored.
- `Gameplay.png`, `Paused.png`: runtime captures from automated attempts.

Image read-back is incompatible with the editor's new EntityId identifiers in this environment, so these files were generated but **not visually reviewed by the assistant**. Review them in Unity and check real notch safe areas, tablet sizes and simultaneous control interactions.

## Android
Android Build Support is not installed in this editor. No APK or usable Android native build profile could be verified. Landscape-only PlayerSettings and the pre-existing Android Mobile quality mapping are ready; see README for profile creation steps after installing the module. No active platform switch, signing changes, package installation or compatibility-guard modifications were made.

## Manual checklist before extending gameplay
- Play the sandbox with WASD/arrows, Shift, Space, RMB drag and Escape.
- Verify joystick neutral center, proportional input, cancellation, and simultaneous move/look/jump.
- Confirm jump height and buffering/coyote timing after the integration fix, plus slopes/stairs/walls/ceilings.
- Reset on ground/in air/while paused; walk off the edge and verify respawn.
- Check camera against corner walls and platform edges at 30/60/120 FPS.
- Inspect HUD preview images, tall landscape phones, tablets, notches and gesture regions.
- Install Android tooling, create the documented build profile, build and test on a physical Android device.
- Measure frame time, managed allocations, background/resume and thermal behavior on target hardware. 60 FPS remains a target, not a measured guarantee.
