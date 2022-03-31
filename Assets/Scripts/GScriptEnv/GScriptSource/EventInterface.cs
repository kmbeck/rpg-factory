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

    public static void DEBUG_PRINT(int val) {
        Debug.Log(val);
    }

    public static void DEBUG_PRINT(float val) {
        Debug.Log(val);
    }

    public static void DEBUG_PRINT(bool val) {
        Debug.Log(val);
    }

    // Set and Get flag for each possible type.
    public static void SET_INT_FLAG(string flagUniqueID, int newVal) {
        SODB.LIB_FLAG.SetFlag(flagUniqueID, newVal);
    }

    public static int GET_INT_FLAG(string flagUniqueID) {
        return SODB.LIB_FLAG.GetFlagIVal(flagUniqueID);
    }

    public static void SET_STRING_FLAG(string flagUniqueID, string newVal) {
        SODB.LIB_FLAG.SetFlag(flagUniqueID, newVal);
    }

    public static string GET_STRING_FLAG(string flagUniqueID) {
        return SODB.LIB_FLAG.GetFlagSVal(flagUniqueID);
    }

    public static void SET_BOOL_FLAG(string flagUniqueID, bool newVal) {
        SODB.LIB_FLAG.SetFlag(flagUniqueID, newVal);
    }    
    
    public static bool GET_BOOL_FLAG(string flagUniqueID) {
        return SODB.LIB_FLAG.GetFlagBVal(flagUniqueID);
    }

    public static void SET_FLOAT_FLAG(string flagUniqueID, float newVal) {
        SODB.LIB_FLAG.SetFlag(flagUniqueID, newVal);
    }

    public static float GET_FLOAT_FLAG(string flagUniqueID) {
        return SODB.LIB_FLAG.GetFlagFVal(flagUniqueID);
    }

    // Ideas...
    public static void LOAD_SCENE(string name) {

    }

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