using HarmonyLib;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class GameSettingsPatch {
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(GameSettingManager), "Start")]
    private static void TweakUrpSettings(GameSettingManager __instance) {
        UniversalRenderPipelineAsset urpAsset = __instance.renderPipelineAssets;
        
        urpAsset.shadowCascadeCount = 4;
        // why not public :( cmon unity
        TypeInfos.URP_Asset_M_Cascade4Split.SetValue(urpAsset, new Vector3(0.01f, 0.02f, 0.04f));
    }
}