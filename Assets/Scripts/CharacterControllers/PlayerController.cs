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

        testInit();
    }

    private void testInit() {
        ItemContainer bag = SODB.libItemContainer.GetInstance<ItemContainer>("TestBackpack");
        bag.gameObject.name = "InitTestBackpack";
        bag.data.displayName = "hahahaha im fucked";
        bag.Initialize();
        em.equip(bag);
        // TODO: better way to do this than dict search every time we want to
        //  access or change a value? Memoization here???
        SODB.libFlag.GetFlagSVal("TestFlag");
        SODB.libFlag.GetFlagBVal("TestFlag");
        SODB.libFlag.GetFlagFVal("TestFlag");
        SODB.libFlag.SetFlag("TestFlag", 98765);
        Debug.Log($"new flag value = {SODB.libFlag.GetFlagIVal("TestFlag")}");
    }
}