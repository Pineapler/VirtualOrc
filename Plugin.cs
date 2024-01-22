using System;
using Pineapler.Utils;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using VirtualOrc.Scripts;

namespace VirtualOrc;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin {
    // ==========================================================
    // GAME CONFIGURATION
    public const string PluginGuid = "com.pineapler.virtualorc";
    public const string PluginName = "VirtualOrc";
    public const string PluginVersion = "0.0.1";

    // ==========================================================

    public static Plugin Instance;
    public static RuntimeVRLoaderManager VrLoaderManager;
    
    private void Awake() {
        Instance = this;
        
        Log.SetSource(Logger);
        Log.Info($"Plugin {PluginGuid} is starting");

        bool ok;
        ok = LibraryLoader.LoadEarlyRuntimeDependencies();
        if (!ok) {
            Log.Error("Failed to load runtime dependencies.");
            return;
        }

        ok = LibraryLoader.SetupRuntimeAssets();
        if (!ok) {
            Log.Error("Restart the game for VR to work!");
            return;
        }

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        
        // VR rig is spawned from patched method: CameraManager.Start()

    }


}

