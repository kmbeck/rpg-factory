using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Defines statistics that make up a character in the RPG system.
 * * * * */
[CreateAssetMenu(fileName="NewStatBlock",menuName="Scriptable Objects/StatBlock")]
public class SOStatBlock : ScriptableObject
{
    public List<CharStat> stats;
}

public enum CharStat {
    HP,
    MANA,
    STRENGTH,
    SPEED,
    WILL
}
