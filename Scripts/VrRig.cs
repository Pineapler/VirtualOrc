using System.Reflection;
using Cinemachine;
using UnityEngine;
using VirtualOrc.Input;

namespace VirtualOrc.Scripts;

public class VrRig : MonoBehaviour {
    public static VrRig Instance;
    
    public Transform xrOrigin;
    public GameObject xrOffset;
    public GameObject xrHeadset;
    
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

        xrOrigin = transform;
        
        xrOffset = new GameObject("xrOffset");
        xrOffset.transform.SetParent(xrOrigin, false);

        xrHeadset = new GameObject("xrHeadset", typeof(Camera), typeof(AudioListener));
        xrHeadset.transform.SetParent(xrOffset.transform, false);
        
        InitInputActions();
        
        // Move camera and audioListener to new GameObject
        Camera originalCam = GetComponent<Camera>();
        AudioListener originalAudioListener = GetComponent<AudioListener>();
        Camera newCam = Util.CopyComponent(originalCam, xrHeadset);
        AudioListener newAudioListener = Util.CopyComponent(originalAudioListener, xrHeadset);
        
        originalCam.enabled = false;
        originalCam.tag = "Untagged";
        originalAudioListener.enabled = false;
        
        // Hack cinemachine brain to reference the new camera
        CinemachineBrain brain = GetComponent<CinemachineBrain>();
        if (brain != null) {
            typeof(CinemachineBrain).GetField("m_OutputCamera", BindingFlags.NonPublic | BindingFlags.Instance)!.SetValue(brain, newCam);
        }

    }

    private void InitInputActions() {
        // Actions.Load();
        //
        // // Create HMD tracker
        // actions = Actions.VRInputActions;
        // TrackedPoseDriver headsetPoseDriver = xrHeadset.AddComponent<TrackedPoseDriver>();
        // headsetPoseDriver.positionAction = Actions.Head_Position;
        // headsetPoseDriver.rotationAction = Actions.Head_Rotation;
    }

    // private void Update() {
        // Log.Info(Actions.Head_Rotation.ReadValue<Quaternion>());
        // xrHeadset.transform.localPosition = Actions.Head_Position.ReadValue<Vector3>();
        // xrHeadset.transform.localRotation = Actions.Head_Rotation.ReadValue<Quaternion>();
    // }
}