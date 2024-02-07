using HarmonyLib;
using UnityEngine;
using UnityEngine.UI;
using VirtualOrc.Scripts;

namespace VirtualOrc.Patches;

// Includes patches for several UI classes that behave similarly:
// - StartSceneUIManager

[HarmonyPatch]
public class MenuUIPatch {
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartSceneUIManager), "Start")]
    private static void StartSceneToWorldSpace(StartSceneUIManager __instance) {
        VrRig.OnReady(() => {
            Canvas canvas = __instance.StartCanvas.GetComponent<Canvas>();
            canvas.name = "StartSceneCanvas";
            canvas.renderMode = RenderMode.WorldSpace;
            Transform t = canvas.transform;
            t.SetParent(VrRig.Instance.canvasHolder.transform, false);
            t.localPosition = Vector3.zero;
        });
        
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(StartSceneUIManager), "Start")]
    private static void AddWorldspaceUIHitboxes() {
        var buttons = Object.FindObjectsOfType<Button>(true);
        
        foreach (Button button in buttons) {
            button.gameObject.AddComponent<VrUIItem>();
        }
    }
}