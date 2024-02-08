using System.Collections;
using Pineapler.Utils;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;

namespace VirtualOrc.Scripts;

public class VrLaser : MonoBehaviour {
    public Color laserColor = new (0.5f, 0.8f, 0.6f);
    
    public SteamVR_Action_Boolean uiSelectAction;
    public SteamVR_Input_Sources inputHand;
    public SteamVR_LaserPointer pointer;


    private void Awake() {
        pointer = gameObject.AddComponent<SteamVR_LaserPointer>();
    }
    
    private IEnumerator Start() {
        yield return 0;
        uiSelectAction = SteamVR_Actions._default.Interact;
        
        pointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        pointer.interactWithUI = uiSelectAction;
        pointer.color = laserColor;
       
        // Change the InputModule to the hand that pressed trigger last
        uiSelectAction.RemoveOnStateDownListener(OnPointerClickNotEnabled, SteamVR_Input_Sources.LeftHand);
        uiSelectAction.AddOnStateDownListener(OnPointerClickNotEnabled, inputHand);

        pointer.gameObject.SetActive(false);

        Material material = new Material(Shader.Find("Universal Render Pipeline/NiloToon/NiloToon_Environment"));
        pointer.pointer.GetComponent<MeshRenderer>().material = material;
        pointer.pointer.layer = LayerMask.NameToLayer("UI");
    }

    private void Update() {
        
    }

    private void OnPointerClickNotEnabled(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        if (fromSource != inputHand || !Config.EnableLaserInput) return;

        if (Plugin.VrInputModule.activeLaser != this) {
            Plugin.VrInputModule.SetActiveLaser(this);
        }
    }
}