using HarmonyLib;
using Pineapler.Utils;
using UnityEngine;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class ZibraLiquidToolPatch {

    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "Awake")]
    private static bool DestroyZibraLiquids(ZibraLiquidTool __instance) {
        Log.Warning("Destroying ZibraLiquid objects as they crash the game in VR.");
        
        foreach (Transform child in __instance.transform) {
            Object.Destroy(child.gameObject);
        }

        ZibraLiquidTool.Instance = __instance;
        return false;
    }
    
    // Bypass all instance methods
    // ===========================
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "SetMassageShopWater")]
    private static bool BypassSetMassageShopWater() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "SetOrcSperm")]
    private static bool BypassSetOrcSperm() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "SetupOrcLiquid")]
    private static bool BypassSetupOrcLiquid() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "AddCollidersToOrcLiquid")]
    private static bool BypassAddCollidersToOrcLiquid() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "RemoveFromOrcLiquid")]
    private static bool BypassRemoveFromOrcLiquid() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "AddColliders")]
    private static bool BypassAddColliders() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "RemoveColliders")]
    private static bool BypassRemoveColliders() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "StopOrcLiquid")]
    private static bool BypassStopOrcLiquid() {
        return false;
    }
}