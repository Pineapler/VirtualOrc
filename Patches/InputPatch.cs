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
            Input.InteractLaserPressed() &&
            !EventSystem.current.IsPointerOverGameObject()) {

            if (__instance.hit.collider.TryGetComponent(out __instance.LastworldSpaceButton)) {
                __instance.LastworldSpaceButton.Down();
                if (__instance.LastworldSpaceButton.HasWindow) {
                    __instance.gm.SetGameMode(GameMode.mode.OnWindow);
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
    
    
    // ==================
    // MouseCursorManager
    // ==================
    
    
    public static PropertyInfo mcm_hitProp;
    public static PropertyInfo mcm_lastRaycastableProp;
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouseCursorManager), "MassageInput")]
    private static bool MouseCursorManager_MassageInputVR(MouseCursorManager __instance) {
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
}