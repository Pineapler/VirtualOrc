using HarmonyLib;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class LogoPagePatch {
    [HarmonyPostfix]
    [HarmonyPatch(typeof(LogoPage), nameof(LogoPage.Start))]
    private static void SkipLogoPage(LogoPage __instance) {
        __instance.ToGameScene();
    }
}