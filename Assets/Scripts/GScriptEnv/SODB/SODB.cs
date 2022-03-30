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
    public static SOLib<SOItem> LIB_ITEM_CONTAINER;
    public static SOLib<SOItem> LIB_ITEM_EQUIPMENT;
    public static SOLib<SOItem> LIB_ITEM_INVENTORY;
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

    // Load the ObjectMetadata.json file and use it to generate a library for all
    // desired ScriptableObject data.
    public static void InitLibs() {
        LoadObjDataJSON();

        // * * * * * Item Container * * * * *
        LIB_ITEM_CONTAINER = new SOLib<SOItem>();
        LIB_ITEM_CONTAINER.LoadLib(objMetadata["ItemContainer"]["default_so_dir"].ToString());
        Debug.Log($"Generated {LIB_ITEM_CONTAINER.lib.Count} ItemContainer Objects.");
        // * * * * * Item Equipment * * * * *
        LIB_ITEM_EQUIPMENT = new SOLib<SOItem>();
        LIB_ITEM_EQUIPMENT.LoadLib(objMetadata["ItemEquipment"]["default_so_dir"].ToString());
        Debug.Log($"Generated {LIB_ITEM_EQUIPMENT.lib.Count} EquipmentPrefab Objects.");
        // * * * * * Item Inventory * * * * *
        LIB_ITEM_INVENTORY = new SOLib<SOItem>();
        LIB_ITEM_INVENTORY.LoadLib(objMetadata["ItemInventory"]["default_so_dir"].ToString());
        Debug.Log($"Generated {LIB_ITEM_INVENTORY.lib.Count} ItemInventory Objects.");
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