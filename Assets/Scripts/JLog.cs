using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class JLog : Singleton<JLog> {

    public enum LogLevel { None, Simple, Detail, Fully }

    public static LogLevel logLevel = LogLevel.Detail;

    public enum LogCategory { Card, Action, Replay, Count }

    public List<EnableLog> enableLogs = new List<EnableLog>((int)LogCategory.Count);


    public bool IsEnableCategory(LogCategory category) {
        foreach (EnableLog l in enableLogs) {
            if (l.category.Equals(category)) {
                return l.enable;
            }
        }

        return true;
    }

    public static void Log(LogCategory category, string message) {
        if (m_instance.IsEnableCategory(category)) {
            Debug.Log("[" + category.ToString() + "] " + message);
        }
    }

    public static void Log(LogCategory category, string format, params object[] args) {
        if (m_instance.IsEnableCategory(category)) {
            Debug.LogFormat("[" + category.ToString() + "] " + format, args);
        }
    }

    public static void Log(LogCategory category, Object context, string format, params object[] args) {
        if (m_instance.IsEnableCategory(category)) {
            Debug.LogFormat(context, "[" + category.ToString() + "] " + format, args);
        }
    }

    public static void Simple(string message) {
        Debug.Log(message);
    }

    public static void Simple(string format, params object[] args) {
        Debug.LogFormat(format, args);
    }

    public static void Simple(Object context, string format, params object[] args) {
        Debug.LogFormat(context, format, args);
    }

    public static string ToString<T>(List<T> list) {
        return ToString<T>(list.ToArray());
    }

    public static string ToString<T> (T [] arrays) {
        StringBuilder sb = new StringBuilder("[");
        int len = arrays.Length;
        for (int i = 0; i < len; i++) {
            if (i > 0) {
                sb.Append(",");
            }
            sb.Append(arrays[i].ToString());
        }
        sb.Append("]");

        return sb.ToString();
    }
}

[System.Serializable]
public class EnableLog {
    public JLog.LogCategory category;
    public bool enable;
}


