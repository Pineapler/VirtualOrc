using System.Reflection;
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
        typeof(UniversalRenderPipelineAsset).GetField("m_Cascade4Split", BindingFlags.Instance | BindingFlags.NonPublic)!
            .SetValue(urpAsset, new Vector3(0.01f, 0.02f, 0.04f));
    }
}