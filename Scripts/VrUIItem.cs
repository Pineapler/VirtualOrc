using UnityEngine;

namespace VirtualOrc.Scripts;

public class VrUIItem : MonoBehaviour{
    private void Start() {
        ValidateCollider();
    }
    
    private void OnEnable() {
        ValidateCollider();
    }

    public void ValidateCollider() {
        RectTransform rect = gameObject.GetComponent<RectTransform>();
        
        BoxCollider box;
        if (!TryGetComponent(out box)) {
            box = gameObject.AddComponent<BoxCollider>();
        }

        box.size = rect.sizeDelta;
    }
}