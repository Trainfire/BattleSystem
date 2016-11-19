using UnityEngine;

public static class LogEx
{
    public static void Log<T>(string message, params object[] args)
    {
        Debug.LogFormat("[{0}] {1}", typeof(T).Name, string.Format(message, args));
    }
}
