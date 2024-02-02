using System;
using HarmonyLib;
using Kuro;
using Pineapler.Utils;
using UnityEngine;
using VirtualOrc.Scripts;
using Object = UnityEngine.Object;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class LogSystemManagerPatch {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogSystemMananger), "Awake")]
    private static void InsertVrRig(LogSystemMananger __instance) {
        Log.Warning("Disabling LogSystemManager::Training, as its UI is broken in VR. This may be fixed later.");
        __instance.transform.Find("Training").gameObject.SetActive(false);
    }

}