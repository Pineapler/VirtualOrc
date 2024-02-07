using System.Reflection;
using Cinemachine;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.SpatialTracking;

using Valve.VR;
using Valve.VR.Extras;

namespace VirtualOrc.Scripts;

public class VrRig : MonoBehaviour {
    public static VrRig Instance;
    
    // SETTINGS
    public bool autoRecentre = true;
    public float autoRecentreDistance = 0.5f;
    public float canvasDistance = 1f;
    public float canvasScaleFactor = 0.00075f;
    // ========
    

    private static UnityEvent _onReady = new();
    private static bool _isReady = false;
    
    public Transform rigRoot;
    public Transform rigOffset;
    
    public GameObject cineObj;
    public CameraOrbit cineOrbit;
    public Camera cineCam;
    
    public GameObject headsetObj;
    public Camera headsetCam;

    public GameObject uiCamObj;
    public Camera uiCam;

    public GameObject canvasHolder;

    public GameObject leftController;
    public GameObject rightController;

    public CameraManager cameraManager;

    //  xrOrigin (original Cinemachine body)
    //  |--- xrOffset
    //      |--- xrHeadset
    //      |--- xrControllerL
    //      |--- xrControllerR

    public static void OnReady(UnityAction action) {
        if (_isReady) {
            action.Invoke();
            return;
        }
        
        _onReady.AddListener(action);
    }
    
    
    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }
        Instance = this;
        
        
        Log.Info("Initializing VR Rig");

        
        SeparateCinemachineBrain();
        
        SetupCanvasHolder();
        
        ConfigureUICamera();
        
        SetupXrRig();
        
        SetupControllers();

        FixCameraManagerRefs();

        FixBodyStateRenderCam();

        FixInteractableHintUI();

        InitInputActions();

        _isReady = true;
        _onReady?.Invoke();
        _onReady?.RemoveAllListeners();
    }



    private void InitInputActions() {
        Log.Info("Activating input actions");
       
        SteamVR_Actions._default.Activate();
        
    }

    public void Recentre() {
        rigOffset.transform.position += rigRoot.transform.position - headsetObj.transform.position;
    }

    private void Update() {
        Vector3 offset = rigRoot.transform.position - headsetObj.transform.position;
        
        if (autoRecentre) {
            if (offset.sqrMagnitude > autoRecentreDistance * autoRecentreDistance) {
                Recentre();
            }
        }
    }

    // ##########
    // ## INIT ##
    // ##########

    private void SetupCanvasHolder() {
        canvasHolder = new GameObject("CanvasHolder");
        canvasHolder.layer = LayerMask.NameToLayer("UI");
        canvasHolder.transform.SetParent(cineObj.transform, false);
        canvasHolder.transform.localPosition = new Vector3(0, 0, canvasDistance);
        canvasHolder.transform.localScale = new Vector3(canvasScaleFactor, canvasScaleFactor, 1);
    }

    private void SeparateCinemachineBrain() {
        headsetObj = gameObject;
        
        // Set up cineObj
        cineObj = Instantiate(headsetObj, transform.parent);
        cineObj.tag = "Untagged";
        cineObj.name = "MainCam"; // Name is important - CameraManager does string comparison
        
        Destroy(cineObj.GetComponent<AudioListener>());
        cineCam = cineObj.GetComponent<Camera>();
        cineCam.enabled = false;
       
        headsetObj.name = "XR Headset";
        Destroy(headsetObj.GetComponent<CinemachineBrain>());
        headsetCam = headsetObj.GetComponent<Camera>();
    }

    private void ConfigureUICamera() {
        int uiMask = LayerMask.GetMask("UI");
        headsetCam.cullingMask &= ~uiMask; // Remove UI layer from main cam
        
        uiCamObj = new GameObject("UI Camera");
        uiCamObj.transform.SetParent(headsetObj.transform, false);
        uiCam = uiCamObj.AddComponent<Camera>();
        uiCam.CopyFrom(headsetCam);

        
        uiCam.cullingMask = uiMask;
        uiCam.clearFlags = CameraClearFlags.Depth;
        uiCam.depth = 10;
       
        // Set up overlay
        UniversalAdditionalCameraData adlCamData = uiCamObj.AddComponent<UniversalAdditionalCameraData>();
        adlCamData.renderType = CameraRenderType.Overlay;
        headsetCam.GetUniversalAdditionalCameraData().cameraStack.Add(uiCam);
    }
    
    private void SetupXrRig() {
        rigRoot = new GameObject("XR Rig Root").transform;
        rigRoot.SetParent(cineObj.transform, false);
        
        rigOffset = new GameObject("XR Rig Offset").transform;
        rigOffset.SetParent(rigRoot, false);
        
        headsetObj.transform.SetParent(rigOffset, false);
        var headsetDriver = headsetObj.AddComponent<TrackedPoseDriver>();
    }

    private void SetupControllers() {
        leftController = new GameObject("XR Controller L");
        leftController.transform.SetParent(rigOffset, false);
        var leftPose = leftController.AddComponent<SteamVR_Behaviour_Pose>();
        leftPose.inputSource = SteamVR_Input_Sources.LeftHand;
        var leftDriver = leftController.AddComponent<TrackedPoseDriver>();
        leftDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.LeftPose);
        
        rightController = new GameObject("XR Controller R");
        rightController.transform.SetParent(rigOffset, false);
        var rightPose = rightController.AddComponent<SteamVR_Behaviour_Pose>();
        rightPose.inputSource = SteamVR_Input_Sources.RightHand;
        var rightDriver = rightController.AddComponent<TrackedPoseDriver>();
        rightDriver.SetPoseSource(TrackedPoseDriver.DeviceType.GenericXRController, TrackedPoseDriver.TrackedPose.RightPose);

        var leftLaser = leftController.AddComponent<LaserUIInput>();
        leftLaser.inputHand = SteamVR_Input_Sources.LeftHand;
        var rightLaser = rightController.AddComponent<LaserUIInput>();
        rightLaser.inputHand = SteamVR_Input_Sources.RightHand;
        
    }
    
    private void FixCameraManagerRefs() {
        cameraManager = FindObjectOfType<CameraManager>();
        cameraManager.Cameras.Add(cineObj);
        cameraManager.MainCam = cineObj;
        cameraManager.MainCamOrbit = cineObj.GetComponent<CameraOrbit>();
    }
    
    private void FixBodyStateRenderCam() {
        FieldInfo mainCamField = typeof(BodyStateRenderCam).GetField("mainCam", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (BodyStateRenderCam bsrc in FindObjectsOfType<BodyStateRenderCam>()) {
            Log.Info($"Setting {bsrc}.mainCam to headsetCam");
            mainCamField.SetValue(bsrc, headsetCam);
        }
    }

    private void FixInteractableHintUI() {
        FieldInfo camField = typeof(InteractableHintUI).GetField("cam", BindingFlags.Instance | BindingFlags.NonPublic);
        foreach (InteractableHintUI ui in FindObjectsOfType<InteractableHintUI>()) {
            camField.SetValue(ui, headsetCam);
        }
    }
}