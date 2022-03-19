using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [HideInInspector]
    public EquipmentManager em;
    [HideInInspector]
    public InventoryManager im;

    // Start is called before the first frame update
    void Start()
    {
        em = GetComponent<EquipmentManager>();
        im = GetComponent<InventoryManager>();

        em.Initialize();
        im.Initialize();

        //testInit();
    }

    // private void testInit() {
    //     ItemContainer bag = SODB.LIB_ITEM_CONTAINER.GetInstance<ItemContainer>("TestBackpack");
    //     bag.gameObject.name = "InitTestBackpack";
    //     bag.Initialize();
    //     em.equip(bag);
    //     // TODO: better way to do this than dict search every time we want to
    //     //  access or change a value? Memoization here???
    //     SODB.LIB_FLAG.GetFlagSVal("TestFlag");
    //     SODB.LIB_FLAG.GetFlagBVal("TestFlag");
    //     SODB.LIB_FLAG.GetFlagFVal("TestFlag");
    //     SODB.LIB_FLAG.SetFlag("TestFlag", 98765);
    //     Debug.Log($"new flag value = {SODB.LIB_FLAG.GetFlagIVal("TestFlag")}");
    // }
}