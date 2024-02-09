using System.Reflection;
using Cinemachine;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering.Universal;
using UnityEngine.SpatialTracking;

using Valve.VR;

namespace VirtualOrc.Scripts;

public class VRRig : MonoBehaviour {

    private static UnityEvent _onReady = new();
    private static bool _ready;
    public static void OnReady(UnityAction onReady) {
        if (_ready) {
            onReady?.Invoke();
            return;
        }
        
        _onReady.AddListener(onReady);
    }
    
    public static VRRig Instance;
    
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

    
    private void Awake() {
        if (Instance != null) {
            // Log.Warning("VRRig: Too many instances!");
            // This is expected, we make a copy of this gameObject while setting up the rig
            // Should probably fix this by moving the rig script off the main camera.
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

        Recenter();
    }

    private void Start() {
        Log.Info("VR Rig ready");
        _ready = true;
        _onReady?.Invoke();
        _onReady?.RemoveAllListeners();
    }

    private void OnDestroy() {
        if (Instance == this) {
            Instance = null;
        }
        
        _ready = false;
        _onReady?.RemoveAllListeners();
    }


    
    public void Recenter() {
        rigOffset.transform.position += rigRoot.transform.position - headsetObj.transform.position;
        
        if (Plugin.Config.EnableHeadsetTracking.Value == false) {
            rigOffset.transform.rotation = cineCam.transform.rotation;
        }
    }

    
    private void Update() {
        Vector3 offset = rigRoot.transform.position - headsetObj.transform.position;
        
        if (Plugin.Config.EnableAutoRecenter.Value && Plugin.Config.EnableHeadsetTracking.Value) {
            // Recenter if offset is larger than our threshold
            if (offset.sqrMagnitude > Plugin.Config.AutoRecenterDistance.Value * Plugin.Config.AutoRecenterDistance.Value) {
                Recenter();
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
        canvasHolder.transform.localPosition = new Vector3(0, 0, Plugin.Config.CanvasDistance.Value);
        canvasHolder.transform.localScale = new Vector3(Plugin.Config.CanvasScaleFactor, Plugin.Config.CanvasScaleFactor, 1);
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
        if (Plugin.Config.EnableHeadsetTracking.Value == false) {
            headsetDriver.enabled = false;
        }
        
        Recenter();
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

        var leftLaser = leftController.AddComponent<VRLaser>();
        leftLaser.inputHand = SteamVR_Input_Sources.LeftHand;
        var rightLaser = rightController.AddComponent<VRLaser>();
        rightLaser.inputHand = SteamVR_Input_Sources.RightHand;

    }
    
}