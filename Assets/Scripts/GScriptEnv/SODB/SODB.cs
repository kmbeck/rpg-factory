using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

[ExecuteInEditMode]
public class SODB : MonoBehaviour
{
    public static SODB inst;

    // The libraries in the Data Base.
    // NOTE: It is NECISSARY to have these variable names start with 'lib'
    //      and be the ONLY variables in this class that start with 'lib'
    //      so the sketchy code reflection will work correctly!!!
    public static SOLib<SOEvent> LIB_EVENT;
    public static SOFlagLib<SOFlag> LIB_FLAG;

    private static TextAsset objMetadataJSON;
    private static JObject objMetadata;

    // Start is called before the first frame update
    void Start()
    {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
            Initialize();
        }
    }

    void Awake()
    {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
            Initialize();
        }
    }

    public void Initialize() {
        // Initialize & load all of our libraries.
        InitLibs();
    }

    // Get field in objMetadata at [typeName],[field].
    public static string GetMetadataField(string typeName, string field) {
        return objMetadata[typeName][field].ToString();
    }

    // Same as InitLib() but refreshes event lib ONLY.
    public static void InitEventLib() {      
        LoadObjDataJSON();

        // * * * * * Events * * * * *
        LIB_EVENT = new SOLib<SOEvent>();
        LIB_EVENT.LoadLib(objMetadata["SOEvent"]["default_so_dir"].ToString());
        Debug.Log($"Generated {LIB_EVENT.lib.Count} Event Objects.");
    }

    // Same as InitLib() but initializes flag lib ONLY.
    public static void InitFlagLib() {
        LoadObjDataJSON();

        // * * * * * Flags * * * * *
        LIB_FLAG = new SOFlagLib<SOFlag>();
        LIB_FLAG.LoadLib(objMetadata["SOFlag"]["default_so_dir"].ToString());
        Debug.Log($"Loaded {LIB_FLAG.lib.Count} Flag Objects.");
    }

    // Load the ObjectMetadata.json file and use it to generate a library for all
    // desired ScriptableObject data.
    public static void InitLibs() {
        LoadObjDataJSON();

        //TODO: Dynamically load differet lib types somehow?
        // * * * * * Events * * * * *
        LIB_EVENT = new SOLib<SOEvent>();
        LIB_EVENT.LoadLib(objMetadata["SOEvent"]["default_so_dir"].ToString());
        Debug.Log($"Generated {LIB_EVENT.lib.Count} Event Objects.");
        // * * * * * Flags * * * * *
        LIB_FLAG = new SOFlagLib<SOFlag>();
        LIB_FLAG.LoadLib(objMetadata["SOFlag"]["default_so_dir"].ToString());
        Debug.Log($"Loaded {LIB_FLAG.lib.Count} Flag Objects.");
    }

    // Loads the ObjectMetadata.json file and parses it into a JObject.
    private static void LoadObjDataJSON() {
        //TODO: Validate JSON!!!
        objMetadataJSON = Resources.Load("ObjectMetadata", typeof(TextAsset)) as TextAsset;
        objMetadata = JObject.Parse(objMetadataJSON.text);
    }

    // Returns the name of all defined libs in this class as a list of strings & returns.
    // Used by ASTParser to help poplate it's global variables before parsing.
    public ScopeVar[] getContextualizedScopeVars() {
        // Get a list of all public static vars in SODB.
        FieldInfo[] sodbFields = typeof(SODB).GetFields(BindingFlags.Public|BindingFlags.Static|BindingFlags.DeclaredOnly);

        // Identify vars that start with 'lib' and
        // TODO: use regex to check for libs not just 'lib'?
        //      better/more flexible solution?
        List<ScopeVar> newIdentifiers = new List<ScopeVar>();
        foreach(FieldInfo f in sodbFields) {
            if(f.Name.Substring(0,3).ToUpper() == "LIB") {
                // /newIdentifiers.Add(new Token(TType.IDENTIFIER, f.Name.ToUpper()));
                // TODO: how to load these? VType should not be NONE probably...
                newIdentifiers.Add(new ScopeVar(f.Name.ToUpper(), VType.NONE));
            }
        }
        return newIdentifiers.ToArray();
    }
}