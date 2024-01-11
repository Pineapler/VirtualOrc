using UnityEngine;
using UnityEngine.InputSystem;

namespace VirtualOrc.Input;

public class Actions {
    public static InputAction Head_Position;
    public static InputAction Head_Rotation;
    // public static InputAction Head_TrackingState; // Tracking state not available in InputSystem 1.1's TrackedPoseDriver
    
    public static InputAction LeftHand_Position;
    public static InputAction LeftHand_Rotation;
    // public static InputAction LeftHand_TrackingState;
    
    public static InputAction RightHand_Position;
    public static InputAction RightHand_Rotation;
    // public static InputAction RightHand_TrackingState;
    
    public static InputActionAsset VRInputActions;


    public static void Load() {
        VRInputActions = InputActionAsset.FromJson(Properties.Resources.vr_inputs);
        
        VRInputActions.LoadFromJson(Properties.Resources.vr_inputs);
        
        Log.Info("Loading VR actions");

        foreach (var map in VRInputActions.actionMaps) {
            foreach (var action in map.actions) {
                Log.Info($"{map.name} > {action.name}");
            }
        }
        
        Head_Position = VRInputActions.FindAction("Head/Position");
        Head_Rotation = VRInputActions.FindAction("Head/Rotation");
        if (Head_Position == null) Log.Error("Could not find HeadPosition action!");
        if (Head_Rotation == null) Log.Error("Could not find HeadRotation action!");
        // Head_TrackingState = VRInputActions.FindAction("Head/TrackingState");
        
        LeftHand_Position = VRInputActions.FindAction("LeftHand/Position");
        LeftHand_Rotation = VRInputActions.FindAction("LeftHand/Rotation");
        // LeftHand_TrackingState = VRInputActions.FindAction("LeftHand/TrackingState");
        
        RightHand_Position = VRInputActions.FindAction("RightHand/Position");
        RightHand_Rotation = VRInputActions.FindAction("RightHand/Rotation");
        // RightHand_TrackingState = VRInputActions.FindAction("RightHand/TrackingState");

        VRInputActions.Enable();

    }
}