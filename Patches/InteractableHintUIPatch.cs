using HarmonyLib;
using UnityEngine;
using VirtualOrc.Scripts;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class InteractableHintUIPatch {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(InteractableHintUI), "Start")]
    private static void MoveToWorldSpace(InteractableHintUI __instance) {
        __instance.hintPoint.localPosition = Vector3.zero;
            
        Canvas canvas = __instance.GetComponentInChildren<Canvas>(true);
        canvas.gameObject.layer = LayerMask.NameToLayer("UI");
        canvas.renderMode = RenderMode.WorldSpace;
        Transform canvasTransform = canvas.transform;
        canvasTransform.localPosition = Vector3.zero;
        canvasTransform.localScale = Vector3.one;

        var wsCanvasTools = __instance.gameObject.AddComponent<WorldSpaceCanvasTools>();
        wsCanvasTools.canvasTransform = __instance.hintPoint.transform;
    }
}