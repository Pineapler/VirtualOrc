using DG.Tweening;
using HarmonyLib;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.UIElements;
using VirtualOrc.Scripts;

namespace VirtualOrc.Patches;

// Includes patches for several UI classes that behave similarly:
// - StartSceneUIManager

[HarmonyPatch]
public class UIPatch {

    // ===============
    // TEMPORARY FIXES
    // ===============
    #region TEMPORARY FIXES
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogSystemMananger), "Awake")]
    private static void LogSystemManager_DisableBrokenUI(LogSystemMananger __instance) {
        // TODO: Find out what controls SkillUI.cs
        Log.Warning("Disabling LogSystemManager::Training, as its UI is broken in VR. This may be fixed later.");
        __instance.transform.Find("Training").gameObject.SetActive(false);
    }
    #endregion
    // ################
    
    
    // ======================
    // StartSceneUIManager.cs
    // ======================
    #region StartSceneUIManager
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
    #endregion
    
    // =============
    // ChatNotify.cs
    // =============
    #region ChatNotify
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ChatNotify), "ShowUI")]
    private static void ChatNotify_MoveToWorldSpace(ChatNotify __instance) {
        Transform t = __instance.transform;
        t.localRotation = Quaternion.identity;
        t.localScale = Vector3.one;
        
        __instance.holder.localPosition = Vector3.zero;
        __instance.holder.localRotation = Quaternion.identity;
        __instance.holder.localScale = Vector3.one;

        __instance.canvas.gameObject.layer = LayerMask.NameToLayer("UI");
        __instance.canvas.renderMode = RenderMode.WorldSpace;
        
        Transform canvasTransform = __instance.canvas.transform;
        canvasTransform.localPosition = Vector3.zero;
        canvasTransform.localRotation = Quaternion.identity;
        canvasTransform.localScale = new Vector3(0.005f, 0.005f, 0.005f);
        
        var wsCanvasTools = __instance.canvas.gameObject.AddComponent<WorldSpaceCanvasTools>();
        // wsCanvasTools.targetRectTransform = __instance.holder.GetComponent<RectTransform>();
        wsCanvasTools.targetRectTransform = __instance.canvas.GetComponent<RectTransform>();
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
    #endregion
    
    
    // =====================
    // InteractableHintUI.cs
    // =====================
    #region InteractableHintUI
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
        wsCanvasTools.targetRectTransform = __instance.hintPoint.GetComponent<RectTransform>();
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
    #endregion
   
    
    // ===========
    // GameManager
    // ===========
    #region GameManager
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), "Start")]
    private static void GameManager_ToWorldSpace(GameManager __instance) {
        VRRig.OnReady(() => {
            Canvas canvas = UIManager.Instance.PhoneWindow.transform.parent.GetComponentInChildren<Canvas>(); // jesus christ
            canvas.name = "GameManager Canvas";
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = VRInputModule.Instance.uiCamera;
            Transform t = canvas.transform;
            t.SetParent(VRRig.Instance.canvasHolder.transform, false);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            
            canvas.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));

            foreach (Transform child in canvas.transform) {
                var wsCanvasTools = child.gameObject.AddComponent<WorldSpaceCanvasTools>();
                wsCanvasTools.colliderEnabled = true;
            }
        });
    }
    
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameManager), "Start")]
    private static void GameManager_ToWorldSpace_TalkCanvas(GameManager __instance) {
        VRRig.OnReady(() => {
            Canvas canvas = __instance.GetComponent<TalkManager>().talkWindow.transform.parent.GetComponent<Canvas>(); // jesus christ
            canvas.name = "TalkWindow Canvas";
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = VRInputModule.Instance.uiCamera;
            Transform t = canvas.transform;
            t.SetParent(VRRig.Instance.canvasHolder.transform, false);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            
            canvas.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));

            // var wsCanvasTools = canvas.gameObject.AddComponent<WorldSpaceCanvasTools>();
            // wsCanvasTools.colliderEnabled = true;
        });
    }
    #endregion
    
    
    // ===========
    // DialogueManager
    // ===========
    #region DialogueManager
    [HarmonyPostfix]
    [HarmonyPatch(typeof(DialogueManager), "Awake")]
    private static void DialogueManager_ToWorldSpace(DialogueManager __instance) {
        VRRig.OnReady(() => {
            Canvas canvas = __instance.GetComponent<Canvas>();
            canvas.name = "DialogueManager Canvas";
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = VRInputModule.Instance.uiCamera;
            Transform t = canvas.transform;
            t.SetParent(VRRig.Instance.canvasHolder.transform, false);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;

            canvas.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
            
            // foreach (Transform child in __instance.dialogueSelection) {
            //     var wsCanvasTools = child.gameObject.AddComponent<WorldSpaceCanvasTools>();
            //     wsCanvasTools.colliderEnabled = true;
            // }
        });
    }
    #endregion


    // ==============
    // VipMassagePlan
    // ==============
    #region VipMassagePlan
    //
    //
    // [HarmonyPostfix]
    // [HarmonyPatch(typeof(VipMassagePlan), "Start")]
    // private static void VipMassagePlan_ToWorldSpace(VipMassagePlan __instance) {
    //     VRRig.OnReady(() => {
    //         Canvas canvas = __instance.GetComponent<Canvas>();
    //         canvas.name = "VipMassagePlan Canvas";
    //         canvas.renderMode = RenderMode.WorldSpace;
    //         canvas.worldCamera = VRInputModule.Instance.uiCamera;
    //         Transform t = canvas.transform;
    //         t.SetParent(VRRig.Instance.canvasHolder.transform, false);
    //         t.localPosition = Vector3.zero;
    //         t.localRotation = Quaternion.identity;
    //         t.localScale = Vector3.one;
    //
    //         canvas.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
    //
    //         foreach (Transform child in __instance.transform) {
    //             var wsCanvasTools = child.gameObject.AddComponent<WorldSpaceCanvasTools>();
    //             wsCanvasTools.colliderEnabled = true;
    //         }
    //     });
    // }
    //
    // [HarmonyPostfix]
    // [HarmonyPatch(typeof(AnalyzeMassageSelection), "Setup")]
    // private static void AnalyzeMassageSelection_FixPosition(AnalyzeMassageSelection __instance) {
    //     foreach (var child in __instance.analyzeMassageModeUIs) {
    //         Transform t = child.transform;
    //         Vector3 tempPos = t.localPosition;
    //         t.localPosition = new Vector3(tempPos.x, tempPos.y, 0);
    //     }
    // }
    //
    #endregion
    
}