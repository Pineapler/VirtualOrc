using System.Reflection;
using HarmonyLib;
using Kuro;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using VirtualOrc.Scripts;
using Input = VirtualOrc.Scripts.Input;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class InputPatch {
    // TODO: These should probably be replaced with hand tracked stuff

    // ============
    // InputManager
    // ============
    #region InputManager
    [HarmonyPrefix]
    [HarmonyPatch(typeof(InputManager), "SexInput")]
    private static bool InputManager_SexInputVR(InputManager __instance) {
        // Fallback to original implementation
        if (VRInputModule.Instance == null || !VRInputModule.Instance.isLaserActive) return true;
        
        // Same as original, but replace screen point with a raycast from the laser,
        // and check for interact action instead of mouse.
        Transform laserT = VRInputModule.Instance.activeLaser.transform;
        Ray ray = new Ray(laserT.position, laserT.forward);
        if (Physics.Raycast(ray, out __instance.hit, 100f, (int)__instance.SexSceneMask) &&
            Input.InteractLaserPressed() &&
            !EventSystem.current.IsPointerOverGameObject()) {

            __instance.LastworldSpaceButton = __instance.hit.collider.GetComponent<WorldSpaceButton>();
            __instance.LastworldSpaceButton.Down();
        }

        if (__instance.hit.collider == __instance.LastHit) {
            return false;
        }

        if (__instance.hit.collider != null) {
            __instance.hit.collider.SendMessage("Enter", SendMessageOptions.DontRequireReceiver);
        }
        if (__instance.LastHit != null) {
            __instance.LastHit.SendMessage("Exit", SendMessageOptions.DontRequireReceiver);
        }

        __instance.LastHit = __instance.hit.collider;
        return false;
    }
   
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(InputManager), "OperatingInput")]
    private static bool InputManager_OperatingInputVR(InputManager __instance) {
        if (VRInputModule.Instance == null || !VRInputModule.Instance.isLaserActive) return true;
        
        Transform laserT = VRInputModule.Instance.activeLaser.transform;
        Ray ray = new Ray(laserT.position, laserT.forward);
        if (Physics.Raycast(ray, out __instance.hit, 100f, (int)__instance.mask) &&
            !EventSystem.current.IsPointerOverGameObject()) {

            if (Input.InteractLaserPressed()) {
                if (__instance.hit.collider.TryGetComponent(out __instance.LastworldSpaceButton)) {
                    __instance.LastworldSpaceButton.Down();
                    if (__instance.LastworldSpaceButton.HasWindow) {
                        __instance.gm.SetGameMode(GameMode.mode.OnWindow);
                    }
                }
            }

            if (__instance.hit.collider == __instance.LastHit) {
                return false;
            }
            if (__instance.hit.collider != null) {
                __instance.hit.collider.SendMessage("Enter", SendMessageOptions.DontRequireReceiver);
            }
            if (__instance.LastHit != null) {
                __instance.LastHit.SendMessage("Exit", SendMessageOptions.DontRequireReceiver);
            }

            __instance.LastHit = __instance.hit.collider;
        }
        else {
            if (__instance.LastHit == null) {
                return false;
            }
            __instance.LastHit.SendMessage("Exit", SendMessageOptions.DontRequireReceiver);
            __instance.LastHit = null;
        }

        return false;
    }
    
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(InputManager), "MassaggingInput")]
    private static bool InputManager_MassagingInputVR(InputManager __instance) {
        if (VRInputModule.Instance == null || !VRInputModule.Instance.isLaserActive) return true;
        
        Transform laserT = VRInputModule.Instance.activeLaser.transform;
        Ray ray = new Ray(laserT.position, laserT.forward);
        if (Physics.Raycast(ray, out __instance.hit, 100f, (int)__instance.MassageGameMask) &&
            Input.InteractLaserPressed() &&
            !EventSystem.current.IsPointerOverGameObject()) {

            CharacterManager.Instance.SetPlayerAnimatorLayerWeight(2, 1f);
        }
        
        return false;
    }
    #endregion
    
    // ==================
    // MouseCursorManager
    // ==================
    #region MouseCursorManager
    public static PropertyInfo mcm_hitProp;
    public static PropertyInfo mcm_lastRaycastableProp;
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouseCursorManager), "MassageInput")]
    private static bool MouseCursorManager_MassageInputVR(MouseCursorManager __instance) {
        Log.Info("MouseCursor MassageInput");
        if (VRInputModule.Instance == null || !VRInputModule.Instance.isLaserActive) return true;
        
        Log.Info("Hello");

        if (mcm_hitProp == null) {
            mcm_hitProp = typeof(MouseCursorManager).GetProperty("hit", BindingFlags.NonPublic | BindingFlags.Instance)!;
            mcm_lastRaycastableProp = typeof(MouseCursorManager).GetProperty("lastRaycastable", BindingFlags.Public | BindingFlags.Instance)!;
        }

        Transform laserT = VRInputModule.Instance.activeLaser.transform;
        Ray ray = new Ray(laserT.position, laserT.forward);
        RaycastHit hitTemp;
        if (Physics.Raycast(ray, out hitTemp, 100f, (int)__instance.mask) &&
            !EventSystem.current.IsPointerOverGameObject()) {

            mcm_hitProp.SetValue(__instance, hitTemp);

            ISelectable component;
            if (!hitTemp.collider.TryGetComponent(out component)) {
                return false;
            }

            if (__instance.lastRaycastable != null) {
                __instance.lastRaycastable.Exit();
            }
            mcm_lastRaycastableProp.SetValue(__instance, component);
            __instance.lastRaycastable!.Enter();
            
            MouseCursorManager.SwitchState(MouseCursorManager.MouseState.Hovering);
            if (!Input.InteractLaserPressed()) {
                return false;
            }

            __instance.lastRaycastable.Select();
            MouseCursorManager.SwitchState(MouseCursorManager.MouseState.Pressing);
        }
        else {
            if (__instance.lastRaycastable == null) {
                return false;
            }

            __instance.lastRaycastable.Exit();
            mcm_lastRaycastableProp.SetValue(__instance, null);
            MouseCursorManager.SwitchState(MouseCursorManager.MouseState.Normal);
        }

        return false;
    }
    #endregion
    
    
    // ===========
    // TalkManager
    // ===========
    #region TalkManager
    [HarmonyPrefix]
    [HarmonyPatch(typeof(TalkManager), "Update")]
    private static bool TalkManager_Update(TalkManager __instance) {
        if (VRInputModule.Instance == null || !VRInputModule.Instance.isLaserActive) return true;

        if (__instance.talkWindow.gameObject.activeInHierarchy && Input.InteractLaserPressed()) {
            __instance.StartCoroutine("NextEvent");
        }
        return false;
    }
    #endregion
    
    
    // ===============
    // OrcTouchingHand
    // ===============
    #region OrcTouchingHand
    [HarmonyPostfix]
    [HarmonyPatch(typeof(OrcTouchingHand), "Awake")]
    private static void OrcTouchingHand_FollowController(OrcTouchingHand __instance) {
        // VRRig.OnReady(() => {
        //     __instance.orcHand.SetParent(VRRig.Instance.rigOffset, false);
        //     __instance.orcHand.localPosition = Vector3.zero;
        //     __instance.orcHand.localRotation = Quaternion.identity;
        // });
        __instance.gameObject.AddComponent<VRHand>();
    }
    
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(OrcTouchingHand), "CheckMouse_New")]
    private static bool OrcTouchingHand_CheckMouse_New(OrcTouchingHand __instance) {
        return false;
        
        // if (!Plugin.Config.vrInputEnabled) {
        //     return true; // use default implementation
        // }
        //
        // // TODO: need to know which controller the hand is attached to
        // bool isGrabbing = Input.interact.state;
        //
        // return false;
    }


    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(OrcTouchingHand), "OrchHandPuppetMasterUpdate")]
    // private static bool OrcTouchingHand_PuppetMasterUpdate(OrcTouchingHand __instance) {
    //     Transform controller = VRRig.Instance.rightController.transform;
    //     __instance.PointTarget.position = controller.position;
    //     __instance.PointTarget.rotation = controller.rotation;
    //     return true;
    // }
    #endregion
}