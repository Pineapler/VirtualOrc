# WIP

- [ ] Move all InteractableHintUI objects to world space and fix their canvases
  - might need to billboard them?

# Backlog

- [ ] Hook the SteamVR recentre event and apply our own `VrRig().Recentre`. Also do Y rotation.

# Other mods?

- [ ] SetGirlCostume
  - Currently costume is set in `VipMassageGameSystem.CheckCostume()` based on heart level.
  - Instead have these as unlocked costumes and let the player choose on massage begin.