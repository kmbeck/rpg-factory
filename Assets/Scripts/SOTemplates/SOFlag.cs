using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewFlag", menuName="Scriptable Objects/Flag")]
public class SOFlag : SOOrigin
{
    [Tooltip("Tells the game which value below should be used for this Flag.")]
    public FlagDataType dataType;
    [Tooltip("Integer value. Default = -1000000")]
    public int iVal;
    [Tooltip("String value. Default = \"\"")]
    public string sVal;
    [Tooltip("Boolean value. Default = false")]
    public bool bVal;
    [Tooltip("Float value. Default = -1000000.0")]
    public float fVal;

    public static int I_DEFAULT = -1000000;
    public static string S_DEFAULT = "";
    public static bool B_DEFAULT = false;
    public static float F_DEFAULT = -1000000.0f;
}

public enum FlagDataType {
    INT = 0,
    STRING = 1,
    BOOL = 2,
    FLOAT = 3
}