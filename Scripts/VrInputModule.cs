using Pineapler.Utils;
using UnityEngine;
using UnityEngine.EventSystems;
using Valve.VR;
using Input = VirtualOrc.Scripts.Input;

namespace VirtualOrc.Scripts;

public class VrInputModule : BaseInputModule {
    public bool isActive = true;

    public BaseInputModule oldInputModule;
    
    public VrLaser activeLaser { get; private set; }
    public Camera uiCamera;

    public SteamVR_Input_Sources targetSource;

    private bool _wishClickDown = false;
    private bool _wishClickUp = false;

    public GameObject currentObject;
    public PointerEventData eventData { get; private set; }


    protected override void Awake() {
        if (Plugin.VrInputModule != null) {
            Destroy(this);
            return;
        }
        Plugin.VrInputModule = this;
        base.Awake();

        uiCamera = new GameObject("LaserCamera").AddComponent<Camera>();
    }
    
    protected override void Start() {
        base.Start();
        oldInputModule = FindObjectOfType<StandaloneInputModule>();
        oldInputModule.enabled = !Config.EnableLaserInput;
        
        Log.Info("Initializing VR Input Module");
        eventData = new PointerEventData(eventSystem);

        uiCamera.transform.SetParent(transform, false);
        
        uiCamera.clearFlags = CameraClearFlags.Nothing;
        uiCamera.cullingMask = 0; // Don't draw anything
        uiCamera.orthographic = true;
        uiCamera.orthographicSize = 0.1f;
        uiCamera.aspect = 1.0f;
        uiCamera.nearClipPlane = 0.01f;
        uiCamera.farClipPlane = 100f;
        uiCamera.enabled = false;
    }

    public void OnInteractDown(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        _wishClickDown = true;
    }
    
    public void OnInteractUp(SteamVR_Action_Boolean fromAction, SteamVR_Input_Sources fromSource) {
        _wishClickUp = true;
    }


    public override void Process() {
        if (isActive == false || !Config.EnableLaserInput) return;
        
        // Reset data
        eventData.Reset();
        eventData.position = new Vector2(uiCamera.pixelWidth, uiCamera.pixelHeight) / 2;
        
        // raycast
        eventSystem.RaycastAll(eventData, m_RaycastResultCache);
        eventData.pointerCurrentRaycast = FindFirstRaycast(m_RaycastResultCache);
        currentObject = eventData.pointerCurrentRaycast.gameObject;

        // clear
        m_RaycastResultCache.Clear();

        // hover
        HandlePointerExitAndEnter(eventData, currentObject);
        
        // press
        if (_wishClickDown) {
            _wishClickDown = false;
            ProcessPress(eventData);
        }
        
        // release
        if (_wishClickUp) {
            _wishClickUp = false;
            ProcessRelease(eventData);
        }

    }
    

    private void ProcessPress(PointerEventData data) {
        Log.Info("pressed");
        data.pointerPressRaycast = data.pointerCurrentRaycast;
        GameObject newPointerPress = ExecuteEvents.ExecuteHierarchy(currentObject, data, ExecuteEvents.pointerDownHandler);

        if (newPointerPress == null) {
            newPointerPress = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);
        }

        data.pressPosition = data.position;
        data.pointerPress = newPointerPress;
        data.rawPointerPress = currentObject;
    }
    
    
    private void ProcessRelease(PointerEventData data) {
        Log.Info("released");
        ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerUpHandler);

        GameObject pointerUpHandler = ExecuteEvents.GetEventHandler<IPointerClickHandler>(currentObject);
        if (data.pointerPress == pointerUpHandler) {
            ExecuteEvents.Execute(data.pointerPress, data, ExecuteEvents.pointerClickHandler);
        }

        eventSystem.SetSelectedGameObject(null);
        data.pressPosition = Vector2.zero;
        data.pointerPress = null;
        data.rawPointerPress = null;
    }

    
    public void SetActiveLaser(VrLaser laser) {
        if (laser == activeLaser) return;
        
        Input.interact.RemoveOnStateDownListener(OnInteractDown, SteamVR_Input_Sources.LeftHand);
        Input.interact.RemoveOnStateDownListener(OnInteractDown, SteamVR_Input_Sources.RightHand);
        Input.interact.RemoveOnStateUpListener(OnInteractUp, SteamVR_Input_Sources.LeftHand);
        Input.interact.RemoveOnStateUpListener(OnInteractUp, SteamVR_Input_Sources.RightHand);

        if (activeLaser != null) {
            activeLaser.pointer.gameObject.SetActive(false);
        }
        
        if (laser == null) {
            activeLaser = null;
            isActive = false;
            return;
        }
       
        isActive = true;

        activeLaser = laser;
        Input.interact.AddOnStateDownListener(OnInteractDown, activeLaser.inputHand);
        Input.interact.AddOnStateUpListener(OnInteractUp, activeLaser.inputHand);
        
        activeLaser.pointer.gameObject.SetActive(true);
        Transform t = uiCamera.transform;
        t.SetParent(activeLaser.transform, false);
        
        targetSource = activeLaser.inputHand;
    }

}