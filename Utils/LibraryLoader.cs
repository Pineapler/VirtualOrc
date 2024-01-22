using System;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using UnityEngine;
namespace Pineapler.Utils;

public static class LibraryLoader {
    private static readonly string[] PluginDllNames = {
        "openvr_api.dll",
        "XRSDKOpenVR.dll",
        "UnityOpenXR.dll",
        "openxr_loader.dll",
    };
    
    public static bool LoadEarlyRuntimeDependencies() {
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
    public static bool SetupRuntimeAssets() {
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