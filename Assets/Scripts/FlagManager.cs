using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlagManager : MonoBehaviour
{
    public static FlagManager inst;

    public static SOLib<SOFlag> flags;

    void Start() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
            // Initialize?
        }
    }

    public static void SetFlag(string id, int val) {
        flags.lib[id].iVal = val;
        flags.lib[id].sVal = SOFlag.S_DEFAULT;
        flags.lib[id].bVal = SOFlag.B_DEFAULT;
        flags.lib[id].fVal = SOFlag.F_DEFAULT;
    }

    public static void SetFlag(string id, string val) {
        flags.lib[id].iVal = SOFlag.I_DEFAULT;
        flags.lib[id].sVal = val;
        flags.lib[id].bVal = SOFlag.B_DEFAULT;
        flags.lib[id].fVal = SOFlag.F_DEFAULT;
    }

    public static void SetFlag(string id, bool val) {
        flags.lib[id].iVal = SOFlag.I_DEFAULT;
        flags.lib[id].sVal = SOFlag.S_DEFAULT;
        flags.lib[id].bVal = val;
        flags.lib[id].fVal = SOFlag.F_DEFAULT;
    }

    public static void SetFlag(string id, float val) {
        flags.lib[id].iVal = SOFlag.I_DEFAULT;
        flags.lib[id].sVal = SOFlag.S_DEFAULT;
        flags.lib[id].bVal = SOFlag.B_DEFAULT;
        flags.lib[id].fVal = val;
    }
}
