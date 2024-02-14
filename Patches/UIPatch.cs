using System.Reflection;
using System.Runtime.CompilerServices;
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
            canvas.worldCamera = VRInputModule.Instance.laserCamera;
            Transform t = canvas.transform;
            t.SetParent(VRRig.Instance.canvasHolder.transform, false);
            t.localPosition = Vector3.zero;
            
            var wsCanvasTools = canvas.gameObject.AddComponent<WorldSpaceCanvasTools>();
            wsCanvasTools.EnableCollider = true;
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
        wsCanvasTools.TargetRectTransform = __instance.canvas.GetComponent<RectTransform>();
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
        wsCanvasTools.TargetRectTransform = __instance.hintPoint.GetComponent<RectTransform>();
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
            canvas.worldCamera = VRInputModule.Instance.laserCamera;
            Transform t = canvas.transform;
            t.SetParent(VRRig.Instance.canvasHolder.transform, false);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
            
            canvas.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));

            foreach (Transform child in canvas.transform) {
                var wsCanvasTools = child.gameObject.AddComponent<WorldSpaceCanvasTools>();
                wsCanvasTools.EnableCollider = true;
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
            canvas.worldCamera = VRInputModule.Instance.laserCamera;
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
            canvas.worldCamera = VRInputModule.Instance.laserCamera;
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

    // ==========
    // OptionMenu
    // ==========
    #region OptionMenu

    [HarmonyPostfix]
    [HarmonyPatch(typeof(OptionMenu), "Start")]
    private static void OptionMenu_ToWorldSpace(OptionMenu __instance) {
        FieldInfo canvasField = typeof(OptionMenu).GetField("canvas", BindingFlags.Instance | BindingFlags.NonPublic)!;
        
        VRRig.OnReady(() => {
            Canvas pauseCanvas = (canvasField.GetValue(__instance) as Canvas)!;
            Canvas skipCanvas = __instance.skipManga.GetComponent<Canvas>();

            Transform holderT = VRRig.Instance.canvasHolder.transform;
            Transform pauseT = pauseCanvas.transform;
            Transform skipT = skipCanvas.transform;

            pauseT.SetParent(holderT, false);
            pauseCanvas.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
            pauseCanvas.renderMode = RenderMode.WorldSpace;
            pauseCanvas.worldCamera = VRInputModule.Instance.laserCamera;
            pauseT.name = "Pause Canvas";
            pauseT.localPosition = new Vector3(0, 0, -100f);
            pauseT.localRotation = Quaternion.identity;
            pauseT.localScale = Vector3.one;
            var pauseWS = pauseCanvas.gameObject.AddComponent<WorldSpaceCanvasTools>();
            var pauseBG = pauseT.Find("Background") as RectTransform;
            pauseBG.gameObject.SetActive(true);
            pauseWS.TargetRectTransform = pauseBG;
            pauseWS.EnableCollider = true;

            skipT.SetParent(holderT, false);
            skipCanvas.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
            skipCanvas.renderMode = RenderMode.WorldSpace;
            skipCanvas.worldCamera = VRInputModule.Instance.laserCamera;
            skipT.name = "Skip Canvas";
            skipT.localPosition = Vector3.zero;
            skipT.localRotation = Quaternion.identity;
            skipT.localScale = Vector3.one;
            var skipWS = skipCanvas.gameObject.AddComponent<WorldSpaceCanvasTools>();
            skipWS.EnableCollider = true;
        });
    }
    #endregion
    

    // ==============
    // VipMassagePlan
    // ==============
    #region VipMassagePlan
    
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(VipMassagePlan), "Start")]
    private static void VipMassagePlan_ToWorldSpace(VipMassagePlan __instance) {
        VRRig.OnReady(() => {
            Canvas canvas = __instance.GetComponent<Canvas>();
            canvas.name = "VipMassagePlan Canvas";
            canvas.renderMode = RenderMode.WorldSpace;
            canvas.worldCamera = VRInputModule.Instance.laserCamera;
            Transform t = canvas.transform;
            t.SetParent(VRRig.Instance.canvasHolder.transform, false);
            t.localPosition = Vector3.zero;
            t.localRotation = Quaternion.identity;
            t.localScale = Vector3.one;
    
            canvas.gameObject.SetLayerRecursive(LayerMask.NameToLayer("UI"));
    
            foreach (Transform child in __instance.transform) {
                var wsCanvasTools = child.gameObject.AddComponent<WorldSpaceCanvasTools>();
                wsCanvasTools.EnableCollider = true;
            }
        });
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(AnalyzeMassageSelection), "Setup")]
    private static void AnalyzeMassageSelection_Setup(AnalyzeMassageSelection __instance) {
        foreach (var button in __instance.analyzeMassageModeUIs) {
            Transform t = button.transform;
            t.localRotation = Quaternion.identity; 
            t.localPosition = new Vector3(t.localPosition.x, 0, 0);
        }
    }
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(AnalyzeMassageSelection), "OnEnable")]
    // private static bool AnalyzeMassageSelection_OnEnable(AnalyzeMassageSelection __instance) {
    //     RectTransform t = __instance.GetComponent<RectTransform>();
    //     t.localPosition = new Vector3(t.localPosition.x, -708f, 0);
    //     return false;
    // }
    //
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(AnalyzeMassageSelection), nameof(AnalyzeMassageSelection.ShowAvailableMassageModes))]
    // private static bool AnalyzeMassageSelection_ShowAvailableMassageModes(AnalyzeMassageSelection __instance) {
    //     RectTransform t = __instance.GetComponent<RectTransform>();
    //     t.DOKill();
    //     __instance.holder.SetActive(true);
    //     t.DOLocalMoveY(-508f, 0.5f);
    //     return false;
    // }
    //
    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(AnalyzeMassageSelection), nameof(AnalyzeMassageSelection.HideUI))]
    // private static bool AnalyzeMassageSelection_HideUI(AnalyzeMassageSelection __instance) {
    //     RectTransform t = __instance.GetComponent<RectTransform>();
    //     t.DOLocalMoveY(-708f, 0.5f).OnComplete(() => __instance.holder.SetActive(false));
    //     return false;
    // }
    
    #endregion
    
}