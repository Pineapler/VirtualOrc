using System.Reflection;
using Cinemachine;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Serialization;
using UnityEngine.SpatialTracking;
using Valve.VR;

namespace VirtualOrc.Scripts;

public class VrRig : MonoBehaviour {
    public static VrRig Instance;

    public Transform rigRoot;
    public Transform rigOffset;
    
    public GameObject cineObj;
    public CameraOrbit cineOrbit;
    public Camera cineCam;
    
    public GameObject headsetObj;
    public TrackedPoseDriver headsetDriver;
    public Camera headsetCam;



    public CameraManager cameraManager;

    public float autoRecentreDistance = 0.5f;
    
    
    //  xrOrigin (original Cinemachine body)
    //  |--- xrOffset
    //      |--- xrHeadset
    //      |--- xrControllerL
    //      |--- xrControllerR

    private void Awake() {
        if (Instance != null) {
            DestroyImmediate(this);
            return;
        }
        Instance = this;
        
        
        Log.Info("Initializing VR Rig");
        
        SeparateCinemachineBrain();
        
        SetupXrRig();

        FixCameraManagerRefs();

        FixBodyStateRenderCam();

        FixInteractableHintUI();

        InitInputActions();
        
    }



    private void InitInputActions() {
        Log.Info("Activating input actions");
       
        // Head tracker
        headsetDriver = headsetObj.AddComponent<TrackedPoseDriver>();
        
        foreach (var actionSet in SteamVR_Input.actionSets) {
            Log.Info($"Activating action set: {actionSet.GetShortName()}");
            actionSet.Activate();
        }
        
    }

    public void Recentre() {
        rigOffset.transform.position += rigRoot.transform.position - headsetObj.transform.position;
    }

    private void Update() {
        Vector3 offset = rigRoot.transform.position - headsetObj.transform.position;
        if (offset.sqrMagnitude > autoRecentreDistance * autoRecentreDistance) {
            Recentre();
        }
    }

    // ##########
    // ## INIT ##
    // ##########

    private void SeparateCinemachineBrain() {
        headsetObj = gameObject;
        
        // Set up cineObj
        cineObj = Instantiate(headsetObj, transform.parent);
        cineObj.tag = "CinemachineCam";
        cineObj.name = "MainCam"; // Name is important - CameraManager does string comparison
        
        Destroy(cineObj.GetComponent<AudioListener>());
        cineCam = cineObj.GetComponent<Camera>();
        cineCam.enabled = false;
       
        headsetObj.name = "XR Headset";
        Destroy(headsetObj.GetComponent<CinemachineBrain>());
        headsetCam = headsetObj.GetComponent<Camera>();
    }
    
    private void SetupXrRig() {
        rigRoot = new GameObject("XR Rig Root").transform;
        rigRoot.SetParent(cineObj.transform, false);
        
        rigOffset = new GameObject("XR Rig Offset").transform;
        rigOffset.SetParent(rigRoot, false);
        
        headsetObj.transform.SetParent(rigOffset, false);
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
            Log.Info($"Setting {ui}.cam to headsetCam");
            camField.SetValue(ui, headsetCam);
        }
    }
}