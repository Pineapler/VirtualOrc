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

    public TrackedPoseDriver headsetDriver;

    public CameraManager cameraManager;
    
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
        camXr.name = "MainCam - XR Headset";
        camCine = Instantiate(camXr, transform.parent);
        
        // Turn off cinemachine brain on the original camera (now xr camera)
        camXr.GetComponent<CinemachineBrain>().enabled = false;
        
        // Turn off camera and audio listener on cinemachine brain
        camCine.GetComponent<Camera>().enabled = false;
        camCine.GetComponent<AudioListener>().enabled = false;
        camCine.tag = "untagged";
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
        
        InitInputActions();
        
        // TODO: Recentre on first tracking established
        // Just estimate for now.
        rigOffset.transform.localPosition += new Vector3(0, -1.2f, 0);
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

    // private void Update() {
        // Log.Info(Actions.Head_Rotation.ReadValue<Quaternion>());
        // xrHeadset.transform.localPosition = Actions.Head_Position.ReadValue<Vector3>();
        // xrHeadset.transform.localRotation = Actions.Head_Rotation.ReadValue<Quaternion>();
    // }
}