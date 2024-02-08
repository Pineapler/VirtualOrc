using System.Reflection;
using HarmonyLib;
using Kuro;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using Input = VirtualOrc.Scripts.Input;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class MouseCursorManagerPatch {

    public static PropertyInfo hitProp;
    public static PropertyInfo lastRaycastableProp;
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MouseCursorManager), "MassageInput")]
    private static bool MassageInputVR(MouseCursorManager __instance) {
        if (!Config.EnableLaserInput || Plugin.VrInputModule == null) return true;
        
        Log.Info("Hello");

        if (hitProp == null) {
            hitProp = typeof(MouseCursorManager).GetProperty("hit", BindingFlags.NonPublic | BindingFlags.Instance)!;
            lastRaycastableProp = typeof(MouseCursorManager).GetProperty("lastRaycastable", BindingFlags.Public | BindingFlags.Instance)!;
        }

        Transform laserT = Plugin.VrInputModule.activeLaser.transform;
        Ray ray = new Ray(laserT.position, laserT.forward);
        RaycastHit hitTemp;
        if (Physics.Raycast(ray, out hitTemp, 100f, (int)__instance.mask) &&
            !EventSystem.current.IsPointerOverGameObject()) {

            hitProp.SetValue(__instance, hitTemp);

            ISelectable component;
            if (!hitTemp.collider.TryGetComponent(out component)) {
                return false;
            }

            if (__instance.lastRaycastable != null) {
                __instance.lastRaycastable.Exit();
            }
            lastRaycastableProp.SetValue(__instance, component);
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
            lastRaycastableProp.SetValue(__instance, null);
            MouseCursorManager.SwitchState(MouseCursorManager.MouseState.Normal);
        }

        return false;
    }
    
}