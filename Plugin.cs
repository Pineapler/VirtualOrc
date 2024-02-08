using System;
using Pineapler.Utils;
using System.IO;
using System.Linq;
using System.Reflection;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using VirtualOrc.Scripts;

namespace VirtualOrc;

public class Config {
    public static bool EnableLaserInput = true;
    public static bool EnableHeadsetTracking = true;
    public static bool EnableAutoRecentre = true;
    public static float AutoRecentreDistance = 0.5f;
    public static float CanvasDistance = 1f;
    public static float CanvasScaleFactor = 0.00075f;
}


[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BaseUnityPlugin {
    // ==========================================================
    // GAME CONFIGURATION
    public const string PluginGuid = "com.pineapler.virtualorc";
    public const string PluginName = "VirtualOrc";
    public const string PluginVersion = "0.0.1";
    // ==========================================================
    
    private void Awake() {
        
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

    // ======================================================================
    
    public static RuntimeVRLoaderManager VrLoaderManager;
    public static VrRig VrRig;
    public static VrInputModule VrInputModule;
    public static StandaloneInputModule OldInputModule;

    public SteamVR_Action_Boolean toggleUIAction;
   
    public CameraManager cameraManager;
    
    
    public void Start() {
        RuntimeVRLoaderManager.OnReady(AfterVrLoaded);
    }

    
    public void AfterVrLoaded() {
        toggleUIAction = SteamVR_Actions._default.ToggleUI;
        toggleUIAction.AddOnStateDownListener(OnToggleUI, SteamVR_Input_Sources.Any);
        
        InsertVrInputModule();
        
        FixCameraManagerRefs();
        
        FixBodyStateRenderCam();
        
        FixInteractableHintUI();
    }

    
    public void InsertVrInputModule() {
        OldInputModule = FindObjectOfType<StandaloneInputModule>();
        VrInputModule = OldInputModule.gameObject.AddComponent<VrInputModule>();
        OldInputModule.enabled = !VirtualOrc.Config.EnableLaserInput;
        VrInputModule.enabled = VirtualOrc.Config.EnableLaserInput;
    }
    
    private void FixCameraManagerRefs() {
        cameraManager = FindObjectOfType<CameraManager>();
        cameraManager.Cameras.Add(VrRig.cineObj);
        cameraManager.MainCam = VrRig.cineObj;
        cameraManager.MainCamOrbit = VrRig.cineObj.GetComponent<CameraOrbit>();
    }
    
    private void FixBodyStateRenderCam() {
        FieldInfo mainCamField = typeof(BodyStateRenderCam).GetField("mainCam", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (BodyStateRenderCam bsrc in FindObjectsOfType<BodyStateRenderCam>()) {
            Log.Info($"Setting {bsrc}.mainCam to headsetCam");
            mainCamField.SetValue(bsrc, VrRig.headsetCam);
        }
    }

    private void FixInteractableHintUI() {
        FieldInfo camField = typeof(InteractableHintUI).GetField("cam", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (InteractableHintUI ui in FindObjectsOfType<InteractableHintUI>()) {
            camField.SetValue(ui, VrRig.headsetCam);
        }
    }


    // =========================================================================
    
    public void OnToggleUI(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        VirtualOrc.Config.EnableLaserInput = !VirtualOrc.Config.EnableLaserInput;
        Log.Info("Toggle UI");
        
        VrInputModule.enabled = VirtualOrc.Config.EnableLaserInput;
        OldInputModule.enabled = !VirtualOrc.Config.EnableLaserInput;
    }


}

