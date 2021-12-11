using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Manages character's equipment. Use to equiup/uniequip.
 * * * * */
public class EquipmentManager : MonoBehaviour
{
    // Define slots available to equip items.
    public SOEquipLayout layout;
   
    // List of currently equipped items. 
    private List<ItemEquipment> equippedItems;

    // Event when character equips a new container item.
    public delegate void ContainerEquipped();
    public event ContainerEquipped OnContainerEquipped;

    public void Initialize() {
        // Should be loaded from data eventually...
        equippedItems = new List<ItemEquipment>();
    }

    // Equip the item if there is an open valid slot.
    public void equip(ItemEquipment item) {
        Debug.Log(item.data.uniqueID);
        foreach (EquipSlot e in item.data.validSlots) {
            if (!hasItemInSlot(e)) {
                equippedItems.Add(item);
                item.curSlot = e;
                // Probably invoke something on item here...
                if (item.data.equipType == EquipType.CONTAINER) {
                    if (OnContainerEquipped != null) {
                        OnContainerEquipped();
                    }
                }
                Debug.Log($"Equipped: {item.data.displayName} to slot {e.ToString()}.");
                return;
            }
        }
    }

    // For equipping item containers.
    // public void equip(ItemContainer item) {
    //     foreach (EquipSlot e in item.data.validSlots) {
    //         if (!hasItemInSlot(e)) {
    //             equippedItems.Add(item);
    //         }
    //     }
    // }

    // Unequip the item.
    public void unequip(ItemEquipment item) {
        equippedItems.Remove(item);
        Debug.Log($"Unequipped: {item.data.displayName} from slot {item.curSlot.ToString()}.");
    }

    // Is there an item in the slot? Returns bool.
    public bool hasItemInSlot(EquipSlot slot) {
        foreach (ItemEquipment e in equippedItems) {
            if (e.curSlot == slot) { return true; }
        }
        return false;
    }

    // What item is in the slot? Returns ItemEquipment.
    public ItemEquipment getItemInSlot(EquipSlot slot) {
        foreach (ItemEquipment e in equippedItems) {
            if (e.curSlot == slot) { return e; }
        }
        return null;
    }

    public List<ItemEquipment> getEquippedItems() {
        return equippedItems;
    }
}
