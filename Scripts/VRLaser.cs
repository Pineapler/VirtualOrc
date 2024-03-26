using System.Collections;
using Pineapler.Utils;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

namespace VirtualOrc.Scripts;

public class VRLaser : MonoBehaviour {

    private static int _userCount;
    private static int _hiderCount;

    public static void AddUser() {
        _userCount += 1;
    }
    
    public static void RemoveUser() {
        _userCount -= 1;
    }

    public static void AddHider() {
        _hiderCount += 1;
    }
    
    public static void RemoveHider() {
        _hiderCount -= 1;
    }

    public Color laserColor = new (0.5f, 0.8f, 0.6f);
    public Color laserColorClick = new (0.7f, 0.9f, 0.8f);
    
    public SteamVR_Action_Boolean uiSelectAction;
    public SteamVR_Input_Sources inputHand;
    public SteamVR_LaserPointer pointer;


    private void Awake() {
        pointer = gameObject.AddComponent<SteamVR_LaserPointer>();
    }

    private IEnumerator Start() {
        yield return 0; // Wait for pointer to set itself up. Could implement custom logic instead.
        uiSelectAction = SteamVR_Actions._default.Interact;

        pointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        pointer.interactWithUI = uiSelectAction;
        pointer.color = laserColor;
        pointer.clickColor = laserColorClick;

        // Change the InputModule to the hand that pressed trigger last
        uiSelectAction.RemoveOnStateDownListener(OnPointerClickNotEnabled, SteamVR_Input_Sources.LeftHand);
        uiSelectAction.AddOnStateDownListener(OnPointerClickNotEnabled, inputHand);

        pointer.pointer.gameObject.SetActive(false);
        pointer.enabled = false;

        Material material = new Material(Shader.Find("Universal Render Pipeline/NiloToon/NiloToon_Environment"));
        pointer.pointer.GetComponent<MeshRenderer>().material = material;
        pointer.pointer.layer = LayerMask.NameToLayer("UI");
    }

    private void OnPointerClickNotEnabled(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (fromSource != inputHand || !VRInputModule.Instance.isLaserActive) return;
        if (VRInputModule.Instance.activeLaser == this) return;
        
        VRInputModule.Instance.SetActiveLaser(this);
    }

    private void Update() {
        if (VRInputModule.Instance == null) return;

        if (pointer.enabled) {
            if (_hiderCount > 0 || _userCount <= 0 || VRInputModule.Instance.activeLaser != this) {
                pointer.pointer.gameObject.SetActive(false);
                pointer.enabled = false;
            }
        }
        else {
            if (_hiderCount <= 0 && _userCount > 0 && VRInputModule.Instance.activeLaser == this) {
                pointer.pointer.gameObject.SetActive(true);
                pointer.enabled = true;
            }
        }
    }
}