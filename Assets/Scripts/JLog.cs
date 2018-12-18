using UnityEngine;
using System.Collections;

public class JLog : Singleton<JLog> {

    public enum LogLevel { None, Simple, Detail, Fully }

    public LogLevel logLevel = LogLevel.Simple;

    private void Log(LogLevel lv, string message) {
        if (this.logLevel >= lv) {
            Debug.Log(message);
        }
    }

    private void Log(LogLevel lv, string format, params object[] args) {
        if (this.logLevel >= lv) {
            Debug.LogFormat(format, args);
        }
    }

    private void Log(LogLevel lv, Object context, string format, params object[] args) {
        if (this.logLevel >= lv) {
            Debug.LogFormat(context, format, args);
        }
    }


    public void Simple(string message) {
        this.Log(LogLevel.Simple, message);
    }

    public void Simple(string format, params object[] args) {
        this.Log(LogLevel.Simple, format, args);
    }

    public void Simple(Object context, string format, params object[] args) {
        this.Log(LogLevel.Simple, context, format, args);
    }


    public void Detail(string message) {
        this.Log(LogLevel.Detail, message);
    }

    public void Detail(string format, params object[] args) {
        this.Log(LogLevel.Detail, format, args);
    }

    public void Detail(Object context, string format, params object[] args) {
        this.Log(LogLevel.Detail, context, format, args);
    }
}


