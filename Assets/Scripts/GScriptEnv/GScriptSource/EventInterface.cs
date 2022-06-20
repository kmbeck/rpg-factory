using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/* * * * *
 * These are functions that are visible globally within the g-script engine.
 * Add functions to this file as needed...
 *
 * TODO: better way to register functions with EventInterface...
 * * * * */
public class EventInterface : MonoBehaviour
{
    public static EventInterface inst;

    void Start() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
        }
    }

    void Awake() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
        }
    }

    [GScript]
    public static void DEBUG_PRINT(string message) {
        Debug.Log(message);
    }

    [GScript]
    public static void EXEC_EVENT(string uniqueID) {
        string methodName = "EVENT_" + uniqueID.ToUpper();
        var m = typeof(GScriptEventLibrary).GetMethod(methodName);
        m.Invoke(null, null);
    }

    // Ideas...

    public static void WRITE_TO_FILE(string fp, string text) {

    }

    public static void START_DIALOGUE() {
        // Start a dialogue sesion with THIS object.
    }

    public static void DIALOGUE_MESSAGE(string message, bool replace=true) {
        // Display this string  in the current dialogue window.
        //      replace tells us weather we want to
    }

    public static string DIALOGUE_OPTION(string message, List<string> options) {
        // Display this option.

        // return selection by player...?
        return "";
    }
}