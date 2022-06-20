using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/***
 * Defines the attributes used by GScript to help with code reflection when
 * constructing the global scope for the GScript compiler.
 ***/

// Attribute denotes if this function is exposed to GScript.
[AttributeUsage(AttributeTargets.All)]
public class GScript : Attribute
{

}