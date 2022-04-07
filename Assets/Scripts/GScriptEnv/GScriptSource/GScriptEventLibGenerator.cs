using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class GScriptEventLibGenerator
{
    // Used by traverse() to ensure all variables are used correctly.
    // private VarContext context;
    private GScriptExceptionHandler exceptions;

    public GScriptEventLibGenerator() {

    }

    // Compile scripts into C# code in Unity project.
    // Invoked from GScriptCompiler.cs
    public void genEventLibFile(List<string> events) {
        string bodyCode = "";
        foreach(string e in events) {
            bodyCode += e;
        }

        string outputFileName = "GScriptEventLibrary.cs";
        string fileHeaderCode = @"
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 *          --- DO NOT EDIT ---
 *  THIS CLASS CONTAINS GENERATED CODE!!!
 *          --- DO NOT EDIT ---
 * * * * */

public abstract class GScriptEventLibrary : MonoBehaviour
{
    public static GScriptEventLibrary inst;

    void Start() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
        }
    }

    void Awake() {
        if (inst != null) {
            Destroy(this);
        }
        else {
            inst = this;
        }
    }
";
        string fileFooterCode = "}";
        string outStr = fileHeaderCode + bodyCode + fileFooterCode;
        StreamWriter sw = new StreamWriter($"Assets/Scripts/GScriptEnv/GScriptSource/GeneratedCode/{outputFileName}");
        sw.WriteLine(outStr);
        sw.Close();
    }
}
