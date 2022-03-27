using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GScriptCompiler
{
    // Regex for finding all IDENTIFIERS.
    Regex rxIdentifier = new Regex("[a-zA-Z_]");    //TODO: need numeral support for identifiers (0-9)!
    // Regex for finding all NUMBERS (float + int).
    Regex rxNumeral = new Regex("[0-9.]");
    // Regex for finding all STRINGS.
    Regex rxString = new Regex("\"");
    // Regex for finding BOOL LITERALS
    Regex rxBool = new Regex("(true|false)");

    const string KEY_IF = "if";
    const string KEY_ELIF = "elif";
    const string KEY_ELSE = "else";
    const string KEY_WHILE = "while";
    const string KEY_WAIT = "wait";
    const string KEY_BOOL = "bool";
    const string KEY_INT = "int";
    const string KEY_STR = "str";
    const string KEY_FLOAT = "float";
    const string WS_SPACE = " ";
    const string WS_NEWLINE = "\n";
    const string WS_TAB = "\t";

    const string L_BRACKET = "[";
    const string R_BRACKET = "]";
    const string L_PAREN = "(";
    const string R_PAREN = ")";

    const string OP_ASSIGNMENT = "=";
    const string OP_ADDITION = "+";
    const string OP_SUBRACTION = "-";
    const string OP_MULTIPLICATION = "*";
    const string OP_DIVISION = "/";
    //TODO: const string op_Modulus = "%";
    const string OP_EQUALITY = "==";
    const string OP_NOTEQUALS = "!=";
    const string OP_GREATER = ">";
    const string OP_GREATEROREQUAL = ">=";
    const string OP_LESS = ">";
    const string OP_LESSOREQUAL = "<=";
    const string OP_AND = "&&";
    const string OP_OR = "||";
    const string OP_COMMA = ",";
    const string OP_NEGATION = "!";
    const string OP_INVERSE = "-";
    const string EOF = "EOF";
    //TODO: "." access operator for referencing members!

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
            Debug.Log(csCode);
        }
    }

    // Compile scripts into C# code in Unity project.
    public void compile(string program) {
        List<Token> tokens = tokenize(program);
        List<Statement> statements = parse(tokens.ToArray());
        traverse(statements);
        if (!exceptions.empty()) {
            exceptions.printAllExceptions();
            return;
        }
        // Go on to generate c# code here...
        string cCode = translate(statements);
    }

    // Convert input into a list of Tokens.
    // NOTE: Every time idx is incremented, xLoc must also be incremented!!!
    //       Otherwise error messages will point to the wrong location in source code LOL
    public List<Token> tokenize(string input) {
        char[] inputChars = input.ToCharArray();    // Expand input string to char array.
        List<Token> tokens = new List<Token>();     // Return value.
        string buf = "";
        int idx = 0;
        int xLoc = 0;   // Line column of token.
        int yLoc = 1;   // Line number of token.
        while(idx < inputChars.Length) {
            buf += inputChars[idx];
            if (buf == L_BRACKET) {                                     // [
                tokens.Add(new Token(TType.L_BRACKET,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == R_BRACKET) {                                // ]
                tokens.Add(new Token(TType.R_BRACKET,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == L_PAREN) {                                  // (
                tokens.Add(new Token(TType.L_PAREN,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == R_PAREN) {                                  // )
                tokens.Add(new Token(TType.R_PAREN,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_ASSIGNMENT) {                            // = & ==
                if (inputChars[idx + 1] == '=') {                       
                    idx++;
                    xLoc++;
                    buf += inputChars[idx];
                    tokens.Add(new Token(TType.OP_EQUALITY,buf,new int[2] {xLoc, yLoc}));
                    buf = "";
                }
                else {
                    tokens.Add(new Token(TType.OP_ASSIGNMENT,buf,new int[2] {xLoc, yLoc}));
                    buf = "";
                }
            }
            else if (buf == OP_ADDITION) {                              // +
                tokens.Add(new Token(TType.OP_ADDITION,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_SUBRACTION) {                            // -
                tokens.Add(new Token(TType.OP_SUBTRACTION,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_MULTIPLICATION) {                        // *
                tokens.Add(new Token(TType.OP_MULTIPLICATION,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_DIVISION) {                              // /
                tokens.Add(new Token(TType.OP_DIVISION,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_NEGATION) {                              // ! && !=
                if (inputChars[idx + 1] == '=') {
                    idx++;
                    buf += inputChars[idx];
                    tokens.Add(new Token(TType.OP_NOTEQUALS,buf,new int[2] {xLoc, yLoc}));
                    buf = "";
                }
                else {
                    tokens.Add(new Token(TType.OP_NEGATION,buf,new int[2] {xLoc, yLoc}));
                    buf = "";
                }
            }
            else if (buf == OP_NOTEQUALS) {                             // !=
                tokens.Add(new Token(TType.OP_NOTEQUALS,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_GREATER) {                               // >
                tokens.Add(new Token(TType.OP_GREATER,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_GREATEROREQUAL) {                        // >=
                tokens.Add(new Token(TType.OP_GREATEROREQUAL,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_LESS) {                                  // <
                tokens.Add(new Token(TType.OP_LESS,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_LESSOREQUAL) {                           // <=
                tokens.Add(new Token(TType.OP_LESSOREQUAL,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_AND) {                                   // &&
                tokens.Add(new Token(TType.OP_AND,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_OR) {                                    // ||
                tokens.Add(new Token(TType.OP_OR,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_COMMA) {                                 // ,
                tokens.Add(new Token(TType.OP_COMMA,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == OP_INVERSE) {                               // -
                tokens.Add(new Token(TType.OP_INVERSE,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == WS_SPACE) {                                 // ' '
                // Check if it is a tab (4 consecutive spaces)
                int count = 1;
                while (idx + 1 < inputChars.Length &&
                        inputChars[idx + 1] == ' ') {
                    idx++;
                    xLoc++;
                    count++;
                }
                for (int i = 0; i < count / 4; i++) {
                    tokens.Add(new Token(TType.WS_TAB, "\t", new int[2] {xLoc, yLoc}));
                }

                for (int i = 0; i < count % 4; i++) {
                    //tokens.Add(new Token("ws_space"," "));
                }
                buf = "";
            }
            else if (buf == WS_TAB) {                                   // \t
                tokens.Add(new Token(TType.WS_TAB,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (buf == WS_NEWLINE) {                               // \n
                tokens.Add(new Token(TType.WS_NEWLINE,buf,new int[2] {xLoc, yLoc}));
                xLoc = 0;
                yLoc++;
                buf = "";
            }
            else if (rxBool.Match(buf).Success) {
                tokens.Add(new Token(TType.BOOL_LITERAL,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            else if (rxIdentifier.Match(buf).Success) {                 // ALL IDENTIFIERS & KEYWORDS.
                // Read up until end of current Identifier.
                // Check to ensure we havent reached end of input.
                while (idx + 1 < inputChars.Length &&
                        rxIdentifier.Match(inputChars[idx + 1].ToString()).Success) {
                    idx++;
                    xLoc++;
                    buf += inputChars[idx];
                }

                // Determine if we are looking at a keyword.
                if (buf == KEY_IF) {                                        // if
                    tokens.Add(new Token(TType.KEY_IF,buf,new int[2] {xLoc, yLoc}));
                }
                else if (buf == KEY_ELIF) {                                 // elif
                    tokens.Add(new Token(TType.KEY_ELIF,buf,new int[2] {xLoc, yLoc}));
                }
                else if (buf == KEY_ELSE) {                                 // else
                    tokens.Add(new Token(TType.KEY_ELSE,buf,new int[2] {xLoc, yLoc}));
                }
                else if (buf == KEY_WHILE) {                                // while
                    tokens.Add(new Token(TType.KEY_WHILE,buf,new int[2] {xLoc, yLoc}));
                }
                else if (buf == KEY_WAIT) {                                 // wait
                    tokens.Add(new Token(TType.KEY_WAIT,buf,new int[2] {xLoc, yLoc}));
                }
                else if (buf == KEY_BOOL) {                                 // bool
                    tokens.Add(new Token(TType.KEY_BOOL,buf,new int[2] {xLoc, yLoc}));
                }
                else if (buf == KEY_FLOAT) {                                // float
                    tokens.Add(new Token(TType.KEY_FLOAT,buf,new int[2] {xLoc, yLoc}));
                }
                else if (buf == KEY_INT) {                                  // int
                    tokens.Add(new Token(TType.KEY_INT,buf,new int[2] {xLoc, yLoc}));
                }
                else if (buf == KEY_STR) {                                  // str
                    tokens.Add(new Token(TType.KEY_STR,buf,new int[2] {xLoc, yLoc}));
                }
                else {                                                      // IDENTIFIER
                    tokens.Add(new Token(TType.IDENTIFIER,buf,new int[2] {xLoc, yLoc}));
                }
                buf = "";
            }
            else if (rxNumeral.Match(buf).Success) {                    // INT & FLOAT LITERAL
                // Read up until end of current INT or FLOAT LITERAL.
                while (idx + 1 < inputChars.Length &&
                        rxNumeral.Match(inputChars[idx + 1].ToString()).Success) {
                    idx++;
                    xLoc++;
                    buf += inputChars[idx];
                }

                if (buf.Contains(".")) {
                    tokens.Add(new Token(TType.FLOAT_LITERAL,buf,new int[2] {xLoc, yLoc}));
                }
                else {
                    tokens.Add(new Token(TType.INT_LITERAL,buf,new int[2] {xLoc, yLoc}));
                }
                buf = "";
            }
            else if (rxString.Match(buf).Success) {                     // STRING LITERAL
                // Read up until end of current STRING LITERAL.
                while (inputChars[idx + 1] != '\"') {   // Break out before closing "
                    idx++;
                    xLoc++;
                    buf += inputChars[idx];
                    // TODO: throw error if end of input encountered here or wait until later?
                }
                // Push closing "
                // TODO: rule where end of string must be located on the same line?
                //      How would you handle whitespace characters encountered inside of a string?
                idx++;
                xLoc++;
                buf += inputChars[idx];
                tokens.Add(new Token(TType.STR_LITERAL,buf,new int[2] {xLoc, yLoc}));
                buf = "";
            }
            idx++;
            xLoc++;
        }
        // string temp = "";
        // foreach (Token t in tokens) {
        //     temp += t.ToString() + "|";
        // }
        // Debug.Log(temp);
        return tokens;
    }

    // Parse Token list returned from tokenize(). Returns list of statements
    // equivalent to the entire program.
    //      If an error is encountered while parsing, we must throw an error & return.
    public List<Statement> parse(Token[] tokens) {
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
        Debug.Log(this.ToString());
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

// A token.
public struct Token {
    public TType type;
    public string value;
    public int[] location;  // x (col) & y (row) position of this token in the original source code.

    public Token(TType _type, string _value, int[] _location) {
        type = _type;
        value = _value;
        location = _location;
    }

    public string ToString() {
        return value;
    }
}

// Enum defines all types of Tokens.
//      order of decleration == precedence (if applicable)
public enum TType {
    L_PAREN,
    R_PAREN,
    L_BRACKET,
    R_BRACKET,
    OP_NEGATION,
    OP_INVERSE,
    OP_MULTIPLICATION,
    OP_DIVISION,
    OP_ADDITION,
    OP_SUBTRACTION,
    OP_LESS,
    OP_LESSOREQUAL,
    OP_GREATER,
    OP_GREATEROREQUAL,
    OP_EQUALITY,
    OP_NOTEQUALS,
    OP_AND,
    OP_OR,
    OP_ASSIGNMENT,
    OP_COMMA,
    WS_TAB,
    WS_NEWLINE,
    KEY_IF,
    KEY_ELIF,
    KEY_ELSE,
    KEY_WHILE,
    KEY_WAIT,
    KEY_BOOL,
    KEY_FLOAT,
    KEY_INT,
    KEY_STR,
    IDENTIFIER,
    BOOL_LITERAL,
    FLOAT_LITERAL,
    INT_LITERAL,
    STR_LITERAL,
    NONE
}