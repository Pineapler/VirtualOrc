using System.Reflection;
using UnityEngine.Rendering.Universal;

namespace VirtualOrc.Patches;

// Keep all this ugly reflection shit hidden
public static class TypeInfos {
    public static MethodInfo FingerDetectGame_Event1_CheckForTwoSeconds =
        typeof(FingerDetectGame).GetMethod("Event1_CheckForTwoSeconds", BindingFlags.Instance | BindingFlags.NonPublic);

    public static FieldInfo FingerDetectGame_Main =
        typeof(FingerDetectGame).GetField("main", BindingFlags.NonPublic | BindingFlags.Instance);

    public static PropertyInfo MouseCursorManager_Hit =
        typeof(MouseCursorManager).GetProperty("hit", BindingFlags.NonPublic | BindingFlags.Instance);
    public static PropertyInfo MouseCursorManager_LastRaycastable =
        typeof(MouseCursorManager).GetProperty("lastRaycastable", BindingFlags.NonPublic | BindingFlags.Instance);
    
    
    public static FieldInfo OptionMenu_Canvas = 
        typeof(OptionMenu).GetField("canvas", BindingFlags.Instance | BindingFlags.NonPublic);

    public static FieldInfo URP_Asset_M_Cascade4Split =
        typeof(UniversalRenderPipelineAsset).GetField("m_Cascade4Split", BindingFlags.Instance | BindingFlags.NonPublic);

    public static FieldInfo OrcTouchingHand_Distance =
        typeof(OrcTouchingHand).GetField("distance", BindingFlags.NonPublic | BindingFlags.Instance);
    public static FieldInfo OrcTouchingHand_DistanceAudio =
        typeof(OrcTouchingHand).GetField("distanceAudio", BindingFlags.NonPublic | BindingFlags.Instance);

    public static FieldInfo GMState_VipMassageEnd_UpdateEvent =
        typeof(GMState_VipMassageEnd).GetField("updateEvent", BindingFlags.NonPublic | BindingFlags.Instance);
}