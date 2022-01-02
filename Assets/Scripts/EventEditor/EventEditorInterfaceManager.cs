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

            // PRINT METHOD NAME
            Debug.Log($"\t{m.Name}");

            // GET ALL PARAMETER DATA & PRINT "NAME, TYPE"
            ParameterInfo[] pars = m.GetParameters();
            foreach(ParameterInfo p in pars) {
                Debug.Log($"\t{p.Name}, {p.ParameterType}");
            }
        }

        //TODO: sort paramters by Name -> Paramter names -> Paramter types...
        //TODO: expose info in some data structure that allows for autosuggest...?
        //TODO: build basic interface that allows text field editing and saving or something...
        //TODO: how to read event editor script and execute.
    }
}