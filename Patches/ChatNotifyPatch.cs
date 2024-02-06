using HarmonyLib;
using UnityEngine;
using VirtualOrc.Scripts;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class ChatNotifyPatch {
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatNotify), "ShowUI")]
    private static void MoveToWorldSpace(ChatNotify __instance) {
        __instance.holder.localPosition = Vector3.zero;

        Canvas canvas = __instance.GetComponentInChildren<Canvas>(true);
        canvas.gameObject.layer = LayerMask.NameToLayer("UI");
        canvas.renderMode = RenderMode.WorldSpace;
        Transform canvasTransform = canvas.transform;
        canvasTransform.localScale = Vector3.one;
        // * scale and rotation is handled by a method patch
        
        var wsCanvasTools = __instance.gameObject.AddComponent<WorldSpaceCanvasTools>();
        wsCanvasTools.canvasTransform = __instance.holder.transform;
    }
    
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatNotify), "FloatOnHead")]
    private static bool ApplyPerspectiveScaleAndBillboard(ChatNotify __instance) {
        Transform transform = __instance.transform;
        transform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        __instance.holder.transform.position = transform.position + Vector3.up * 0.3f;
        // TODO apply scale based on how far the canvas is from the camera
        // TODO apply billboard
        return false;
    }
}