using System;
using UnityEngine;

namespace VirtualOrc.Scripts;

public class VRHand : MonoBehaviour {

    public OrcTouchingHand touchingHand;
    public Transform orcHand;
    public Transform tracker;
    public Vector3 handOffsetPosition = new Vector3(0, -.02f, -0.07f);
    public Quaternion handOffsetRotation = Quaternion.Euler(30, 320, 280);

    private void Start() {
        touchingHand = GetComponent<OrcTouchingHand>();
        orcHand = touchingHand.orcHand;
        
        VRRig.OnReady(() => {
            tracker = VRRig.Instance.rightController.transform;
        });
    }
    
    private void FixedUpdate() {
        if (orcHand == null || tracker == null) return;
        orcHand.position = tracker.TransformPoint(handOffsetPosition);
        orcHand.rotation = tracker.rotation * handOffsetRotation;
    }
}