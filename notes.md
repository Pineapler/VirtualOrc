# WIP

- [ ] Camera should self-recentre the first time headset tracking is gained
  - For now, recentre whenever headset gets too far from origin
- [ ] Fix UI icon sync issues in massage (`BodyStateRenderCam.cs`)
- [ ] Fix UI icon sync issues in main menu
  - These should eventually be turned into world space objects or something

# Backlog

- [ ] Screenspace canvases should be moved to world-space (children of VR rig)

# Other mods?

- [ ] SetGirlCostume
  - Currently costume is set in `VipMassageGameSystem.CheckCostume()` based on heart level.
  - Instead have these as unlocked costumes and let the player choose on massage begin.