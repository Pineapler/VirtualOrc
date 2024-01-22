using HarmonyLib;
using UnityEngine;
using VirtualOrc.Scripts;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class CameraManagerPatch {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CameraManager), nameof(CameraManager.Start))]
    private static void InsertVrRig(CameraManager __instance) {
        
        if (Plugin.VrLoaderManager == null) {
            // XrLoaderManager will start a coroutine once it has spawned in
            Plugin.VrLoaderManager = new GameObject("XR Loader Manager").AddComponent<RuntimeVRLoaderManager>();
            Object.DontDestroyOnLoad(Plugin.VrLoaderManager.gameObject);
        }
        
        
        __instance.MainCam.AddComponent<VrRig>();
        // __instance.PushCam.AddComponent<VrRig>();
    }
}