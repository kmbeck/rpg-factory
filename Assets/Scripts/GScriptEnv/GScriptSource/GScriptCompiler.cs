using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class GScriptCompiler
{
    // Used by traverse() to ensure all variables are used correctly.
    // private VarContext context;
    private GScriptExceptionHandler exceptions;

    public GScriptCompiler() {
    }

    // Validate the input string of code.
    public void validate(string program) {
        exceptions = new GScriptExceptionHandler();
        List<Token> tokens = tokenize(program);
        List<Statement> statements = parse(tokens.ToArray());
        traverse(statements);
        if (!exceptions.empty()) {
            exceptions.printAllExceptions();
        }
        else {
            Debug.Log("Script validated successfully!");
            // TODO: translate call for debugging only!
            string csCode = translate(statements);
        }
    }

    // Compile a single event and return as a C# function.
    public string compileEvent(SOEvent e) {
        Debug.Log($"Compiling Event Script: {e.uniqueID}");
        exceptions = new GScriptExceptionHandler();
        List<Token> tokens = tokenize(e.script);
        List<Statement> statements = parse(tokens.ToArray());
        traverse(statements);
        if (!exceptions.empty()) {
            exceptions.printAllExceptions();
            return "";
        }
        string headerCode = $"public static void EVENT_{e.uniqueID.ToUpper()}() {{\n";
        string bodyCode = translate(statements);
        string footerCode = "}\n\n";
        return headerCode + bodyCode + footerCode;
    }

    // Compile scripts into C# code in Unity project.
    public void compileAllEvents() {
        string bodyCode = "";
        foreach(SOEvent e in SODB.LIB_EVENT.lib.Values) {
            bodyCode += compileEvent(e);
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

    // Translate input string into a list of Tokens & return.
    List<Token> tokenize(string program) {
        return new GScriptTokenizer().tokenize(program);
    }

    // Parse Token list returned from tokenize(). Returns list of statements
    // equivalent to the entire program.
    //      If an error is encountered while parsing, we must throw an error & return.
    List<Statement> parse(Token[] tokens) {
        return new GScriptASTParser().parse(tokens);
    }

    // Traverse over our parsed Statements and ensure that there are no grammar
    // or scope errors in the program.
    void traverse(List<Statement> statements) {
        new GScriptTraverser().traverse(statements);
    }

    // Translate list of Statements and translate them into a C# function.
    string translate(List<Statement> statements) {
        return new GScriptTranslator().translate(statements);
    }
}