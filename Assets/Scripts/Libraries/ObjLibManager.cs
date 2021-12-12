using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjLibManager : MonoBehaviour
{
    public static ObjLibManager inst;

    [HideInInspector]
    public static ObjLib<ItemEquipment> libItemEquipment;
    [HideInInspector]
    public static ObjLib<ItemInventory> libItemInventory;
    [HideInInspector]
    public static ObjLib<ItemContainer> libItemContainer;

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

    void InitLibs() {
        libItemInventory = new ObjLib<ItemInventory>();
        libItemInventory.Initialize("Prefabs/ItemPrefab", "SOs/Items");
        Debug.Log($"Generated {libItemInventory.lib.Count} ItemInventory Objects.");

        libItemEquipment = new ObjLib<ItemEquipment>();
        libItemEquipment.Initialize("Prefabs/EquipmentPrefab", "SOs/Equipment");
        Debug.Log($"Generated {libItemEquipment.lib.Count} EquipmentPrefab Objects.");

        libItemContainer = new ObjLib<ItemContainer>();
        libItemContainer.Initialize("Prefabs/ContainerPrefab", "SOs/Equipment/Containers");
        Debug.Log($"Generated {libItemContainer.lib.Count} ItemContainer Objects.");
    }
}