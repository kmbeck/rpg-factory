using Newtonsoft.Json.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class SODB : MonoBehaviour
{
    public static SODB inst;

    public static SOLib<SOItem> libItemContainer;
    public static SOLib<SOItem> libItemEquipment;
    public static SOLib<SOItem> libItemInventory;
    public static SOLib<SOEvent> libEvent;
    public static SOFlagLib<SOFlag> libFlag;

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
            InitLibs();
        }
    }

    // Get field in objMetadata at [typeName],[field].
    public static string GetMetadataField(string typeName, string field) {
        return objMetadata[typeName][field].ToString();
    }

    // Same as InitLib() but refreshes event lib ONLY.
    public static void InitEventLib() {      
        LoadObjDataJSON();

        // * * * * * Events * * * * *
        libEvent = new SOLib<SOEvent>();
        libEvent.LoadLib(objMetadata["SOEvent"]["default_so_dir"].ToString());
        Debug.Log($"Generated {libEvent.lib.Count} Event Objects.");
    }

    // Load the ObjectMetadata.json file and use it to generate a library for all
    // desired ScriptableObject data.
    public static void InitLibs() {
        LoadObjDataJSON();

        // * * * * * Item Container * * * * *
        libItemContainer = new SOLib<SOItem>();
        libItemContainer.LoadLib(objMetadata["ItemContainer"]["default_so_dir"].ToString());
        Debug.Log($"Generated {libItemContainer.lib.Count} ItemContainer Objects.");
        // * * * * * Item Equipment * * * * *
        libItemEquipment = new SOLib<SOItem>();
        libItemEquipment.LoadLib(objMetadata["ItemEquipment"]["default_so_dir"].ToString());
        Debug.Log($"Generated {libItemEquipment.lib.Count} EquipmentPrefab Objects.");
        // * * * * * Item Inventory * * * * *
        libItemInventory = new SOLib<SOItem>();
        libItemInventory.LoadLib(objMetadata["ItemInventory"]["default_so_dir"].ToString());
        Debug.Log($"Generated {libItemInventory.lib.Count} ItemInventory Objects.");

        // * * * * * Events * * * * *
        libEvent = new SOLib<SOEvent>();
        libEvent.LoadLib(objMetadata["SOEvent"]["default_so_dir"].ToString());
        Debug.Log($"Generated {libEvent.lib.Count} Event Objects.");

        // * * * * * Flags * * * * *
        libFlag = new SOFlagLib<SOFlag>();
        libFlag.LoadLib(objMetadata["SOFlag"]["default_so_dir"].ToString());
        Debug.Log($"Loaded {libFlag.lib.Count} Flag Objects.");
    }

    // Loads the ObjectMetadata.json file and parses it into a JObject.
    private static void LoadObjDataJSON() {
        //TODO: Validate JSON!!!
        objMetadataJSON = Resources.Load("ObjectMetadata", typeof(TextAsset)) as TextAsset;
        objMetadata = JObject.Parse(objMetadataJSON.text);
    }
}
