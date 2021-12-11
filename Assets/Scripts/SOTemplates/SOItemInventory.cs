using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Data for ItemInventory
 * * * * */

[CreateAssetMenu(fileName="NewInventoryItem",menuName="Scriptable Objects/Item_Inventory")]
public class SOItemInventory : SOItem
{
    public bool dropable;
    public bool stackable;
    public int maxStackSize;
    public bool useInCombat;
    public bool useOutOfCombat;
    public int baseCost;
    public bool destroyOnUse;
}
