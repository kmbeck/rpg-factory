using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/* * * * *
 * Generates Event Interface used by designer in the Event Editor.
 * * * * */

[ExecuteInEditMode]
public class EventEditorInterfaceManager : MonoBehaviour
{
    void Awake() {
        generateDesignerEventInterface();
    }

    // Get a list of all public functions in EventInterface and record function
    // metadata.
    public void generateDesignerEventInterface() {
        MethodInfo[] methods = typeof(EventInterface).GetMethods(BindingFlags.Public|BindingFlags.Static|BindingFlags.DeclaredOnly);
        Debug.Log($"{methods.Length} functions found in EventInterface.");
        foreach(MethodInfo m in methods) {
            //Debug.Log($"\t{m.Name}");
            EventInterface.PRINT_MSG_TO_CONSOLE($"Get Fucked {methods.Length} TIMES!!!");
        }
    }
}