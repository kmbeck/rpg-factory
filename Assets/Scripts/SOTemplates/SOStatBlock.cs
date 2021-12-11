using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Defines statistics that make up a character in the RPG system.
 * * * * */
[CreateAssetMenu(fileName="NewStatBlock",menuName="Scriptable Objects/StatBlock")]
public class SOStatBlock : ScriptableObject
{
    // Core Attributes.
    public int core_str;
    public int core_con;
    public int core_dex;
    public int core_wis;
    public int core_int;
    public int core_cha;

    // Attribute Modifiers.
    public int mod_str;
    public int mod_con;
    public int mod_dex;
    public int mod_wis;
    public int mod_int;
    public int mod_cha;
}
