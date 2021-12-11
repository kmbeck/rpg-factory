using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Defines what equipment slots are available. Used by EquipmentManager.
 * * * * */
[CreateAssetMenu(fileName="NewEquipmentLayout",menuName="Scriptable Objects/EquipLayout")]
public class SOEquipLayout : ScriptableObject
{
    public List<EquipSlot> slots;
}
