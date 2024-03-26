# WIP

- [ ] Controller laser interaction (partial)
- [ ] Move full body massage targets UI to world space

# Backlog

### Features

- [ ] Move around the scene using VR controllers
  - Orbit
  - Free move?
- [ ] Add an effect to hide the outside world while indoors
- [ ] Laser should be hidden during loading screens (outside)
- [ ] Lock horizon (option to disable)

### Bugs 

- [ ] Fix LogSystemManager properly. Currently disabled the UI so that it's not cluttering the screen.
- [ ] Patch out WaterFiller (null ref from zibra liquids)
- [ ] Replace the missing fluid sim water with a vr-friendly alternative
- [ ] "Return to menu" breaks user input
  - SteamVR actions don't like the reload for some reason, probably should add check if steamvr already initialized
- [ ] Pause menu layout is slightly messed up in world space.
  - volume sliders don't work - implement in VRInputModule

### UI
 
- [ ] Move computer UI to world space
- [ ] Add fade effect to full camera

### Unity/BepInEx/Asset specific

- [ ] Hook the SteamVR recentre event and apply our own `VrRig().Recentre`. Also do Y rotation.

# Complete

- [X] Automatic SteamVR injection
- [X] VR camera rig that follows the vanilla camera
- [X] Patch out features that would crash VR (Zibra Liquid)
- [X] World space UI elements use an overlay camera
- [X] Move main menu canvas to world space
- [X] Move phone UI to world space
- [X] Move client dialogue UI to world space
- [X] Automatically copy SteamVR StreamingAssets to game directory
- [x] Parent orc hands to VR controllers, use these in massage games instead of laser

# Notes

- OrcTouchingHand.cs has cam/mouse raycast logic
- RaycastSystem.cs
- BodyStateRenderCam.cs
- MassageIndicator.cs
- TrainingGame.cs is the main massage script I think
- CubeMassageGame.cs
- WeakPoint.cs might need patching, might be ok


- OrcTouchingHand