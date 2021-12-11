using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LibItems : ObjLib<ItemInventory>
{  
    new public static LibItems instance;

    // Start is called before the first frame update
    void Start()
    {
        // if (instance != null) {
        //     Destroy(this);
        // }
        // else {
        //     instance = this;
        //     Initialize("SOs/Items","Prefabs/ItemPrefab");
        // }
    }

    // public override void Initialize() {
    //     SODir = "SOs/Items";
    //     prefabFP = "Prefabs/ItemPrefab";
    //     LoadLib();
    // }
}
