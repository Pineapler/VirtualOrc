using Pineapler.Utils;
using System.Reflection;
using BepInEx;
using HarmonyLib;

namespace VirtualOrc;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin {
    // ==========================================================
    // GAME CONFIGURATION
    public const string PluginGuid = "com.pineapler.virtualorc";
    public const string PluginName = "VirtualOrc";
    public const string PluginVersion = "0.0.1";
    // ==========================================================

    public static new Config Config { get; private set; }
    
    private void Awake() {
        Config = new Config(base.Config);
        
        Log.SetSource(Logger);

        if (!Config.EnablePlugin.Value) {
            Log.Warning("VirtualOrc is disabled.");
            return;
        }
        
        Log.Info($"Plugin {PluginGuid} is starting");

        bool ok;
        ok = LibraryLoader.LoadEarlyRuntimeDependencies();
        if (!ok) {
            Log.Error("Failed to load runtime dependencies.");
            return;
        }

        bool ok1 = LibraryLoader.SetupPlugins();
        bool ok2 = LibraryLoader.SetupSubsystems();
        bool ok3 = LibraryLoader.SetupStreamingAssets();
        if (!ok1 || !ok2 || !ok3) {
            Log.Error("Restart the game for VR to work!");
            return;
        }

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());
        
        // VR rig is spawned from patched method: CameraManager.Start()

    }
}

