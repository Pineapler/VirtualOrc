using BepInEx.Configuration;

namespace VirtualOrc;

public class Config(ConfigFile file) {
    
    // VR

    public ConfigEntry<bool> EnableHeadsetTracking { get; } = file.Bind("VR", "HeadsetTrackingEnabled", true, "Whether the headset's position and rotation should influence the camera's position.");
    public ConfigEntry<bool> EnableAutoRecenter { get; } = file.Bind("VR.AutoRecenter", "Enabled", true, "Whether the camera is automatically teleported when it is moved too far from its anchor point.");
    public ConfigEntry<float> AutoRecenterDistance { get; } = file.Bind("VR.AutoRecenter", "Distance", 0.5f, "When VR.AutoRecenter is enabled, how far is the headset allowed to move from its anchor point?");
    
    // UI

    public ConfigEntry<float> CanvasDistance { get; } = file.Bind("UI", "Distance", 1f, "How far should the UI be from the camera?");
    public ConfigEntry<float> CanvasScaleFactorWish { get; } = file.Bind("UI", "Scale", 1f, "How large should the UI appear?");

    
    // Some internal values are weird and need to be translated from a user-friendly config value.
    public float CanvasScaleFactor => CanvasScaleFactorWish.Value * 0.00075f;
}