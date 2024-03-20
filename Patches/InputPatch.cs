using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using DG.Tweening;
using HarmonyLib;
using Kuro;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Valve.VR;
using Valve.VR.InteractionSystem.Sample;
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
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouseCursorManager), "MassageInput")]
    private static bool MouseCursorManager_MassageInputVR(MouseCursorManager __instance) {
        Log.Info("MouseCursor MassageInput");
        if (VRInputModule.Instance == null || !VRInputModule.Instance.isLaserActive) return true;
        

        Transform laserT = VRInputModule.Instance.activeLaser.transform;
        Ray ray = new Ray(laserT.position, laserT.forward);
        RaycastHit hitTemp;
        if (Physics.Raycast(ray, out hitTemp, 100f, (int)__instance.mask) &&
            !EventSystem.current.IsPointerOverGameObject()) {

            TypeInfos.MouseCursorManager_Hit.SetValue(__instance, hitTemp);

            ISelectable component;
            if (!hitTemp.collider.TryGetComponent(out component)) {
                return false;
            }

            if (__instance.lastRaycastable != null) {
                __instance.lastRaycastable.Exit();
            }
            TypeInfos.MouseCursorManager_LastRaycastable.SetValue(__instance, component);
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
            TypeInfos.MouseCursorManager_LastRaycastable.SetValue(__instance, null);
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
    #endregion


    [HarmonyPrefix]
    [HarmonyPatch(typeof(GMState_VipMassageEnd), "WaitForInput")]
    private static bool GMState_VipMassageEnd_WaitForInputPatch(GMState_VipMassageEnd __instance) {
        if (!Input.interact.GetState(SteamVR_Input_Sources.Any)) {
            return false;
        }

        UnityAction temp = TypeInfos.GMState_VipMassageEnd_UpdateEvent.GetValue(__instance) as UnityAction;
        temp -= __instance.WaitForInput;
        TypeInfos.GMState_VipMassageEnd_UpdateEvent.SetValue(__instance, temp);

        if (!NewGM.Instance.DebugMode) {
            VipMassageGameSystem.Instance.resultUI.HideUI();
            BodyStateStatus.Instance.analyzePanelUI.HideUI();
        }
        TalkManager.Instance.SetConverSation_New(GameManager.Instance.NowClient, ConverSationType.type.Exit, 0);
        return false;
    }
}