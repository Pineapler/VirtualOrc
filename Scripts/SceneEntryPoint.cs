using System.Reflection;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;

namespace VirtualOrc.Scripts;

public class SceneEntryPoint : MonoBehaviour {
    public static SceneEntryPoint Instance;
    
    public StandaloneInputModule oldInputModule;
   
    public CameraManager cameraManager;

    public void Awake() {
        if (Instance != null) {
            Log.Warning("SceneEntryPoint: Too many instances!");
            Destroy(this);
            return;
        }

        Instance = this;
    }
    
    public void Start() {
        RuntimeVRLoader.OnReady(AfterVrLoaded);
    }

    public void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
    }


    public void AfterVrLoaded() {
        Input.toggleUI.RemoveOnStateDownListener(OnToggleUI, SteamVR_Input_Sources.Any);
        Input.toggleUI.AddOnStateDownListener(OnToggleUI, SteamVR_Input_Sources.Any);
        Input.pause.RemoveOnStateDownListener(OnPause, SteamVR_Input_Sources.Any);
        Input.pause.AddOnStateDownListener(OnPause, SteamVR_Input_Sources.Any);
        
        InsertVrInputModule();

        InsertVRRig();
        
        FixCameraManagerRefs();
        
        FixBodyStateRenderCam();
        
        VRInputModule.Instance.PushLaserContext();
    }

    
    public void InsertVrInputModule() {
        oldInputModule = FindObjectOfType<StandaloneInputModule>();
        var vrInputModule = oldInputModule.gameObject.AddComponent<VRInputModule>();
        oldInputModule.enabled = !vrInputModule.enabled;
    }

    public void InsertVRRig() {
        CameraManager.Instance.MainCam.AddComponent<VRRig>();
    }
    
    private void FixCameraManagerRefs() {
        CameraManager.Instance.Cameras.Add(VRRig.Instance.cineObj);
        CameraManager.Instance.Cameras.Add(VRRig.Instance.cineObj);
        CameraManager.Instance.MainCam = VRRig.Instance.cineObj;
        CameraManager.Instance.MainCamOrbit = VRRig.Instance.cineObj.GetComponent<CameraOrbit>();
    }
    
    private void FixBodyStateRenderCam() {
        FieldInfo mainCamField = typeof(BodyStateRenderCam).GetField("mainCam", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (BodyStateRenderCam bsrc in FindObjectsOfType<BodyStateRenderCam>()) {
            Log.Info($"Setting {bsrc}.mainCam to headsetCam");
            mainCamField.SetValue(bsrc, VRRig.Instance.headsetCam);
        }
    }

    // =========================================================================
    
    public void OnToggleUI(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        Plugin.Config.vrInputEnabled = !Plugin.Config.vrInputEnabled;
        Log.Info($"VR UI input: {Plugin.Config.vrInputEnabled}");
        
        VRInputModule.Instance.enabled = Plugin.Config.vrInputEnabled;
        oldInputModule.enabled = !Plugin.Config.vrInputEnabled;
    }


    public void OnPause(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (OptionMenu.Instance != null) {
            OptionMenu.Instance.ToggleCanvas();
        }
    }

}