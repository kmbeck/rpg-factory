using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

/* * * * *
 * Generates the GScriptFlagLibrary.cs file. This is used to easily reference
 * and update flag values in the GScript environment.
 * * * * */

public class GScriptFlagLibGenerator
{

    public GScriptFlagLibGenerator() {

    }

    // Generate GScriptFlagLibrary.cs file.
    public void genFlagLibFile() {
        string defCode = genFlagDefCode();
        string outputFileName = "GScriptFlagLibrary.cs";
        string fileCode = $@"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 *          --- DO NOT EDIT ---
 *  THIS CLASS CONTAINS GENERATED CODE!!!
 *          --- DO NOT EDIT ---
 * * * * */

public static class GScriptFlagLibrary
{{
{defCode} 
}}
"; 
        StreamWriter sw = new StreamWriter($"Assets/Scripts/GScriptEnv/GScriptSource/GeneratedCode/{outputFileName}");
        sw.WriteLine(fileCode);
        sw.Close();
    }

    // Generate code to define all global flag variables.
    private string genFlagDefCode() {
        GScriptContextualizer cxtr = new GScriptContextualizer();
        ScopeVar[] flagVars = cxtr.getContextualizedFlags();
        string retval = "";
        // Generate fake VAR_DEF Statements for each flag.
        foreach (ScopeVar v in flagVars) {
            switch(v.type) {
                case VType.INT:
                    retval += $"\tpublic static int {v.name} = {SODB.LIB_FLAG.lib[v.name].iVal};\n";
                    break;
                case VType.STRING: 
                    retval += $"\tpublic static string {v.name} = \"{SODB.LIB_FLAG.lib[v.name].sVal}\";\n";
                    break;
                case VType.FLOAT:
                    retval += $"\tpublic static float {v.name} = {SODB.LIB_FLAG.lib[v.name].fVal}f;\n";
                    break;
                case VType.BOOL:
                    retval += $"\tpublic static bool {v.name} = {SODB.LIB_FLAG.lib[v.name].bVal.ToString().ToLower()};\n";
                    break;
                case VType.LIST:
                    break;
                    //TODO: Case for lists?
                    //      - better tabs?          
            }
        }
        return retval;
    }
}