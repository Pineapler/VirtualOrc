using Valve.VR;

namespace VirtualOrc.Scripts;

public static class Input {

    public static SteamVR_Action_Boolean interact;
    public static SteamVR_Action_Boolean interactSecondary;
    public static SteamVR_Action_Boolean move;
    public static SteamVR_Action_Boolean radialMenu;
    public static SteamVR_Action_Boolean toggleUI;
    public static SteamVR_Action_Boolean pause;

    public static void Init() {
        interact = SteamVR_Actions.default_Interact;
        interactSecondary = SteamVR_Actions.default_InteractSecondary;
        move = SteamVR_Actions.default_GrabMove;
        radialMenu = SteamVR_Actions.default_RadialMenu;
        toggleUI = SteamVR_Actions.default_ToggleUI;
        pause = SteamVR_Actions.default_Pause;
    }

    public static void UnsubscribeAllListeners() {
        interact.RemoveAllListeners(SteamVR_Input_Sources.Any);
        interactSecondary.RemoveAllListeners(SteamVR_Input_Sources.Any);
        move.RemoveAllListeners(SteamVR_Input_Sources.Any);
        radialMenu.RemoveAllListeners(SteamVR_Input_Sources.Any);
        toggleUI.RemoveAllListeners(SteamVR_Input_Sources.Any);
        pause.RemoveAllListeners(SteamVR_Input_Sources.Any);
    }
    
    public static bool InteractLaserPressed() {
        if (VRInputModule.Instance == null) return false;
        return interact.GetStateDown(VRInputModule.Instance.targetSource);
    }
}