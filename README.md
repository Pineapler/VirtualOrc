﻿# ⚠️ VERY EARLY WIP ⚠️

This mod is barely functional and not ready for general use.

See [notes.md](notes.md) for a more up-to-date checklist of what I'm working on and what is planned.

I'm not sure what the scope of this mod will be at the moment. The vanilla gameplay seems to lend itself
to a VR control scheme very well, but I'm not sure if I will have time to fully flesh it out. At the very least there will
be 6 degrees of freedom head movement with the original keyboard/mouse controls.

# VirtualOrc

A VR mod for the game ["Orc Massage" by Torch Studio](https://store.steampowered.com/app/1129540/Orc_Massage/)


# Prerequisites

Install BepInEx and run the game at least once to generate the mod folders.

# Building and manually packaging plugin

## Build the plugin

- Copy `<game>/OrcMassage_Data/Managed/Assembly-CSharp.dll` to `Libs`
- Build

## Package the plugin

- Create a directory `<pkg_dir>` to store the packaged plugin, e.g. "VirtualOrc".
- Copy `VirtualOrc.dll` from the build output directory into the package directory.
- Copy `<src_dir>/Libs/RuntimeDeps` into `<pkg_dir>/RuntimeDeps`
- Copy the directory `<src_dir>/StreamingAssets` into `<pkg_dir>/RuntimeDeps/StreamingAssets`

The packaged plugin should look like the following
```
v VirtualOrc
    v RuntimeDeps
        > Plugins
        > StreamingAssets
        ...
    VirtualOrc.dll
```

- Place the packaged plugin directory in `<game>/BepInEx/plugins`
- Launch the game and wait for it to load. This first run will set up the game to work with VR.
- Close the game and relaunch it.

## Updating the plugin

After following these instructions once, the mod can be updated by overwriting only the `VirtualOrc.dll` file.


A Rider configuration is available, "Run Modded", which will build and copy the file, then launch the game.

In order to use either the configuration or the following command, you will need to set an environment variable: `ORC_MASSAGE_DIR` to the directory where the game is installed.

If you're not using Rider, you can set up a build configuration with this command:
```shell
Copy-Item "bin\Debug\netstandard2.1\VirtualOrc.dll" "$env:ORC_MASSAGE_DIR\BepInEx\plugins\VirtualOrc.dll" -force; Start-Process "$env:ORC_MASSAGE_DIR\OrcMassage.exe"
```