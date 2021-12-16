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

    [Header("Interactions With Other StatMods")]
    [Tooltip("When this mod is applied, remove these mods from the character.")]
    public List<SOStatMod> overwriteMods;
    [Tooltip("Instead of applying this mod, reduce the stack count of these mods if they are present.")]
    public List<SOStatMod> reduceMods; 
    [Tooltip("This mod cannot be applied if one of these exists on the character.")]
    public List<SOStatMod> incompatableMods;
}

public enum Operator {
    ADD = 0, MULTIPLY = 1, SET = 2
}

/* TODO: ObjLib currently requires template type to be descendant of ItemOrigin
   should this be changed to something more generic so a StatMod object could
   also be generated into an ObjLib (all it would need is a 'data' field...).
   Create new object type named 'DynamicObject' or something where all of these
   types can descend from??? 
   
   A generic class that can take any descendant of Scriptable Object and store it in
   a dictionary.
    ---> a descendant of this class that can use a prefab to us SO to instantiate objects...
   */