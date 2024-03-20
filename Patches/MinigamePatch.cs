using System.Net;
using DG.Tweening;
using HarmonyLib;
using UnityEngine;
using Valve.VR;
using VirtualOrc.Scripts;
using Input = VirtualOrc.Scripts.Input;

namespace VirtualOrc.Patches;

[HarmonyPatch]
public static class MinigamePatch {
    
    [HarmonyPrefix]
    [HarmonyPatch(typeof(OrcTouchingHand), "CheckMouse_New")]
    private static bool OrcTouchingHand_CheckMouse_New(OrcTouchingHand __instance) {
        if (Input.interact.GetState(SteamVR_Input_Sources.Any)) {
            float endValue = Mathf.Clamp01(1 - Mathf.Clamp(Mathf.Abs((float)TypeInfos.OrcTouchingHand_Distance.GetValue(__instance)), 0,
                                               __instance.MaxRange));
            DOTween.Kill(__instance.gameObject);
            DOTween.To((() => __instance.currentSensitive), (n => __instance.currentSensitive = n), endValue, 0.2f);
        }
        else {
            DOTween.To((() => __instance.currentSensitive), (n => __instance.currentSensitive = n), 0, 0.2f);
            (TypeInfos.OrcTouchingHand_DistanceAudio.GetValue(__instance) as AudioSource)!.pitch = 1f;
            __instance.orcHandAnim.speed = 1f;
        }
        CharacterManager.Instance.GirlCharacterStateManager.anim.SetFloat("Sensitive", __instance.currentSensitive);
        return false;
        
        // if (!Plugin.Config.vrInputEnabled) {
        //     return true; // use default implementation
        // }
        //
        // // TODO: need to know which controller the hand is attached to
        // bool isGrabbing = Input.interact.state;
        //
        // return false;
    }


    [HarmonyPrefix]
    [HarmonyPatch(typeof(FingerDetectGame), "Detect_ClickOnMassagePart", [])]
    
    private static bool FingerDetectGame_ClickOnMassagePart(FingerDetectGame __instance) {
        VipMassageGameSystem main = TypeInfos.FingerDetectGame_Main.GetValue(__instance) as VipMassageGameSystem;
        bool interactPressed = Input.interact.GetState(SteamVR_Input_Sources.Any);
        
        // Log.Info("ClickOnMassagePart");
        bool handHitCharacter = HandCast(main!.orcTouchingHand);
        
        if (main.OrcAnimating)
            return false;
        if (main.Massaging != interactPressed) {
            main.Massaging = interactPressed;
            main.orcTouchingHand.orcHandAnim.SetBool("Massage", main.Massaging);
        }
        main.orcTouchingHand.orcTouchingHandUI.SetMainHeartSize(main.Massaging ? 1f : 0.65f);
        if (main.Massaging && handHitCharacter) {
            main.orcTouchingHand.ShowWave(true);
            main.orcTouchingHand.PlayAudio(true);
            main.orcTouchingHand.CheckDistance();
            if (__instance.inTutorial) {
                    TypeInfos.FingerDetectGame_Event1_CheckForTwoSeconds.Invoke(__instance, []);
            }
            else if (!__instance.lockStamina)
                __instance.SetStamina(main.currentStamina - Time.deltaTime * main.staminaDeductSpeed);
            if ((double) main.currentStamina > 0.0)
                return false;
            main.LevelDefeated();
        }
        else
        {
            main.orcTouchingHand.PlayWaveAnimation(false);
            main.orcTouchingHand.ShowWave(false);
            main.orcTouchingHand.PlayAudio(false);
        }
    
        return false;
    }

    // [HarmonyPrefix]
    // [HarmonyPatch(typeof(OrcTouchingHand), "CheckDistance")]
    private static bool HandCast(OrcTouchingHand __instance) {
        VRHand hand = VRHand.Instance;
        Transform t = hand.orcHand.transform;
        Ray ray = new Ray(t.position + hand.raycastOffset, t.forward);
        if (Physics.SphereCast(ray, hand.spherecastRadius, out RaycastHit hit, hand.raycastDistance, __instance.raycastSystem.detectLayerMask)) {
            __instance.raycastSystem.selectedHit = hit;
            
            // Debug.Log($"Hit character at {hit.point}");
            __instance.forceFieldOuterMeshRen.transform.position = hit.point;
            float dist = Mathf.Abs(Vector3.Distance(__instance.raycastSystem.selectedHit.point,
                __instance.weakPoint.transform.position));
            TypeInfos.OrcTouchingHand_Distance.SetValue(__instance, dist);
            return true;
        }
        // else {
        //     Debug.Log($"Missed ray {ray.origin} -> {ray.direction}");
        // }
        return false;
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(OrcTouchingHand), "CheckClick")]
    private static bool OrcTouchingHand_CheckClickPatch(OrcTouchingHand __instance, bool _requirement) {
        if (_requirement && Input.interact.GetState(SteamVR_Input_Sources.Any)) {
            __instance.stageEvent();
        }
        return false;
    }
}