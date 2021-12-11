using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewStatMod",menuName="Scriptable Objects/StatMod")]
public class SOStatMod : ScriptableObject
{
    public string uniqueID;
    public string displayName;
    [Tooltip("What stat is this going to effect.")]
    public CharStat tgtStat;
    [Tooltip("How to modify the value of the specified stat")]
    public Operator operation;
    public float value;
    [Tooltip("Can this effect stack?")]
    public bool stackable;
    [Tooltip("How many times can this effect stack?")]
    public int maxStackSize;
}

public enum Operator {
    ADD, MULTIPLY, SET
}


/* TODO: ObjLib currently requires template type to be descendant of ItemOrigin
   should this be changed to something more generic so a StatMod object could
   also be generated into an ObjLib (all it would need is a 'data' field...).
   Create new object type named 'DynamicObject' or something where all of these
   types can descend from??? */