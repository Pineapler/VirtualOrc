using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Pineapler.Utils;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using Valve.VR;

namespace VirtualOrc.Scripts;

public class RuntimeVRLoaderManager : MonoBehaviour {
    private void Start() {
        StartCoroutine(InitializeVRLoader());
    }
   
    
    private IEnumerator InitializeVRLoader() {
        Log.Info("Initializing SteamVR");

        SteamVR_Actions.PreInitialize();
        
        var generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
        var managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
        var xrLoader = ScriptableObject.CreateInstance<OpenVRLoader>();

        var settings = OpenVRSettings.GetSettings();
        settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.MultiPass;
        

        generalSettings.Manager = managerSettings;
        ((List<XRLoader>)managerSettings.activeLoaders).Clear();
        ((List<XRLoader>)managerSettings.activeLoaders).Add(xrLoader);
        
        managerSettings.InitializeLoaderSync();
        
        Log.Info("Getting methods and invoking");
        typeof(XRGeneralSettings).GetMethod("InitXRSDK", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(generalSettings, []);
        typeof(XRGeneralSettings).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(generalSettings, []);

        SteamVR.Initialize(true);
        // SteamVR.Initialize();

        // if (StartDisplay() == false) {
        //     Log.Error("Failed to start XR display subsystem");
        // }

        yield return null;
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
// ==========================================================
