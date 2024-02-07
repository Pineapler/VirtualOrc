# WIP

- [ ] Controller laser interaction
- [ ] Move full body massage targets UI to world space
  - These use a single canvas, probably need to do perspective math or something
- [ ] Maybe fix screen space raycasting? See InputManager.cs
- [ ] Patch out WaterFiller (null ref from zibra liquids)

# Backlog

- [ ] Change raycast mechanism to use controller
- [ ] Hook the SteamVR recentre event and apply our own `VrRig().Recentre`. Also do Y rotation.
- [ ] Fix LogSystemManager properly. Currently disabled the UI so that it's not cluttering the screen.
- [ ] Replace the missing fluid sim water with a vr-friendly alternative

# Complete

- [X] Automatic SteamVR injection
- [X] VR camera rig that follows the vanilla camera
- [X] Patch out features that would crash VR (Zibra Liquid)
- [X] World space UI elements use an overlay camera
- [X] Move main menu canvas to world space (No interaction)

# Other mods?

- [ ] SkipCutscenes
  - Intro skip already implemented as part of this mod, could be migrated
  - Add option to auto skip "perfect" cutscene
  - Add option to auto skip story cutscenes? idk, I like these but some people might want the option.
- [ ] SetGirlCostume 
  - Currently costume is set in `VipMassageGameSystem.CheckCostume()` based on heart level.
  - Instead have these as unlocked costumes and let the player choose on massage begin.

## Notes

- OrcTouchingHand.cs has cam/mouse raycast logic
- Buttons have event handlers on PointerEnter