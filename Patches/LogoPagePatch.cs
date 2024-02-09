using HarmonyLib;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class LogoPagePatch {
    // TODO: Move this to its own mod
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogoPage), nameof(LogoPage.Start))]
    private static void SkipLogoPage(LogoPage __instance) {
        __instance.ToGameScene();
    }
}