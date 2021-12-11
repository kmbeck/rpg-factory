using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * An item like a bag or pouch or quiver.
 * * * * */
public class ItemContainer : ItemEquipment
{
    private static char SLOT = 'X';
    private static char NOSLOT = '_';

    void Start() {
        // Generate slots in container...
        Initialize();
    }

    public void Initialize() {
        initSlots();
        Debug.Log($"Empty Slots: {emptySlotCount().ToString()}");
    }

    // Add item to container if specified slot = null.
    public void addItem(int slotKey, ItemInventory item) {
        if (!data.items[slotKey] == null) {
            data.items[slotKey] = item;
        }
    }

    // Remove item from container. Returns removed item as GameObject.
    public ItemInventory removeItem(int slotKey) {
        ItemInventory retval = data.items[slotKey];
        data.items[slotKey] = null;
        return retval;
    }

    // How many empty slots are there in the container?
    public int emptySlotCount() {
        int count = 0;
        foreach (int key in data.items.Keys) {
            if (data.items[key] == null) {
                count += 1;
            }
        }
        return count;
    }

    // Use data.layout.text to generate storage slots in the container.
    private void initSlots() {
        data.items = new Dictionary<int,ItemInventory>();
        string[] layout = data.layout.text.Split('\n');
        int slotNum = 0;
        // Give slots int label starting w/ 0 at the top left.
        foreach (string s in layout) {
            foreach (char c in s) {
                if (c == SLOT) {
                    data.items[slotNum] = null;
                    slotNum++;
                }
            }
        }
    }
}

/* * * * *
 * Containers with different StorageTypes are allowed to store different
 * things.
 * * * * */
public enum StorageType {
    NONE,       // No slot / no available slot.    
    UNIVERSAL,  // Can hold anything.
    ITEM,       // Can only hold items (not equipment)
    AMMO,       // Can only hold ammo.
    SPECIFIC,   // Only holds specific items.
}