using BepInEx.Logging;

namespace Pineapler.Utils;

public static class Log {
    private static ManualLogSource _logSource;

    public static void SetSource(ManualLogSource logger) {
        _logSource = logger;
    }

    public static void Info(object message) {
        _logSource.LogInfo(message);
    }

    public static void Warning(object message) {
        _logSource.LogWarning(message);
    }

    public static void Error(object message) {
        _logSource.LogError(message);
    }


}