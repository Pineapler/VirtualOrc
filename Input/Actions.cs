
namespace VirtualOrc.Input;

public class Actions {
    // public static InputAction Head_Position;
    // public static InputAction Head_Rotation;
    // // public static InputAction Head_TrackingState; // Tracking state not available in InputSystem 1.1's TrackedPoseDriver
    //
    // public static InputAction LeftHand_Position;
    // public static InputAction LeftHand_Rotation;
    // // public static InputAction LeftHand_TrackingState;
    //
    // public static InputAction RightHand_Position;
    // public static InputAction RightHand_Rotation;
    // // public static InputAction RightHand_TrackingState;
    //
    // public static InputActionAsset VRInputActions;
    //
    //
    // public static void Load() {
    //     Log.Info("Initializing InputSystem");
    //     // typeof(InputSystem).GetMethod("PerformDefaultPluginInitialization", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, []);
    //     typeof(InputSystem).GetMethod("RunInitializeInPlayer", BindingFlags.NonPublic | BindingFlags.Static)!.Invoke(null, []);
    //     
    //     Log.Info($"Loading VR actions: \n{Properties.Resources.vr_inputs}");
    //     VRInputActions = InputActionAsset.FromJson(Properties.Resources.vr_inputs);
    //     
    //     
    //     Head_Position = VRInputActions.FindAction("head/position");
    //     if (Head_Position == null) Log.Error("Could not find HeadPosition action!");
    //     Head_Rotation = VRInputActions.FindAction("head/rotation");
    //     if (Head_Rotation == null) Log.Error("Could not find HeadRotation action!");
    //     // Head_TrackingState = VRInputActions.FindAction("Head/TrackingState");
    //     
    //     // LeftHand_Position = VRInputActions.FindAction("LeftHand/Position");
    //     // LeftHand_Rotation = VRInputActions.FindAction("LeftHand/Rotation");
    //     // LeftHand_TrackingState = VRInputActions.FindAction("LeftHand/TrackingState");
    //     
    //     // RightHand_Position = VRInputActions.FindAction("RightHand/Position");
    //     // RightHand_Rotation = VRInputActions.FindAction("RightHand/Rotation");
    //     // RightHand_TrackingState = VRInputActions.FindAction("RightHand/TrackingState");
    //
    //     VRInputActions.Enable();
    //
    // }
}