using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Items that can be equipped to the Player. Equipment type enum is defined here.
 * * * * */
public class ItemEquipment : ItemInventory
{

    public bool equipped;               // Is the item currently equipped?
    public EquipSlot curSlot;           // What slot is the Item equipped to?
    public delegate Dictionary<string,string> OnEquipHandler(List<string> func_ids);
    public delegate Dictionary<string,string> OnUnequipHandler(List<string> func_ids);

    // Start is called before the first frame update
    void Start()
    {

    }

    // Called when the Player uses the item.
    public Dictionary<string,string> OnEquip(OnEquipHandler h, List<string> func_ids) {
        equipped = true;
        return h(func_ids);
    }

    // Called when the item is placed in the Player's inventory.
    public Dictionary<string,string> OnUnequip(OnUnequipHandler h, List<string> func_ids) {
        equipped = false;
        return h(func_ids);
    }
}

// General categores of equipment.
public enum EquipType {
    WEAPON,     // 1h weapons
    WEAPON2H,   // 2h weapons
    SHIELD,     // all shields
    ARMOR,      // all armor & clothes
    TRINKET,    // all rings,amulets,accessories
    AMMO,       // all bolts, bullets, arrows
    CONTAINER   // all bags, pouches, etc.
}

// List of equipment slots that exist across all characters
// in the game.
public enum EquipSlot {
    NONE,
    HEAD,
    HANDS,
    CHEST,
    LEGS,
    FEET,
    HAND_1,     // Held
    HAND_2,     // Held
    TRINKET_1,
    TRINKET_2,
    TRINKET_3,
    TRINKET_4,
    BAG,        // Storage
    BELT,       // Storage
    PACK_1,     // Extra Storage
    PACK_2,     // Extra Storage
    PACK_3,     // Extra Storage
    PACK_4,     // Extra Storage
}