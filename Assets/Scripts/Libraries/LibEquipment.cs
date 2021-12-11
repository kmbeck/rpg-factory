using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibEquipment : ObjLib<ItemEquipment>
{
    new public static LibEquipment instance;

    void Start() {
        // if (instance != null) {
        //     Destroy(this);
        // }
        // else {
        //     instance = this;
        //     Initialize("SOs/Equipment","Prefabs/EquipmentPrefab");
        // }
    }

    // public override void Initialize() {
    //     SODir = "SOs/Equipment";
    //     prefabFP = "Prefabs/EquipmentPrefab";
    //     LoadLib();
    // }
}
