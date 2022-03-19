using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * These are functions that are visible globally within the g-script engine.
 * Add functions to this file as needed...
 * * * * */
public abstract class EventInterface : MonoBehaviour
{
    public static void PRINT_MSG_TO_CONSOLE(string message) {
        Debug.Log(message);
    }

    public static void SET_FLAG(string flagUniqueID, int newVal) {
        SODB.LIB_FLAG.SetFlag(flagUniqueID, newVal);
    }

    public static void SET_FLAG(string flagUniqueID, string newVal) {
        SODB.LIB_FLAG.SetFlag(flagUniqueID, newVal);
    }

    public static void SET_FLAG(string flagUniqueID, bool newVal) {
        SODB.LIB_FLAG.SetFlag(flagUniqueID, newVal);
    }

    public static void SET_FLAG(string flagUniqueID, float newVal) {
        SODB.LIB_FLAG.SetFlag(flagUniqueID, newVal);
    }
}