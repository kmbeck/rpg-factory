using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Data for Item_Equipment.
 * * * * */

[CreateAssetMenu(fileName="NewEquipmentItem",menuName="Scriptable Objects/Item_Equipment")]
public class SOItemEquipment : SOItemInventory
{
    [Tooltip("What type of equipment is this?")]
    public EquipType equipType;
    [Tooltip("What EquipSlot types can this be equiped to?")]
    public List<EquipSlot> validSlots;
    // List of changes to player stats when equipped...
}
