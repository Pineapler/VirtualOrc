using System;
using HarmonyLib;
using Kuro;
using Pineapler.Utils;
using UnityEngine;
using VirtualOrc.Scripts;
using Object = UnityEngine.Object;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class EntryPointPatch {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), "Start")]
    private static void InsertEntryPoint(GameManager __instance) {
        Log.Info("Entry point inserting");
        if (RuntimeVRLoader.Instance == null) {
            // Starts a coroutine once it has spawned in
            var loader = new GameObject("Runtime VR Loader", typeof(RuntimeVRLoader));
            Object.DontDestroyOnLoad(loader);
        }
        else {
            Log.Info("  VR already loaded");
        }

        if (SceneEntryPoint.Instance == null) {
            var sceneEntryPoint = new GameObject("VirtualOrc Scene Entry Point", typeof(SceneEntryPoint));
        }
        else {
            Log.Info("  Entry point already exists");
        }
        
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CameraManager), nameof(CameraManager.SetActiveCamera))]
    private static void RecenterOnSetActiveCamera(CameraManager __instance, GameCameraType ActiveCamera) {
        if (VRRig.Instance == null) return;
        VRRig.Instance.Recenter();
    }

    // ======= DEBUG PATCHES =========
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(CameraManager), nameof(CameraManager.SetActiveCamera))]
    // private static void SetActiveCameraDebug(CameraManager __instance, GameCameraType ActiveCamera) {
    //     Log.Info($"SetActiveCamera({ActiveCamera})");
    //     Log.Info($"- Main cam: {__instance.MainCam}");
    //     Log.Info($"- Orbit: {__instance.MainCam.GetComponent<CameraOrbit>()}");
    // }

}