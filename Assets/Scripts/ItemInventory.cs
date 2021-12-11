using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Items that can be stored in the inventory.
 * * * * */

public class ItemInventory : ItemOrigin
{
    public delegate Dictionary<string,string> OnUseHandler(List<string> func_ids);
    public delegate Dictionary<string,string> OnGetHandler(List<string> func_ids);

    // Called when the Player uses the item.
    public Dictionary<string,string> OnUse(OnUseHandler h, List<string> func_ids) {
        return h(func_ids);    
    }

    // Called when the item is placed in the Player's inventory.
    public Dictionary<string,string> OnGet(OnGetHandler h, List<string> func_ids) {
        return h(func_ids);
    }
}
