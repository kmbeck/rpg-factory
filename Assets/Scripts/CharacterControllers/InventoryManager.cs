using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Manages character's inventory. Use to add/remove items and get info about
 * what is stored in the inventory.
 * * * * */
 [RequireComponent(typeof(EquipmentManager))]
public class InventoryManager : MonoBehaviour
{
    // ItemContainers (bags, pouches, etc) that the character has
    // access to.
    private List<ItemContainer> containers;

    // The equipment manager attached to this GameObject.
    private EquipmentManager em;

    public void Initialize() {
        em = GetComponent<EquipmentManager>();
        containers = new List<ItemContainer>();
        checkContainers();
        em.OnContainerEquipped += checkContainers;
    }
    
    // Try and find a spot to put the item. Will check all available
    // containers for a match. Should fill TL -> BR. Returns  true
    // if item is inserted, false if not.
    public bool addItem(ItemInventory item) {
        foreach (ItemContainer c in containers) {
            List<int> keys = new List<int>(c.data.items.Keys);
            foreach (int key in keys) {
                if (c.data.items[key] == null) {
                    c.data.items[key] = item;
                    return true;
                }
            }
        }
        return false;
    }
    
    // Remove item from inventory. Returns the Item as a GameObject.
    // Returns null if specified item is not found.
    public ItemInventory removeItem(ItemInventory item) {
        foreach (ItemContainer c in containers) {
            foreach(int key in c.data.items.Keys) {
                if (c.data.items[key] == item) {
                    return c.removeItem(key);
                }
            }
        }
        return null;
    }

    public List<ItemContainer> getContainers() {
        return containers;
    }

    // Check what containers this InventoryManager has access to.
    private void checkContainers() {
        // Get EquipmentManager of this GameObject to determine which
        // containers it can access.
        containers = new List<ItemContainer>();
        if (em == null) { Debug.Log("em is null!"); return; }
        foreach (ItemEquipment item in em.getEquippedItems()) {
            if (item.data.equipType == EquipType.CONTAINER) {
                containers.Add(item.GetComponent<ItemContainer>());
            }
        }
    }
}