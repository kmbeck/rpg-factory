using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Child of SOLib that defines additional functions for setting flags.
 * NOTE: GetInstance() should never be invoked on SOFlagLib. 
 * * * * */

// TODO: is it worth adding another level of inheritance for Libs that can 
// initalize objects and libs that cannot to ensure no one tries to instantiate 
// something that cannot be instantiated?
// TODO: what if id not in dict???
//      - let game crash?

public class SOFlagLib<D> : SOLib<D>
where D : SOFlag
{
    //TODO: - warnings SetFlag type != dataType
    //      - warnings GetFlagXVAL != dataType OR == X_DEFAULT
    public void SetFlag(string id, int val) {
        // Cannot set values that do not correspond with Flag dataType.
        if (lib[id].dataType != FlagDataType.INT) {
            Debug.Log($"Error: Setting int of Flag {id}, but FlagDataType = {lib[id].dataType.ToString()}.");
            return;
        }
        lib[id].iVal = val;
        lib[id].sVal = SOFlag.S_DEFAULT;
        lib[id].bVal = SOFlag.B_DEFAULT;
        lib[id].fVal = SOFlag.F_DEFAULT;
    }

    public void SetFlag(string id, string val) {
        // Cannot set values that do not correspond with Flag dataType.
        if (lib[id].dataType != FlagDataType.STRING) {
            Debug.Log($"Error: Setting string of Flag {id}, but FlagDataType = {lib[id].dataType.ToString()}.");
            return;
        }
        lib[id].iVal = SOFlag.I_DEFAULT;
        lib[id].sVal = val;
        lib[id].bVal = SOFlag.B_DEFAULT;
        lib[id].fVal = SOFlag.F_DEFAULT;
    }

    public void SetFlag(string id, bool val) {
        // Cannot set values that do not correspond with Flag dataType.
        if (lib[id].dataType != FlagDataType.BOOL) {
            Debug.Log($"Error: Setting bool of Flag {id}, but FlagDataType = {lib[id].dataType.ToString()}.");
            return;
        }
        lib[id].iVal = SOFlag.I_DEFAULT;
        lib[id].sVal = SOFlag.S_DEFAULT;
        lib[id].bVal = val;
        lib[id].fVal = SOFlag.F_DEFAULT;
    }

    public void SetFlag(string id, float val) {
        // Cannot set values that do not correspond with Flag dataType.
        if (lib[id].dataType != FlagDataType.FLOAT) {
            Debug.Log($"Error: Setting float of Flag {id}, but FlagDataType = {lib[id].dataType.ToString()}.");
            return;
        }
        lib[id].iVal = SOFlag.I_DEFAULT;
        lib[id].sVal = SOFlag.S_DEFAULT;
        lib[id].bVal = SOFlag.B_DEFAULT;
        lib[id].fVal = val;
    }

    public FlagDataType GetFlagDataType(string id) {
        return lib[id].dataType;
    }

    // WARNING: these functions are referenced in code reflection!
    // Their names should not be changed!!! (see GScriptTranslator.cs)
    public int GetFlagINTVal(string id) {
        int val = lib[id].iVal;
        if (lib[id].dataType != FlagDataType.INT) {
            Debug.Log($"Warning: Get on int value from Flag {id}, but FlagDataType = {lib[id].dataType.ToString()}.");
        }
        else if (val == SOFlag.I_DEFAULT) {
            Debug.Log($"Warning: Get made on Flag {id} returning default value.");
        }

        return val;
    }

    public string GetFlagSTRINGVal(string id) {
        string val = lib[id].sVal;
        if (lib[id].dataType != FlagDataType.STRING) {
            Debug.Log($"Warning: Get on string value from Flag {id}, but FlagDataType = {lib[id].dataType.ToString()}.");
        }
        else if (val == SOFlag.S_DEFAULT) {
            Debug.Log($"Warning: Get made on Flag {id} returning default value.");
        }
        return val;
    }

    public bool GetFlagBOOLVal(string id) {
        bool val = lib[id].bVal;
        // Probably don't need warning for false bool statements...
        if (lib[id].dataType != FlagDataType.BOOL) {
            Debug.Log($"Warning: Get on bool value from Flag {id}, but FlagDataType = {lib[id].dataType.ToString()}");
        }
        return val;
    }

    public float GetFlagFLOATVal(string id) {
        float val = lib[id].fVal;
        if (lib[id].dataType != FlagDataType.FLOAT) {
            Debug.Log($"Warning: Get on float value from Flag {id}, but FlagDataType = {lib[id].dataType.ToString()}");
        }
        else if (val == SOFlag.F_DEFAULT) {            
            Debug.Log($"Warning: Get made on Flag {id} returning default value.");
        }
        return val;
    }
}
