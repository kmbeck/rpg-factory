using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Parent for all Items.
 * * * * */
[CreateAssetMenu(fileName="NewItem", menuName="Scriptable Objects/Item")]
public class SOItem : SOOrigin
{
    [Header("Equipment Values")]
    [Tooltip("What type of equipment is this?")]
    public EquipType equipType;
    [Tooltip("What EquipSlot types can this be equiped to?")]
    public List<EquipSlot> validSlots;
    // List of changes to player stats when equipped...

    [Header("Container Values")]
    // [Tooltip("Is this Item an ItemContainer?")]
    // public bool isContainer;
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
