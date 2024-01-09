# Building and installing plugin

- Copy `<game>/OrcMassage_Data/Managed/Assembly-CSharp.dll` to `Libs`
- Build
- Copy VirtualOrc.dll from the output directory into `<game>/BepInEx/plugins`
- Make a new directory `<game>/BepInEx/plugins/RuntimeDeps`
- Copy the following files from Libs into RuntimeDeps:
    - `openxr_loader.dll`
    - `Unity.InputSystem.dll`
    - `Unity.XR.Management.dll`
    - `Unity.XR.OpenXR.dll`
    - `UnityEngine.SpatialTracking.dll`
    - `UnityOpenXR.dll`
- Launch the game and wait for it to load. This first run will set up the game to work with VR.
- Close the game and relaunch it.