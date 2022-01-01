using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class EventInterface : MonoBehaviour
{
    public static void PRINT_MSG_TO_CONSOLE(string message) {
        Debug.Log(message);
    }

    public static void SET_FLAG(string flagUniqueID, int newVal) {
        SODB.libFlag.SetFlag(flagUniqueID, newVal);
    }

    public static void SET_FLAG(string flagUniqueID, string newVal) {
        SODB.libFlag.SetFlag(flagUniqueID, newVal);
    }

    public static void SET_FLAG(string flagUniqueID, bool newVal) {
        SODB.libFlag.SetFlag(flagUniqueID, newVal);
    }

    public static void SET_FLAG(string flagUniqueID, float newVal) {
        SODB.libFlag.SetFlag(flagUniqueID, newVal);
    }
}