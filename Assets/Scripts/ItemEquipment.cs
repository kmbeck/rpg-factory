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
    WEAPON     = 0,   // 1h weapons
    WEAPON2H   = 1,   // 2h weapons
    SHIELD     = 2,   // all shields
    ARMOR      = 3,   // all armor & clothes
    TRINKET    = 4,   // all rings,amulets,accessories
    AMMO       = 5,   // all bolts, bullets, arrows
    CONTAINER  = 6,   // all bags, pouches, etc.
}

// List of equipment slots that exist across all characters
// in the game.
public enum EquipSlot {
    NONE        = 0,
    HEAD        = 1,
    HANDS       = 2,
    CHEST       = 3,
    LEGS        = 4,
    FEET        = 5,
    HAND_1      = 6,    // Held
    HAND_2      = 7,    // Held
    TRINKET_1   = 8,
    TRINKET_2   = 9,
    TRINKET_3   = 10,
    TRINKET_4   = 11,
    BAG         = 12,   // Storage
    BELT        = 13,   // Storage
    PACK_1      = 14,   // Extra Storage
    PACK_2      = 15,   // Extra Storage
    PACK_3      = 16,   // Extra Storage
    PACK_4      = 17,   // Extra Storage
}