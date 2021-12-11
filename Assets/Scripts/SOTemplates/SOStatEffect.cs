using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SOStatEffect : ScriptableObject
{
    public string uniqueID;
    public string displayName;
    [Tooltip("Can this effect stack?")]
    public bool stackable;
    [Tooltip("How many times can this effect stack?")]
    public int maxStackSize;
}