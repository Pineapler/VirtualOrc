using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using UnityEngine.XR.OpenXR;
using UnityEngine.XR.OpenXR.Features;
using UnityEngine.XR.OpenXR.Features.Interactions;

namespace VirtualOrc;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin {
    public const string PluginGuid = "com.pineapler.virtualorc";
    public const string PluginName = "VirtualOrc";
    public const string PluginVersion = "0.0.1";

    // public static GameObject secondEye;
    // public static Camera secondCam;

    public static Plugin Instance;
    public static RuntimeXRLoaderManager XrLoaderManager;
    
    private void Awake() {
        Instance = this;
        
        Log.SetSource(Logger);

        Log.Info($"Plugin {PluginGuid} is starting...");

        QualitySettings.vSyncCount = 0;
        
        if (!LoadEarlyRuntimeDependencies()) {
            Log.Error("Failed to load runtime dependencies.");
        }
        
        if (!SetupRuntimeAssets()) {
            Log.Error("Restart the game for VR to work!");
            return;
        }

        if (XrLoaderManager == null) {
            // XrLoaderManager will start a coroutine once it has spawned in
            XrLoaderManager = new GameObject("XR Loader Manager").AddComponent<RuntimeXRLoaderManager>();
            DontDestroyOnLoad(XrLoaderManager.gameObject);
        }
        
        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

    }

    private bool LoadEarlyRuntimeDependencies() {
        try {
            string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
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

        string plugins = Path.Combine(root, "Plugins/x86_64");
        string uoxrTarget = Path.Combine(plugins, "UnityOpenXR.dll");
        string oxrLoaderTarget = Path.Combine(plugins, "openxr_loader.dll");

        string current = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)!;
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
}

public class RuntimeXRLoaderManager : MonoBehaviour {
    private void Start() {
        StartCoroutine(InitializeVRLoader());
    }
   
    
    private IEnumerator InitializeVRLoader() {
        Log.Info("Loading VR...");

        EnableControllerProfiles();
        InitializeXRRuntime();

        if (!StartDisplay()) {
            Log.Error("Failed to start in VR Display subsystem!");
        }

        yield return null;
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
        typeof(XRGeneralSettings).GetMethod("InitXRSDK", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(generalSettings, []);
        typeof(XRGeneralSettings).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(generalSettings, []);
        
    }

    private void EnableControllerProfiles() {
        var valveIndex = ScriptableObject.CreateInstance<ValveIndexControllerProfile>();
        var htcVive = ScriptableObject.CreateInstance<HTCViveControllerProfile>();
        var mmController = ScriptableObject.CreateInstance<MicrosoftMotionControllerProfile>();
        var khrSimple = ScriptableObject.CreateInstance<KHRSimpleControllerProfile>();
        var oculusTouch = ScriptableObject.CreateInstance<OculusTouchControllerProfile>();

        valveIndex.enabled = true;
        htcVive.enabled = true;
        mmController.enabled = true;
        khrSimple.enabled = true;
        oculusTouch.enabled = true;

        // Patch the OpenXRSettings.features field to include controller profiles
        // This feature list is empty by default if the game isn't a VR game
        var featList = new List<OpenXRFeature> {
            valveIndex,
            htcVive,
            mmController,
            khrSimple,
            oculusTouch
        };
        typeof(OpenXRSettings).GetField("features", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(OpenXRSettings.Instance, featList.ToArray());
 
        Log.Info("Enabled controller profiles.");
    }

    private bool StartDisplay() {
        List<XRDisplaySubsystem> displays = new();
        SubsystemManager.GetInstances(displays);
        
        Log.Info($"Found {displays.Count} displays");
        if (displays.Count <= 0) {
            return false;
        }

        displays[0].Start();
        Log.Info("Started XR display subsystem.");
        
        return true;
    }
}