﻿# Overview

Install BepInEx and run the game at least once to generate the mod folders.

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

## Build configurations

After following these instructions once, the mod can be updated by overwriting only the `VirtualOrc.dll` file.


A Rider configuration is available, "Run Modded", which will build and copy the file, then launch the game.

In order to use either the configuration or the following command, you will need to set an environment variable: `ORC_MASSAGE_DIR` to the directory where the game is installed.

If you're not using Rider, you can set up a build configuration with this command:
```shell
Copy-Item "bin\Debug\netstandard2.1\VirtualOrc.dll" "$env:ORC_MASSAGE_DIR\BepInEx\plugins\VirtualOrc.dll" -force; Start-Process "$env:ORC_MASSAGE_DIR\OrcMassage.exe"
```