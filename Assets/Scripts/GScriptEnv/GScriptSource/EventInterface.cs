using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/* * * * *
 * These are functions that are visible globally within the g-script engine.
 * Add functions to this file as needed...
 *
 * TODO: classes should be able to inherit from this class to add their functions
 * to the list of available functions in GScript.
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
            //Initialize();
        }
    }

    void Awake() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
            //Initialize();
        }
    }

    public static void DEBUG_PRINT(string message) {
        Debug.Log(message);
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