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
        if (!exceptions.isEmpty()) {
            exceptions.printAllExceptions();
        }
        else {
            Debug.Log("Script validated successfully!");
        }
    }

    public void compileCSCode() {
        // Get function string for all events.
        List<string> eventFuncCode = new List<string>();
        foreach (SOEvent e in SODB.LIB_EVENT.lib.Values) {
            eventFuncCode.Add(compileEvent(e));
        }

        GScriptEventLibGenerator evt = new GScriptEventLibGenerator();
        GScriptFlagLibGenerator flg = new GScriptFlagLibGenerator();
        flg.genFlagLibFile();
        evt.genEventLibFile(eventFuncCode);
    }

    // Compile a single event and return as a C# function.
    private string compileEvent(SOEvent e) {
        Debug.Log($"Compiling Event Script: {e.uniqueID}");
        exceptions = new GScriptExceptionHandler();
        List<Token> tokens = tokenize(e.script);
        List<Statement> statements = parse(tokens.ToArray());
        traverse(statements);
        if (!exceptions.isEmpty()) {
            exceptions.printAllExceptions();
            return "";
        }
        string headerCode = $"public static void EVENT_{e.uniqueID.ToUpper()}() {{\n";
        string bodyCode = translate(statements);
        string footerCode = "}\n\n";
        return headerCode + bodyCode + footerCode;
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
