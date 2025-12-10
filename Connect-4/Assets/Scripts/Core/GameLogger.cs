using UnityEngine;

// GameLogger:
// - Central place for all debug logs
// - Toggle logs on/off from GameManager
// - Call GameLogger.Log / LogWarning / LogError instead of Debug.Log directly
public static class GameLogger
{
    // Global toggle
    public static bool IsEnabled { get; set; } = true;

    public static void Log(string message)
    {
        if (!IsEnabled) return;
        Debug.Log(message);
    }

    public static void LogWarning(string message)
    {
        if (!IsEnabled) return;
        Debug.LogWarning(message);
    }

    public static void LogError(string message)
    {
        if (!IsEnabled) return;
        Debug.LogError(message);
    }
}
