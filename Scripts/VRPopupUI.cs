using Pineapler.Utils;
using UnityEngine;

namespace VirtualOrc.Scripts;

public class VRPopupUI : MonoBehaviour {

    private static VRPopupUI Instance;
    
    private void Awake() {
        if (Instance != null) {
            Log.Warning("VRPopupUI: Too many instances!");
            Destroy(this);
            return;
        }

        Instance = this;
        
    }
}