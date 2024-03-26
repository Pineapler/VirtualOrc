using Pineapler.Utils;
using UnityEngine;

namespace VirtualOrc.Scripts;

public class VRHand : MonoBehaviour {

    public static VRHand Instance;
    
    public OrcTouchingHand touchingHand;
    public Transform orcHand;
    public Transform tracker;
    public Vector3 handOffsetPosition = new (0, -0.02f, -0.07f);
    public Quaternion handOffsetRotation = Quaternion.Euler(30, 320, 280);

    public Vector3 raycastOffset = new (0, 0, -0.07f);
    public float raycastDistance = 0.05f;
    public float spherecastRadius = 0.04f;

    private void Awake() {
        if (Instance != null) {
            Destroy(this);
            return;
        }

        Instance = this;
    }
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