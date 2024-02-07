using System.Collections;
using Pineapler.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Valve.VR;
using Valve.VR.Extras;

namespace VirtualOrc.Scripts;

public class LaserUIInput : MonoBehaviour {
    public static LaserUIInput ActiveLaser;
    
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
        // pose = gameObject.AddComponent<SteamVR_Behaviour_Pose>();
        // pose.inputSource = inputHand;
        pointer = gameObject.GetComponent<SteamVR_LaserPointer>();
        
        pointer.color = laserColor;
        pointer.PointerIn -= HandlePointerIn;
        pointer.PointerIn += HandlePointerIn;
        pointer.PointerOut -= HandlePointerOut;
        pointer.PointerOut += HandlePointerOut;

        uiSelectAction.RemoveOnStateDownListener(OnUISelectAction, inputHand);
        uiSelectAction.AddOnStateDownListener(OnUISelectAction, inputHand);

        pointer.enabled = false;

        Material material = new Material(Shader.Find("Universal Render Pipeline/NiloToon/NiloToon_Environment"));
        pointer.pointer.GetComponent<MeshRenderer>().material = material;
        pointer.pointer.layer = LayerMask.NameToLayer("UI");
    }

    private void OnUISelectAction(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        Log.Info($"Select {fromSource}");
        if (ActiveLaser != this) {
            if (ActiveLaser != null ) ActiveLaser.pointer.enabled = false;
            ActiveLaser = this;
            pointer.enabled = true;
            return;
        }
       
        Log.Info($"Pressed: {EventSystem.current.currentSelectedGameObject}");
        if (EventSystem.current.currentSelectedGameObject != null) {
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject,
                new PointerEventData(EventSystem.current), ExecuteEvents.submitHandler);
        }
    }

    private void HandlePointerIn(object sender, PointerEventArgs e) {
        Log.Info($"Hovering: {e.target.name}");
        if (e.target.TryGetComponent(out Button button)) {
            button.Select();
            EventSystem.current.SetSelectedGameObject(button.gameObject);
            ExecuteEvents.Execute(EventSystem.current.currentSelectedGameObject,
                new PointerEventData(EventSystem.current), ExecuteEvents.pointerEnterHandler);
        }

    }

    private void HandlePointerOut(object sender, PointerEventArgs e) {
        Log.Info($"Stopped hovering: {e.target.name}");
        if (e.target.TryGetComponent(out Button button)) {
            EventSystem.current.SetSelectedGameObject(null);
        }
    }
}