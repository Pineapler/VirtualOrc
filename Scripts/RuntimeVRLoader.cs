using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using Pineapler.Utils;
using Unity.XR.OpenVR;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.XR;
using UnityEngine.XR.Management;
using Valve.VR;

namespace VirtualOrc.Scripts;

public class RuntimeVRLoader : MonoBehaviour {
    public static RuntimeVRLoader Instance;
    
    private static bool _isReady;
    private static UnityEvent _onReady = new();

    
    public static void OnReady(UnityAction action) {
        if (_isReady) {
            action.Invoke();
            return;
        }
        
        _onReady.AddListener(action);
    }

    private void Awake() {
        if (Instance != null) {
            Log.Warning("RuntimeVRLoader: Too many instances!");
            Destroy(this);
        }

        Instance = this;
        DontDestroyOnLoad(gameObject); // Only load VR once
    }
    
    private void Start() {
        StartCoroutine(InitializeVRLoader());
    }
   
    
    private IEnumerator InitializeVRLoader() {
        Log.Info("VR Loader starting");

        SteamVR_Actions.PreInitialize();
        
        var generalSettings = ScriptableObject.CreateInstance<XRGeneralSettings>();
        var managerSettings = ScriptableObject.CreateInstance<XRManagerSettings>();
        var xrLoader = ScriptableObject.CreateInstance<OpenVRLoader>();

        var settings = OpenVRSettings.GetSettings();
        // settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.MultiPass;
        settings.StereoRenderingMode = OpenVRSettings.StereoRenderingModes.SinglePassInstanced;
        

        generalSettings.Manager = managerSettings;
        ((List<XRLoader>)managerSettings.activeLoaders).Clear();
        ((List<XRLoader>)managerSettings.activeLoaders).Add(xrLoader);
        
        managerSettings.InitializeLoaderSync();
        
        typeof(XRGeneralSettings).GetMethod("InitXRSDK", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(generalSettings, []);
        typeof(XRGeneralSettings).GetMethod("Start", BindingFlags.NonPublic | BindingFlags.Instance)!.Invoke(generalSettings, []);

        SteamVR.Initialize(true);

        if (StartDisplay() == false) {
            Log.Error("Failed to start XR display subsystem");
            yield return null;
        }
        
        SteamVR_Actions._default.Activate();
        Input.Init();
        
        Log.Info("VR Loader ready");
        _isReady = true;
        _onReady?.Invoke();
        _onReady?.RemoveAllListeners();
        yield return null;
    }

    private bool StartDisplay() {
        List<XRDisplaySubsystem> displays = new();
        SubsystemManager.GetInstances(displays);
        
        if (displays.Count <= 0) {
            Log.Info("No XR displays found");
            return false;
        }

        displays[0].Start();
        Log.Info("Started XR display subsystem.");
        
        return true;
    }
}
// ==========================================================
