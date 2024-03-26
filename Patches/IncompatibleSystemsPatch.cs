using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Kuro;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.Playables;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public class IncompatibleSystemsPatch {

    #region ZibraLiquid
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "Awake")]
    private static bool DestroyZibraLiquids(ZibraLiquidTool __instance) {
        Log.Warning("Destroying ZibraLiquid objects as they crash the game in VR.");
        
        foreach (Transform child in __instance.transform) {
            Object.Destroy(child.gameObject);
        }

        ZibraLiquidTool.Instance = __instance;
        return false;
    }
    
    // Bypass all instance methods
    // ===========================
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "SetMassageShopWater")]
    private static bool BypassSetMassageShopWater() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "SetOrcSperm")]
    private static bool BypassSetOrcSperm() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "SetupOrcLiquid")]
    private static bool BypassSetupOrcLiquid() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "AddCollidersToOrcLiquid")]
    private static bool BypassAddCollidersToOrcLiquid() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "RemoveFromOrcLiquid")]
    private static bool BypassRemoveFromOrcLiquid() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "AddColliders")]
    private static bool BypassAddColliders() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "RemoveColliders")]
    private static bool BypassRemoveColliders() {
        return false;
    }
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(ZibraLiquidTool), "StopOrcLiquid")]
    private static bool BypassStopOrcLiquid() {
        return false;
    }
    #endregion


    [HarmonyPrefix]
    [HarmonyPatch(typeof(WaterFiller), "FillingWater")]
    private static bool WaterFiller_BypassZibraLiquid(WaterFiller __instance, ref IEnumerator __result) {
        __result = WaterFiller_BypassZibraLiquidCoroutine(__instance);
        return false;
    }

    private static IEnumerator WaterFiller_BypassZibraLiquidCoroutine(WaterFiller __instance) {
        PlayerUI.Instance.gameObject.SetActive(false);
        __instance.yes_no_UI.SetActive(false);
        yield return new WaitForSeconds(FadeManager.Fading(1f, 0.5f));
        // ZibraLiquidTool.Instance.zibraLiquid.gameObject.SetActive(true);
        CharacterStateManager currentCharacter = CharacterManager.Instance.GirlCharacterStateManager;
        currentCharacter.transform.position = Vector3.zero;
        currentCharacter.transform.eulerAngles = Vector3.zero;
        currentCharacter.ResetCharacterAnimatorTransform();
        currentCharacter.characterTalkEvent.GetComponent<WorldSpaceButton>().SetOutline(false);
        currentCharacter.characterTalkEvent.GetComponent<WorldSpaceButton>().SetActive(false);
        PlayableDirector timeline = TimelineManager.Instance.GetTimeline("Timeline_OrcHoldPipe");
        timeline.gameObject.gameObject.SetActive(true);
        currentCharacter.SwitchToRascalForm();
        yield return new WaitForSeconds(0.5f);
        yield return new WaitForSeconds(FadeManager.Fading(0.0f, 0.5f));
        yield return new WaitForSeconds((float) timeline.duration - 1.5f);
        yield return new WaitForSeconds(FadeManager.Fading(1f, 0.5f));
        __instance.SetWindow(false);
        __instance.pipe.SetActive(false);
        timeline.gameObject.gameObject.SetActive(false);
        SceneObjectHolder.Instance.objects.First(n => n.name == "WaterPipe").SetActive(false);
        yield return null;
        __instance.isWaterFilled = true;
        __instance.needToFill = false;
        DialogueManager.Instance.dialogueSelection.Setup();
        CharacterManager.Instance.SetPlayerAnim("Idle");
        CharacterManager.Instance.SetPlayerTransform(RefrencePoint.StartPoint);
        CameraManager.Instance.SetCameraMove(CameraMove.Normal, 2f, true);
        currentCharacter.gameObject.SetActive(true);
        OrcStateManager.Instance.gameObject.SetActive(true);
        currentCharacter.ResetCharacterAnimatorTransform();
        currentCharacter.SwitchToRascalForm();
        currentCharacter.characterFormReferences.GetCurrentFormCharacterData().anim.CrossFade("Idle" + currentCharacter.selectedIdleStandPosNum.ToString(), 0.0f);
        CharacterManager.Instance.SetGirlTransform(currentCharacter.GetCurrentCharacterTransformData().idlePoints[currentCharacter.selectedIdleStandPosNum - 1]);
        // ZibraLiquidTool.Instance.SetMassageShopWater(true);
        yield return new WaitForSeconds(1.5f);
        yield return new WaitForSeconds(FadeManager.Fading(0.0f, 1f));
        InteractionObjectManager.Instance.GetInteractionObject(InteractionObjectManager.WorldSpaceButtons.VipMassageTable).SkipMouseDownEvent = false;
        InteractionObjectManager.Instance.GetInteractionObject(InteractionObjectManager.WorldSpaceButtons.VipMassageTable).AddOnEvent = (WorldSpaceButton.SpecialEvent) (() => { });
        InteractionObjectManager.Instance.ActiveInteractionObject(InteractionObjectManager.WorldSpaceButtons.VipMassageTable);
        InteractionObjectManager.Instance.UnActiveInteractionObject(InteractionObjectManager.WorldSpaceButtons.WindowView);
        currentCharacter.characterTalkEvent.GetComponent<WorldSpaceButton>().SetActive(true);
        PlayerUI.Instance.gameObject.SetActive(true);
        __instance.isFloorWet = true;
    }
}