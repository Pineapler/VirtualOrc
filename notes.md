﻿# Camera rig
- Move Unity camera to a child gameobject
  - Update CinemachineBrain.OutputCamera to point to child
  - Move AudioListener to child
- Child gameobject gets local position from TrackedPoseDriver


- Camera rig may need to be its own object that we move around by patching the "camera change" methods


# Input System

- InputActionAsset.FromJson() not working? There's just no maps or actions idk