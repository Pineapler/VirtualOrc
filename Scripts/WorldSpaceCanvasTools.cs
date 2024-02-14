using UnityEngine;
using UnityEngine.Serialization;

namespace VirtualOrc.Scripts;

public class WorldSpaceCanvasTools : MonoBehaviour {

    public bool enableBillboard;
    public bool enablePerspectiveScale;
    public Vector3 perceivedScale = Vector3.one; // Scale at 1m from the camera

    public BoxCollider boxCollider;
    private bool _colliderEnabled;
    private bool _isColliderAdded;

    
    public bool colliderEnabled {
        get => _colliderEnabled;
        set {
            _colliderEnabled = true;
            if (value == false) {
                if (boxCollider == null) return;
                boxCollider.enabled = false;
                return;
            }

            if (boxCollider == null) {
                if (!targetRectTransform.gameObject.TryGetComponent(out boxCollider)) {
                    boxCollider = targetRectTransform.gameObject.AddComponent<BoxCollider>();
                    _isColliderAdded = true;
                }
                else {
                    _isColliderAdded = false;
                }
            }
            boxCollider.size = targetRectTransform.sizeDelta;
            boxCollider.enabled = true;
        }
    }

    private RectTransform _canvasRectTransform;
    public RectTransform targetRectTransform {
        get {
            if (_canvasRectTransform == null) {
                _canvasRectTransform = GetComponent<RectTransform>();
            }
            return _canvasRectTransform;
        }
        set {
            // if (_canvasRectTransform != null) {
            //     _canvasRectTransform.localScale = _canvasScaleOriginal;
            // }
            
            _canvasRectTransform = value;
            // _canvasScaleOriginal = _canvasRectTransform.localScale;
            // float fac = Plugin.Config.CanvasScaleFactor;
            // _canvasRectTransform.localScale = new Vector3(fac, fac, 1);
            
            if (colliderEnabled) {
                if (_isColliderAdded) {
                    Destroy(boxCollider);
                    _isColliderAdded = false;
                }
                
                if (!_canvasRectTransform.gameObject.TryGetComponent(out boxCollider)) {
                    boxCollider = _canvasRectTransform.gameObject.AddComponent<BoxCollider>();
                    _isColliderAdded = true;
                }
                else {
                    _isColliderAdded = false;
                }
            }
            
        }
    }
        
    private void LateUpdate() {
        if (VRRig.Instance == null) return;
        Billboard();
        PerspectiveScale();
    }


    private void Billboard() {
        if (!enableBillboard) return;
        
        targetRectTransform.LookAt(VRRig.Instance.headsetCam.transform, Vector3.up);
    }


    private void PerspectiveScale() {
        if (!enablePerspectiveScale || VRRig.Instance == null) return;

        // Get distance from camera
        // Compare 1u from camera

        // S_p is scale at distance from camera @ 1u (perceived size)
        // s_a is scale required to match the perceived size, when the object is n units from camera

        // d : s
        // 3        :   required_scale
        // 1        :   (2, 2, 2)

        Transform t = transform;
        float dist = (t.position - VRRig.Instance.headsetCam.transform.position).magnitude;
        t.localScale = dist * perceivedScale;
    }

}