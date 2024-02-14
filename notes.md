# WIP

- [ ] Move client dialogue UI to world space
- [ ] Controller laser interaction (partial)
- [ ] Move full body massage targets UI to world space

# Backlog

### Features

- [ ] Parent orc hands to VR controllers, use these in massage games instead of laser

### Bugs 

- [ ] Fix LogSystemManager properly. Currently disabled the UI so that it's not cluttering the screen.
- [ ] Replace the missing fluid sim water with a vr-friendly alternative
- [ ] Make sure plugin behaves properly across scene reloads
- [ ] Patch out WaterFiller (null ref from zibra liquids)

### UI
 
- [ ] Move computer UI to world space

### Unity/BepInEx/Asset specific

- [ ] Hook the SteamVR recentre event and apply our own `VrRig().Recentre`. Also do Y rotation.
- [ ] Automatically copy SteamVR StreamingAssets to game directory

# Complete

- [X] Automatic SteamVR injection
- [X] VR camera rig that follows the vanilla camera
- [X] Patch out features that would crash VR (Zibra Liquid)
- [X] World space UI elements use an overlay camera
- [X] Move main menu canvas to world space
- [X] Move phone UI to world space

# Other mods?

- [ ] SkipCutscenes
  - Intro skip already implemented as part of this mod, could be migrated
  - Add option to auto skip "perfect" cutscene
  - Add option to auto skip story cutscenes? idk, I like these but some people might want the option.
- [ ] SetGirlCostume 
  - Currently costume is set in `VipMassageGameSystem.CheckCostume()` based on heart level.
  - Instead have these as unlocked costumes and let the player choose on massage begin.
  - Figure out how to get the clothes used in the built-in clothing settings

## Notes

- OrcTouchingHand.cs has cam/mouse raycast logic
- RaycastSystem.cs
- BodyStateRenderCam.cs
- MassageIndicator.cs
- TrainingGame.cs is the main massage script I think
- CubeMassageGame.cs
- WeakPoint.cs might need patching, might be ok

- Pause menu is a bit bugged but it mostly works. Should probably revisit it later.
  - Sliders don't work