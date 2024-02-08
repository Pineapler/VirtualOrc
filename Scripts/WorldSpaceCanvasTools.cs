using UnityEngine;

namespace VirtualOrc.Scripts;

public class WorldSpaceCanvasTools : MonoBehaviour {

    public bool billboardEnabled = true;
    public bool perspectiveScaleEnabled = true;

    public Transform canvasTransform;
    
    // public Quaternion billboardOffsetRotation;
    
    private void LateUpdate() {
        if (Plugin.VrRig == null) return;
        Billboard();
        // PerspectiveScale();
    }


    private void Billboard() {
        if (!billboardEnabled || canvasTransform == null) return;
        
        canvasTransform.rotation = Quaternion.LookRotation(Plugin.VrRig.headsetObj.transform.position - canvasTransform.position, Vector3.up);
    }

    
    // private void PerspectiveScale() {
    //     if (!perspectiveScaleEnabled) return;
    //     
    // }
}