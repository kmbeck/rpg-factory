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

    // Generate GScriptFlagLibrar.cs file.
    public void genFlagLibFile() {
        string defCode = genFlagDefCode();
        string initCode = genFlagInitCode();
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

public abstract class GScriptFlagLibrary : MonoBehaviour
{{
    public static GScriptFlagLibrary inst;
{defCode} 
    void Start() {{
        if (inst != null) {{
            Destroy(this);
        }}
        else {{
            inst = this;
            Initialize();
        }}
    }}

    void Awake() {{
        if (inst != null) {{
            Destroy(this);
        }}
        else {{
            inst = this;
            Initialize();
        }}
    }}

    private void Initialize() {{
{initCode}
    }}
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
                    retval += $"\tpublic int {v.name};\n";
                    break;
                case VType.STRING: 
                    retval += $"\tpublic string {v.name};\n";
                    break;
                case VType.FLOAT:
                    retval += $"\tpublic float {v.name};\n";
                    break;
                case VType.BOOL:
                    retval += $"\tpublic bool {v.name};\n";
                    break;
                case VType.LIST:
                    break;
                    //TODO: Case for lists?
                    //      - better tabs?          
            }
        }
        return retval;
    }

    // Generate a function that initializes all flags to their default values.
    private string genFlagInitCode() {
        GScriptContextualizer c = new GScriptContextualizer();
        ScopeVar[] flagVars = c.getContextualizedFlags();
        List<Statement> initStatements = new List<Statement>();
        
        // Generate init strings.
        string retval = "";
        foreach (ScopeVar v in flagVars) {
            switch(v.type) {
                case VType.INT:
                    retval += $"\t\t{v.name} = {SODB.LIB_FLAG.lib[v.name].iVal};\n";
                    break;
                case VType.STRING: 
                    retval += $"\t\t{v.name} = \"{SODB.LIB_FLAG.lib[v.name].sVal}\";\n";
                    break;
                case VType.FLOAT:
                    retval += $"\t\t{v.name} = {SODB.LIB_FLAG.lib[v.name].fVal}f;\n";
                    break;
                case VType.BOOL:
                    retval += $"\t\t{v.name} = {SODB.LIB_FLAG.lib[v.name].bVal.ToString().ToLower()};\n";
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
