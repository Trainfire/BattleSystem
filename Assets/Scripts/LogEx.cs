using UnityEngine;

public static class LogEx
{
    public static void Log<T>(string message)
    {
        Debug.LogFormat("[{0}] {1}", typeof(T).Name, message);
    }

    public static void Log<T>(string message, params object[] args)
    {
        Debug.LogFormat("[{0}] {1}", typeof(T).Name, string.Format(message, args));
    }

    public static void LogError<T>(string message)
    {
        Debug.LogErrorFormat("[{0}] {1}", typeof(T).Name, message);
    }

    public static void LogError<T>(string message, params object[] args)
    {
        Debug.LogErrorFormat("[{0}] {1}", typeof(T).Name, string.Format(message, args));
    }
}
