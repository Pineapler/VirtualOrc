using HarmonyLib;
using VirtualOrc.Scripts;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class CameraManagerPatch {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CameraManager), nameof(CameraManager.Start))]
    private static void InsertVrRig(CameraManager __instance) {
        __instance.MainCam.AddComponent<VrRig>();
        // __instance.PushCam.AddComponent<VrRig>();
    }
}