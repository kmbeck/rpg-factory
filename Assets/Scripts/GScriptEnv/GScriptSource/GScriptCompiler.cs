using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using UnityEngine;

public class GScriptCompiler
{
    // Used by traverse() to ensure all variables are used correctly.
    private VarContext context;
    private GScriptExceptionHandler exceptions;

    // Validate the input string of code.
    public void validate(string program) {
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
        return new ASTParser().parse(tokens);
    }

    // Traverse over our parsed Statements and ensure that there are no grammar
    // or scope errors in the program.
    void traverse(List<Statement> statements) {
        List<string> errors = new List<string>();
        Scope scope = new Scope();
        context = new VarContext();
        exceptions = new GScriptExceptionHandler();
        foreach(Statement s in statements) {
            traverseStatement(s);
        }
    }

    // Translate list of Statements and translate them into a C# function.
    string translate(List<Statement> statements) {
        return new GScriptTranslator().translate(statements);
    }

    // Traverse a Statement and all of its child Statements & Expressions
    void traverseStatement(Statement s) {
        switch (s.type) {
            case SType.EXPR:
                traverseExprStatement(s);
                break;
            case SType.IF:
                traverseIfStatement(s);
                break;
            case SType.ELIF:
                traverseElifStatement(s);
                break;
            case SType.ELSE:
                traverseElseStatement(s);
                break;
            case SType.VAR_DEF:
                traverseVarDefStatement(s);
                break;
            case SType.WAIT:
                traverseWaitStatement(s);
                break;
            case SType.WHILE:
                traverseWhileStatement(s);
                break;
            case SType.NONE:
                //Error: cannot declare variable of NONE type.
                break;
        }

        // Traverse over any children of this statementhasVar
        foreach(Statement child in s.children) {
            traverseStatement(child);
        }
    }

    void traverseExprStatement(Statement s) {
        traverseExpr(s.expr);
    }

    void traverseIfStatement(Statement s) {
        traverseExpr(s.expr);
        if (s.expr.vType != VType.BOOL) {
            // ERROR: if statement does not have a boolean expression.
            exceptions.log($"Error (ln: {s.expr.lineNum}): if statement argument must be a bool expression.");
        }
    }

    void traverseElifStatement(Statement s) {
        traverseExpr(s.expr);
        if (s.expr.vType != VType.BOOL) {
            // ERROR: if statement does not have a boolean expression.
            exceptions.log($"Error (ln: {s.expr.lineNum}): if statement argument must be a bool expression.");
        }
    }

    void traverseElseStatement(Statement s) {
        // Do nothing?
    }

    void traverseVarDefStatement(Statement s) {
        // Traverse all child expressions.
        foreach(ExprNode e in s.expr.children) {
            traverseExpr(e);
        }
        // Check to ensure var does not already exist in scope...
        if (context.hasVar(s.expr.children[0].value)) {
            exceptions.log($"Error (ln: {s.expr.lineNum}): identifier '{s.expr.children[0].value}' already exists in scope.");
        }
        else if (s.varDefVType != s.expr.children[1].vType) {
            exceptions.log($"Error (ln: {s.expr.lineNum}): cannot assign {s.expr.children[1].vType.ToString()} to {s.varDefVType.ToString()} value.");
        }
        else {
            // Add new var to context.
            context.addVar(s.expr.children[0].value, s.varDefVType);
        }
    }

    void traverseWaitStatement(Statement s) {
        traverseExpr(s.expr);
        if (s.expr.vType != VType.INT && s.expr.vType != VType.FLOAT) {
            // ERROR: wait statement requires int or float expression.
            exceptions.log($"Error (ln: {s.expr.lineNum}): wait statement argument must be an int or float expression.");
        }
    }

    void traverseWhileStatement(Statement s) {
        traverseExpr(s.expr);
        if (s.expr.vType != VType.BOOL) {
            // ERROR: while statement does not have a boolean expression.
            exceptions.log($"Error (ln: {s.expr.lineNum}): while statement argument must be a bool expression.");
        }
    }

    // Traverse an Expression and all of it's children.
    void traverseExpr(ExprNode e) {
        // First traverse all children of this expression.
        foreach (ExprNode c in e.children) {
            traverseExpr(c);

            // If c has a valid eType, adjust this eType accordingly...
            if (e.vType == VType.NONE && c.vType != VType.NONE) {
                e.vType = c.vType;
            }
        }

        // Now we traverse the expression node in the parameter.
        switch (e.eType) {
            case EType.BINARY:
                traverseBinaryExpr(e);
                break;
            case EType.EXPRESION:
                break;
            case EType.FUNCTION:
                traverseFunctionCallExpr(e);
                break;
            case EType.IDENTIFIER:
                traverseIdentifierExpr(e);
                break;
            case EType.INDEXING:
                traverseIndexingExpr(e);
                break;
            case EType.LITERAL:
                traverseLiteralExpr(e);
                break;
            case EType.UNARY:
                traverseUnaryExpr(e);
                break;
        }
    }

    void traverseIdentifierExpr(ExprNode e) {
        if (context.hasVar(e.value)) {
            e.vType = context.getVar(e.value).type;
        }
    }

    void traverseLiteralExpr(ExprNode e) {
        // Do nothing because expression vType already set by parser.
    }

    void traverseBinaryExpr(ExprNode e) {
        // These binery operators always evaluate to a bool, reguardless of childrens type.
        if (e.tType == TType.OP_GREATER         ||
            e.tType == TType.OP_GREATEROREQUAL  ||
            e.tType == TType.OP_LESS            ||
            e.tType == TType.OP_LESSOREQUAL     ||
            e.tType == TType.OP_EQUALITY        ||
            e.tType == TType.OP_NOTEQUALS) 
        {
            e.vType = VType.BOOL;
        }

        // Different cases for different binary ops?
        if (e.tType == TType.OP_ADDITION) {
            if (e.children[0].vType != VType.INT   && 
                e.children[0].vType != VType.FLOAT &&
                e.children[0].vType != VType.STRING) {
                exceptions.log($"Error (ln: {e.lineNum}): Invalid type for operator {e.tType}, {e.children[0].vType}.");
            }
            if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
            }
        }
        else if (e.tType == TType.OP_SUBTRACTION    ||
                 e.tType == TType.OP_MULTIPLICATION ||
                 e.tType == TType.OP_DIVISION       ||
                 e.tType == TType.OP_GREATER        ||
                 e.tType == TType.OP_GREATEROREQUAL ||
                 e.tType == TType.OP_LESS           ||
                 e.tType == TType.OP_LESSOREQUAL) {
            if (e.children[0].vType != VType.INT && e.children[0].vType != VType.FLOAT) {
                exceptions.log($"Error (ln: {e.lineNum}): Invalid type for operator {e.tType}, {e.children[0].vType}.");
            }
            if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
            }
        }
        else if (e.tType == TType.OP_AND || e.tType == TType.OP_OR) {
            if (e.children[0].vType != VType.BOOL) {
                exceptions.log($"Error (ln: {e.lineNum}): Invalid type for operator {e.tType}, {e.children[0].vType}.");
            }
            if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
            }
        }
        else if (e.tType == TType.OP_EQUALITY || e.tType == TType.OP_NOTEQUALS) {
            if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
            }
        }
        else if (e.tType == TType.OP_ASSIGNMENT) {
            if (e.children[0].eType != EType.IDENTIFIER) {
                exceptions.log($"Error (ln: {e.lineNum}): Cannot assign a value to an expression of type {e.children[0].eType}");
            }
            else if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
            }
        }
    }

    void traverseUnaryExpr(ExprNode e) {
        // traverseExpr(e.children[0]);
        if (e.tType == TType.OP_NEGATION) {
            if (e.children[0].vType != VType.BOOL) {
                exceptions.log($"Error (ln: {e.lineNum}): Invalid type for operator {e.tType}, {e.children[0].vType}.");
            }
        }
        else if (e.tType == TType.OP_INVERSE) {
            if (e.children[0].vType != VType.INT || e.children[0].vType != VType.FLOAT) {
                exceptions.log($"Error (ln: {e.lineNum}): Invalid type for operator {e.tType}, {e.children[0].vType}.");
            }
        }
    }

    void traverseFunctionCallExpr(ExprNode e) {
        List<ScopeParam> tempParams = new List<ScopeParam>();
        for (int i = 1; i < e.children.Count; i++) {
            tempParams.Add(new ScopeParam(e.children[i].value, e.children[i].vType, false));
        }
        ScopeFunc tempFunc = new ScopeFunc(e.children[0].value, VType.NONE, tempParams);
        e.vType = VType.NONE;
        if (!context.hasFunc(tempFunc)) {
            string paramStr = "";
            foreach(ScopeParam p in tempParams) {
                paramStr += $"{p.type.ToString()},";
            }
            paramStr = paramStr.Remove(paramStr.Length - 1, 1);
            exceptions.log($"Error (ln: {e.lineNum}): No function matches definition for {tempFunc.name} ({paramStr}).");
        }
        else {
            e.vType = context.getFunc(tempFunc).returnType;
        }
    }

    void traverseIndexingExpr(ExprNode e) {
        //TODO
    }
}

public class VarContext {
    public List<Scope> scopes;

    public VarContext() {
        scopes = new List<Scope>();
        initGlobalScope();
    }

    // Push a new Scope.
    public void pushScope() {
        scopes.Add(new Scope());
    }

    // Pop a Scope.
    public void popScope() {
        scopes.RemoveAt(scopes.Count-1);
    }

    // Add a variable to the current Scope.
    public void addVar(string name, VType type) {
        if (!hasVar(name)) {
            scopes[scopes.Count-1].vars.Add(new ScopeVar(name, type));
        }
    }

    // Add a varaible to the current Scope.
    public void addVar(ScopeVar newVar) {
        if (!hasVar(newVar.name)) {
            scopes[scopes.Count-1].vars.Add(newVar);
        }
    }

    public ScopeVar getVar(string name) {
        foreach(Scope s in scopes) {
            if (s.hasVar(name)) {
                return s.getVar(name);
            }
        }
        return null;
    }

    // Check if the var exists in the current context.
    public bool hasVar(string name) {
        foreach(Scope s in scopes) {
            if (s.hasVar(name)) {
                return true;
            }
        }
        return false;
    }

    public void addFunc(ScopeFunc newFunc) {
        if (!hasFunc(newFunc)) {
            scopes[scopes.Count-1].funcs.Add(newFunc);
        }
    }

    // Yea its weird to use a ScopeFunc to get a scope func...
    // But this way a dummy ScopeFunc can be used to get the full data...
    public ScopeFunc getFunc(ScopeFunc tgtFunc) {
        foreach(Scope s in scopes) {
            if (s.hasFunc(tgtFunc)) {
                return s.getFunc(tgtFunc);
            }
        }
        return null;
    }

    public bool hasFunc(ScopeFunc tgtFunc) {
        foreach (Scope s in scopes) {
            if (s.hasFunc(tgtFunc)) {
                return true;
            }
        }
        return false;
     }

    // Create first 'global' level scope and populate it with references to the
    // SODB libraries.
    private void initGlobalScope() {
        pushScope();
        GScriptContextualizer ctxr = new GScriptContextualizer();

        ScopeVar[] vars = ctxr.getContextualizedScopeVars<SODB>();
        foreach (ScopeVar v in vars) { addVar(v); }

        ScopeFunc[] funcs = ctxr.getContextualizedScopeFuncs<EventInterface>();
        foreach (ScopeFunc f in funcs) { addFunc(f); }
    }

    public string ToString() {
        string retval = "";
        foreach(Scope s in scopes) {
            retval += s.ToString();
        }
        return retval;
    }
}

// Keeps track of what variables are defined in the current scope.
public class Scope {
    public List<ScopeVar> vars;
    public List<ScopeFunc> funcs;

    public Scope() {
        vars = new List<ScopeVar>();
        funcs = new List<ScopeFunc>();
    }

    public bool hasVar(string name) { 
        foreach(ScopeVar v in vars) {
            if (v.name == name) {
                return true;
            }
        }
        return false;
    }

    public ScopeVar getVar(string name) {
        foreach(ScopeVar v in vars) {
            if (v.name == name) {
                return v;
            }
        }
        return null;
    }

    public void addVar(string name, VType type) {
        if (!hasVar(name)) {
            addVar(name, type);
        }
    }

    public bool hasFunc(ScopeFunc tgtFunc) {
        foreach (ScopeFunc f in funcs) {
            if (f.isEqual(tgtFunc)) {
                return true;
            }
        }
        return false;
    }

    public ScopeFunc getFunc(ScopeFunc tgtFunc) {
        foreach (ScopeFunc f in funcs) {
            if (f.isEqual(tgtFunc)) {
                return f;
            }
        }
        return null;
    }

    public void addFunc(ScopeFunc newFunc) {
        if (!hasFunc(newFunc)) {
            funcs.Add(newFunc);
        }
    }

    public string ToString() {
        string retval = "";
        foreach(ScopeVar v in vars) {
            retval += v.ToString() + "\n";
        }
        foreach(ScopeFunc f in funcs) {
            retval += f.ToString() + "\n";
        }
        return retval;
    }
}

// A method that is visible in the current scope.
public class ScopeVar {
    public string name;
    public VType type;

    public ScopeVar(string _name, VType _type) {
        name = _name;
        type = _type;
    }

    public string ToString() {
        return $"var: {name}, {type.ToString()}";
    }
}

// A function that is visible in the current scope.
public class ScopeFunc {
    public string name;
    public VType returnType;
    public List<ScopeParam> parameters;

    public ScopeFunc(string _name, VType _returnType, List<ScopeParam> _parameters) {
        name = _name;
        returnType = _returnType;
        parameters = _parameters;
    }

    // Only returns true if name & all parameters are the same type in the same order.
    public bool isEqual(ScopeFunc other) {
        if (name == other.name && parameters.Count == other.parameters.Count) {
            for(int i = 0; i < parameters.Count; i++) {
                if (!parameters[i].isSameType(other.parameters[i])) {
                    return false;
                }
            }
            return true;
        }
        return false;
    }

    public string ToString() {
        string retval = $"func: {name}, {returnType.ToString()}";
        foreach (ScopeParam p in parameters) { retval += "\n\t" + p.ToString(); }
        return retval;
    }
}

// A parameter for a function that is visible in the current scope.
public class ScopeParam {
    public string name;
    public VType type;
    public bool required;   // Is this a required parameter?

    public ScopeParam(string _name, VType _type, bool _required) {
        name = _name;
        type = _type;
        required = _required;
    }

    public bool isSameType(ScopeParam other) {
        if (type == other.type) {
            return true;
        }
        return false;
    }

    public string ToString() {
        return $"param: {name}, {type.ToString()}, {required}";
    }
}
