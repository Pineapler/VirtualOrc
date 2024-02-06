using UnityEngine;

namespace VirtualOrc.Scripts;

public class WorldSpaceCanvasTools : MonoBehaviour {

    public bool billboardEnabled = true;
    public bool perspectiveScaleEnabled = true;

    public Transform canvasTransform;
    
    // public Quaternion billboardOffsetRotation;
    
    private void LateUpdate() {
        Billboard();
        // PerspectiveScale();
    }


    private void Billboard() {
        if (!billboardEnabled || canvasTransform == null) return;
        
        canvasTransform.rotation = Quaternion.LookRotation(VrRig.Instance.headsetObj.transform.position - canvasTransform.position, Vector3.up);
    }

    
    // private void PerspectiveScale() {
    //     if (!perspectiveScaleEnabled) return;
    //     
    // }
}