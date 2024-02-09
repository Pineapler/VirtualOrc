using DG.Tweening;
using HarmonyLib;
using Pineapler.Utils;
using UnityEngine;
using VirtualOrc.Scripts;

namespace VirtualOrc.Patches;

// Includes patches for several UI classes that behave similarly:
// - StartSceneUIManager

[HarmonyPatch]
public class UIPatch {

    // ===============
    // TEMPORARY FIXES
    // ===============
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogSystemMananger), "Awake")]
    private static void LogSystemManager_DisableBrokenUI(LogSystemMananger __instance) {
        // TODO: Find out what controls SkillUI.cs
        Log.Warning("Disabling LogSystemManager::Training, as its UI is broken in VR. This may be fixed later.");
        __instance.transform.Find("Training").gameObject.SetActive(false);
    }
    // ################
    
    
    // ======================
    // StartSceneUIManager.cs
    // ======================
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartSceneUIManager), "Start")]
    private static void StartScene_ToWorldSpace(StartSceneUIManager __instance) {
        VRRig.OnReady(() => {
            Canvas canvas = __instance.StartCanvas.GetComponent<Canvas>();
            canvas.name = "StartSceneCanvas";
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = VRInputModule.Instance.uiCamera;
            Transform t = canvas.transform;
            t.SetParent(VRRig.Instance.canvasHolder.transform, false);
            t.localPosition = Vector3.zero;
            
            var wsCanvasTools = canvas.gameObject.AddComponent<WorldSpaceCanvasTools>();
            wsCanvasTools.colliderEnabled = true;
        });
        
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(StartSceneUIManager), nameof(StartSceneUIManager.UIOnEnter))]
    private static bool StartScene_SelectedUIFix(StartSceneUIManager __instance, Transform target) {
        __instance.selected.transform.DOLocalMoveY(target.localPosition.y, 0.1f).SetEase(__instance.moveEase);
        return false;
    }
    
    
    // =============
    // ChatNotify.cs
    // =============
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatNotify), "ShowUI")]
    private static void ChatNotify_MoveToWorldSpace(ChatNotify __instance) {
        __instance.transform.localScale = Vector3.one;
        __instance.holder.localPosition = Vector3.zero;

        __instance.canvas.gameObject.layer = LayerMask.NameToLayer("UI");
        __instance.canvas.renderMode = RenderMode.WorldSpace;
        Transform canvasTransform = __instance.canvas.transform;
        canvasTransform.localScale = Vector3.one;
        // * scale and rotation is handled by a method patch
        
        var wsCanvasTools = __instance.canvas.gameObject.AddComponent<WorldSpaceCanvasTools>();
        wsCanvasTools.canvasRectTransform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        wsCanvasTools.canvasRectTransform.localPosition = Vector3.zero;
        wsCanvasTools.enableBillboard = true;
        // wsCanvasTools.enablePerspectiveScale = true;
        // wsCanvasTools.perceivedScale = new Vector3(0.005f, 0.005f, 1);
    }
    
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ChatNotify), "FloatOnHead")]
    private static bool ChatNotify_FloatOnHead(ChatNotify __instance) {
        __instance.holder.position = __instance.transform.position + Vector3.up * 0.3f;
        return false;
    }
    
    
    // =====================
    // InteractableHintUI.cs
    // =====================
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(InteractableHintUI), "Start")]
    private static void InteractableHintUI_ToWorldSpace(InteractableHintUI __instance) {
            
        Canvas canvas = __instance.GetComponentInChildren<Canvas>(true);
        canvas.gameObject.layer = LayerMask.NameToLayer("UI");
        canvas.renderMode = RenderMode.WorldSpace;
        Transform canvasTransform = canvas.transform;
        canvasTransform.localPosition = Vector3.zero;
        canvasTransform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        var wsCanvasTools = __instance.gameObject.AddComponent<WorldSpaceCanvasTools>();
        wsCanvasTools.canvasRectTransform = __instance.hintPoint.GetComponent<RectTransform>();
        wsCanvasTools.enableBillboard = true;
        // wsCanvasTools.enablePerspectiveScale = true;
        // wsCanvasTools.perceivedScale = new Vector3(0.005f, 0.005f, 1);
        __instance.hintPoint.localPosition = Vector3.zero;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(InteractableHintUI), "ShowInScreenPoint")]
    private static bool InteractableHintUI_BypassShowInScreenPoint() {
        return false;
    }
}