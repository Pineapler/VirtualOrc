using DG.Tweening;
using HarmonyLib;
using UnityEngine;
using VirtualOrc.Scripts;

namespace VirtualOrc.Patches;

// Includes patches for several UI classes that behave similarly:
// - StartSceneUIManager

[HarmonyPatch]
public class MenuUIPatch {
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartSceneUIManager), "Start")]
    private static void StartSceneToWorldSpace(StartSceneUIManager __instance) {
        RuntimeVRLoaderManager.OnReady(() => {
            Canvas canvas = __instance.StartCanvas.GetComponent<Canvas>();
            canvas.name = "StartSceneCanvas";
            canvas.renderMode = RenderMode.WorldSpace;
            Transform t = canvas.transform;
            t.SetParent(Plugin.VrRig.canvasHolder.transform, false);
            t.localPosition = Vector3.zero;
        });
        
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartSceneUIManager), "Start")]
    private static void AddWorldspaceUIHitbox(StartSceneUIManager __instance) {
        RuntimeVRLoaderManager.OnReady(() => {
            var canvas = __instance.StartCanvas.GetComponent<Canvas>();
            canvas.worldCamera = Plugin.VrInputModule.uiCamera;
            canvas.gameObject.AddComponent<VrUIItem>();
        });
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(StartSceneUIManager), nameof(StartSceneUIManager.UIOnEnter))]
    private static bool UIOnEnterFix(StartSceneUIManager __instance, Transform target) {
        __instance.selected.transform.DOLocalMoveY(target.localPosition.y, 0.1f).SetEase(__instance.moveEase);
        return false;
    }
}