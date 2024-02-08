using HarmonyLib;
using Kuro;
using UnityEngine;
using UnityEngine.EventSystems;
using VirtualOrc.Scripts;
using Input = VirtualOrc.Scripts.Input;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class InputManagerPatch {
    // TODO: These should probably be replaced with hand tracked stuff


    [HarmonyPrefix]
    [HarmonyPatch(typeof(InputManager), "SexInput")]
    private static bool SexInputVR(InputManager __instance) {
        
        // Fallback to original implementation
        if (!Config.EnableLaserInput || Plugin.VrInputModule == null) return true;
        
        // Same as original, but replace screen point with a raycast from the laser,
        // and check for interact action instead of mouse.
        Transform laserT = Plugin.VrInputModule.activeLaser.transform;
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
    private static bool OperatingInputVR(InputManager __instance) {
        if (!Config.EnableLaserInput || Plugin.VrInputModule == null) return true;
        
        Transform laserT = Plugin.VrInputModule.activeLaser.transform;
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
    private static bool MassagingInputVR(InputManager __instance) {
        if (!Config.EnableLaserInput || Plugin.VrInputModule == null) return true;
        
        Transform laserT = Plugin.VrInputModule.activeLaser.transform;
        Ray ray = new Ray(laserT.position, laserT.forward);
        if (Physics.Raycast(ray, out __instance.hit, 100f, (int)__instance.MassageGameMask) &&
            Input.InteractLaserPressed() &&
            !EventSystem.current.IsPointerOverGameObject()) {

            CharacterManager.Instance.SetPlayerAnimatorLayerWeight(2, 1f);
        }
        
        return false;
    }
    
    
}