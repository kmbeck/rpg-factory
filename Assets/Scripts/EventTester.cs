using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventTester : MonoBehaviour
{
    [Tooltip("The event we want to execute.")]
    public SOEvent tgtEvent;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void onRunClicked() {
        string methodName = "EVENT_" + tgtEvent.uniqueID.ToUpper();
        var m = typeof(GScriptEventLibrary).GetMethod(methodName);
        m.Invoke(GScriptEventLibrary.inst, null);
    }
}
