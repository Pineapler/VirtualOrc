# ⚠️ VERY EARLY WIP ⚠️

This mod is barely functional and not ready for general use.

See [notes.md](notes.md) for a more up-to-date checklist of what I'm working on and what is planned.

I'm not sure what the scope of this mod will be at the moment. The vanilla gameplay seems to lend itself
to a VR control scheme very well, but I'm not sure if I will have time to fully flesh it out. At the very least there will
be 6 degrees of freedom head movement with the original keyboard/mouse controls.

# VirtualOrc

A VR mod for the game ["Orc Massage" by Torch Studio](https://store.steampowered.com/app/1129540/Orc_Massage/)


# Prerequisites

- Install BepInEx and run the game at least once to generate the mod folders.
- Add an environment variable "ORC_MASSAGE_DIR" which points to your game installation directory.

# Build and install

You will need to provide your own copy of `Assembly-CSharp.dll` from the game's files. 
1. Locate the DLL: `ORC_MASSAGE_DIR/OrcMassage_Data/Managed/Assembly-CSharp.dll`
2. Copy it to `VirtualOrc/Libs/`

If you are using Jetbrains Rider, there is a run configuration called "Package and run".
This will build the mod, create the package structure, and copy it to your game's BepInEx plugins folder.

If you aren't using Rider:
1. Build the mod: `dotnet build`
2. Package the mod: `./VirtualOrc-PreparePackage.ps1`
3. Install the mod: `./VirtualOrc-Install.ps1`

After installing the mod for the first time, you will need to run the game once, then restart the game for VR to work. 
