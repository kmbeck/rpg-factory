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
        ItemContainer bag = ObjLibManager.libItemContainer.GetCopy<ItemContainer>("TestBackpack");
        bag.gameObject.name = "InitTestBackpack";
        bag.Initialize();
        em.equip(bag);
    }
}