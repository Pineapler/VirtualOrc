using System;
using HarmonyLib;
using Kuro;
using Pineapler.Utils;
using UnityEngine;
using VirtualOrc.Scripts;
using Object = UnityEngine.Object;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class CameraManagerPatch {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CameraManager), nameof(CameraManager.Start))]
    private static void InsertVrRig(CameraManager __instance) {
        
        if (Plugin.VrLoaderManager == null) {
            RuntimeVRLoaderManager.rigTarget = __instance.MainCam;
                
            // XrLoaderManager will start a coroutine once it has spawned in
            Plugin.VrLoaderManager = new GameObject("XR Loader Manager").AddComponent<RuntimeVRLoaderManager>();
            Object.DontDestroyOnLoad(Plugin.VrLoaderManager.gameObject);
        }
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CameraManager), nameof(CameraManager.SetActiveCamera))]
    private static void SetActiveCameraDebug(CameraManager __instance, GameCameraType ActiveCamera) {
        Log.Info($"SetActiveCamera({ActiveCamera})");
        Log.Info($"- Main cam: {__instance.MainCam}");
        Log.Info($"- Orbit: {__instance.MainCam.GetComponent<CameraOrbit>()}");
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CameraManager), nameof(CameraManager.SetActiveCamera))]
    private static void RecentreOnSetActiveCamera(CameraManager __instance, GameCameraType ActiveCamera) {
        if (VrRig.Instance == null) return;
        
        VrRig.Instance.Recentre();
    }
}