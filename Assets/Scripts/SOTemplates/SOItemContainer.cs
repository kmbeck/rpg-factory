using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * A bag or pouch or something that can hold items.
 * * * * */
 [CreateAssetMenu(fileName="NewContainerItem", menuName="Scriptable Objects/Item_Container")]
public class SOItemContainer : SOItemEquipment
{
    [Tooltip("A txt file that defines the layout of the container.")]
    public TextAsset layout;
    [Tooltip("What types of stuff can you store in this container?")]
    public StorageType storageType;
    [Tooltip("Can this container be accessed during combat?")]
    public bool accessInCombat;

    // Items stored in this inventory. Used by InventoryManager in charge of
    // this container. int keys are mapped to the slots defiend in the layout
    // string.
    [HideInInspector]
    public Dictionary<int,ItemInventory> items;
}
