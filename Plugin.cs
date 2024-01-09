using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;


namespace VirtualOrc;

[BepInPlugin(PLUGIN_GUID, PLUGIN_NAME, PLUGIN_VERSION)]
public class Plugin : BaseUnityPlugin {
    public const string PLUGIN_GUID = "com.pineapler.virtualorc";
    public const string PLUGIN_NAME = "VirtualOrc";
    public const string PLUGIN_VERSION = "0.0.1";

    public static string gameExePath = Process.GetCurrentProcess().MainModule.FileName;
    public static string gamePath = Path.GetDirectoryName(gameExePath);
    
    public static List<XRDisplaySubsystemDescriptor> DisplayDescriptors = new();
    public static List<XRDisplaySubsystem> displays = new();
    public static XRDisplaySubsystem myDisplay;

    public static GameObject secondEye;
    public static Camera secondCam;

    public static Plugin Instance;
    private void Awake() {
        Instance = this;
        
        Log.SetSource(Logger);

        Log.Info($"Plugin {PLUGIN_GUID} is starting...");
        
        if (!LoadEarlyRuntimeDependencies()) {
            Log.Error("Failed to load runtime dependencies.");
        }

        if (!InitializeVRLoader()) {
            Log.Error("Failed to initialize VR loader.");
        }
        
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

    }

    private bool LoadEarlyRuntimeDependencies() {
        try {
            string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            string deps = Path.Combine(current, "RuntimeDeps");

            foreach (string file in Directory.GetFiles(deps, "*.dll")) {
                string filename = Path.GetFileName(file);
                if (filename == "UnityOpenXR.dll" || filename == "openxr_loader.dll") {
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

        string root = Path.Combine(Paths.GameRootPath, "OrcMassage_Data");
        
        string subsystems = Path.Combine(root, "UnitySubsystems");
        if (!Directory.Exists(subsystems)) {
            Directory.CreateDirectory(subsystems);
        }

        string openXr = Path.Combine(subsystems, "UnityOpenXR");
        if (!Directory.Exists(openXr)) {
            Directory.CreateDirectory(openXr);
        }

        string manifest = Path.Combine(openXr, "UnitySubsystemsManifest.json");
        if (!File.Exists(manifest)) {
            File.WriteAllText(manifest, Properties.Resources.UnitySubsystemsManifest);
            ok = false;
        }

        string plugins = Path.Combine(root, "Plugins");
        string uoxrTarget = Path.Combine(plugins, "UnityOpenXR.dll");
        string oxrLoaderTarget = Path.Combine(plugins, "openxr_loader.dll");

        string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
        string uoxr = Path.Combine(current, "RuntimeDeps/UnityOpenXR.dll");
        string oxrLoader = Path.Combine(current, "RuntimeDeps/openxr_loader.dll");

        if (File.Exists(uoxr)) {
            File.Copy(uoxr, uoxrTarget, true);
        }
        else {
            Log.Warning("Could not find UnityOpenXR.dll, VR might not work!");
        }

        if (File.Exists(oxrLoader)) {
            File.Copy(oxrLoader, oxrLoaderTarget, true);
        }
        else {
            Log.Warning("Could not find openxr_loader.dll, VR might not work!");
        }
        
        return ok;
    }

    private bool InitializeVRLoader() {
        Log.Info("Loading VR...");

        if (!SetupRuntimeAssets()) {
            Log.Error("Restart the game for VR to work!");
            return false;
        }
        
        EnableControllerProfiles();
        InitializeXRRuntime();

        if (!StartDisplay()) {
            Log.Error("Failed to start in VR mode!");
            return false;
        }

        return true;
    }
    
    private void InitializeXRRuntime() {
        Log.Info("Initializing XR loader...");
        
        XRGeneralSettings generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
        XRManagerSettings managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
        OpenXRLoader xrLoader =             ScriptableObject.CreateInstance<OpenXRLoader>();
        
        generalSettings.Manager = managerSettings;
        
        ((List<XRLoader>)managerSettings.activeLoaders).Clear();
        ((List<XRLoader>)managerSettings.activeLoaders).Add(xrLoader);
        
        OpenXRSettings.Instance.renderMode = OpenXRSettings.RenderMode.MultiPass;
       
        Log.Info("Getting methods and invoking...");
        typeof(XRGeneralSettings).GetMethod("InitXRSDK", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(generalSettings, Array.Empty<object>());
        typeof(XRGeneralSettings).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(generalSettings, Array.Empty<object>());
        
    }

    private void EnableControllerProfiles() {
        Log.Warning("Controller profiles not implemented");
    }

    private bool StartDisplay() {
        List<XRDisplaySubsystem> displays = new();
        SubsystemManager.GetInstances(displays);

        if (displays.Count <= 0) {
            return false;
        }

        displays[0].Start();
        Logger.LogInfo("Started XR display subsystem.");
        
        return true;
    }
}

