using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using Unity.XR.OpenVR;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using Valve.VR;
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

        if (!LoadEarlyRuntimeDependencies()) {
            Log.Error("Failed to load runtime dependencies.");
        }
        
        if (!SetupRuntimeAssets()) {
            Log.Error("Restart the game for VR to work!");
            return;
        }

        // if (VrLoaderManager == null) {
        //     // XrLoaderManager will start a coroutine once it has spawned in
        //     VrLoaderManager = new GameObject("XR Loader Manager").AddComponent<RuntimeVRLoaderManager>();
        //     DontDestroyOnLoad(VrLoaderManager.gameObject);
        // }
        
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

    }


    private static readonly string[] PluginDllNames = {
        "openvr_api.dll",
        "XRSDKOpenVR.dll",
        "UnityOpenXR.dll",
        "openxr_loader.dll",
    };
    
    private bool LoadEarlyRuntimeDependencies() {
        try {
            string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
            string deps = Path.Combine(current, "RuntimeDeps");

            foreach (string file in Directory.GetFiles(deps, "*.dll")) {
                string filename = Path.GetFileName(file);
                if (PluginDllNames.Contains(filename)) {
                    continue;
                }

                Log.Info($"Early loading {filename}");

                try {
                    Assembly.LoadFile(file);
                }
                catch (Exception ex) {
                    Log.Warning($"Failed to early load {filename}: {ex.Message}");
                }
            }
        }
        catch (Exception ex) {
            Log.Error($"Unexpected error while loading early runtime dependencies: {ex.Message}");
            return false;
        }

        return true;
    }

    /// Set up runtime assets so that the game can launch with VR support.
    private bool SetupRuntimeAssets() {
        bool ok = true;

        string root = Path.Combine(Paths.GameRootPath, Application.productName + "_Data");
        
        // Copy subsystem manifest
        string subsystems = Path.Combine(root, "UnitySubsystems");
        if (!Directory.Exists(subsystems)) {
            Directory.CreateDirectory(subsystems);
        }
        
        // string openXr = Path.Combine(subsystems, "UnityOpenXR");
        string openVr = Path.Combine(subsystems, "XRSDKOpenVR");
        if (!Directory.Exists(openVr)) {
            Directory.CreateDirectory(openVr);
        }

        string manifest = Path.Combine(openVr, "UnitySubsystemsManifest.json");
        if (!File.Exists(manifest)) {
            File.WriteAllText(manifest, Properties.Resources.UnitySubsystemsManifest);
            ok = false;
        }
        
        // Copy plugins dlls
        string plugins = Path.Combine(root, "Plugins/x86_64");
        string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;

        foreach (string pluginDll in PluginDllNames) {
            string target = Path.Combine(plugins, pluginDll);
            string source = Path.Combine(current, "RuntimeDeps/" + pluginDll);

            if (File.Exists(source)) {
                File.Copy(source, target, true);
            }
            else {
                Log.Warning($"Could not find {pluginDll}, VR might not work!");
            }
        }
        
        return ok;
    }
}

