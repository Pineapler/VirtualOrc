using Cinemachine;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.SpatialTracking;
using Valve.VR;

namespace VirtualOrc.Scripts;

public class VrRig : MonoBehaviour {
    public static VrRig Instance;

    public Transform rigRoot;
    public Transform rigOffset;
    
    public GameObject camCine;
    public GameObject camXr;
    public CameraOrbit camCineOrbit;

    public TrackedPoseDriver headsetDriver;

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


        camXr = gameObject;
        camXr.name = "XR Headset";
        camCine = Instantiate(camXr, transform.parent);
        
        // Turn off cinemachine brain on the original camera (now xr camera)
        // camXr.GetComponent<CinemachineBrain>().enabled = false;
        Destroy(camXr.GetComponent<CinemachineBrain>());
        
        // Turn off camera and audio listener on cinemachine brain
        camCine.GetComponent<Camera>().enabled = false;
        // camCine.GetComponent<AudioListener>().enabled = false;
        // Destroy(camCine.GetComponent<EPOOutline.Outliner>());
        // Destroy(camCine.GetComponent<UniversalAdditionalCameraData>());
        // Destroy(camCine.GetComponent<Camera>());
        Destroy(camCine.GetComponent<AudioListener>());
        camCine.tag = "Untagged";
        camCine.name = "MainCam"; // Name is important - CameraManager does string comparison
        
        // Set up rig as child of cinemachine camera
        rigRoot = new GameObject("XR Rig Root").transform;
        rigRoot.SetParent(camCine.transform, false);
        rigOffset = new GameObject("XR Rig Offset").transform;
        rigOffset.SetParent(rigRoot, false);
        camXr.transform.SetParent(rigOffset, false);

        cameraManager = FindObjectOfType<CameraManager>();
        cameraManager.Cameras.Add(camCine);
        cameraManager.MainCam = camCine;
        cameraManager.MainCamOrbit = camCine.GetComponent<CameraOrbit>();
        
        InitInputActions();
        
        
    }


    private void InitInputActions() {
        Log.Info("Activating input actions");
       
        // Head tracker
        headsetDriver = camXr.AddComponent<TrackedPoseDriver>();
        
        foreach (var actionSet in SteamVR_Input.actionSets) {
            Log.Info($"Activating action set: {actionSet.GetShortName()}");
            actionSet.Activate();
        }
        
    }

    public void Recentre() {
        rigOffset.transform.position += rigRoot.transform.position - camXr.transform.position;
    }

    private void Update() {
        Vector3 offset = rigRoot.transform.position - camXr.transform.position;
        if (offset.sqrMagnitude > autoRecentreDistance * autoRecentreDistance) {
            Recentre();
        }
    }
}