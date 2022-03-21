using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

/* * * * * * * 
 * GSCRIPT IDEAS
 * -------------
 * Features:
 *  - White-space & case sensitive.
 *  - easy references to all objects in SODB
 *  - easy reference to any number of programmer-defiend interfaces for
 *      manipulating the game.
 *  - dynamically execute other gscript events via and EXEC_EVENT() function
 *  - freely manipulate existing flags (only copies not original SOs)
 *  - EVENT TAGS
 * COMPILER NOTES
 * --------------
 *  - compiles down to a list of functions and operations to be executed.
 *      This can basically be function calls from the aformentioned interface,
 *      Math, string manipulation, calling functions & referencing data from SODB
 *      and other game objects.
 *  - this implies that every object in the game that needs to be controlled or
 *      manipulated by an event will need to somehow make itself visible to the
 *      thing that is executing event commands.
 *  - scripts should be 'threadable' and there will probably need to be special
 *      functionality for when an script invokes another 'parallel' script to
 *      executed as a coroutine or something.
 * * * * * * */

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
    //const string op_Modulus = "%";
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
        }
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
                // TODO: There are better was to do it than looping (RE.INDEX/RE.LENGTH)...
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
                // TODO: There are better was to do it than looping (RE.INDEX/RE.LENGTH)...
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
                // TODO: There are better was to do it than looping (RE.INDEX/RE.LENGTH)...
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
        ASTParser parser = new ASTParser();
        return new ASTParser().parse(tokens);
    }

    // Traverse over our parsed Statements and ensure that there are no grammar
    // or scope errors in the program.
    void traverse(List<Statement> program) {
        List<string> errors = new List<string>();
        Scope scope = new Scope();
        context = new VarContext();
        exceptions = new GScriptExceptionHandler();
        foreach(Statement s in program) {
            traverseStatement(s);
        }
        Debug.Log(context.ToString());
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

            foreach (Statement c in s.children) {
                traverseStatement(c);
            }
        }

        // Traverse over any children of this statementhasVar
        // TODO: should this be done here or in a wrapper function???
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
        foreach (ExprNode c in e.children) {
            traverseExpr(c);
        }
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
        // Different cases for different binary ops?
        if (e.tType == TType.OP_ADDITION) {
            if (e.children[0].vType != VType.INT   && 
                e.children[0].vType != VType.FLOAT &&
                e.children[0].vType != VType.STRING) {
                exceptions.log($"Error (ln: {e.lineNum}): Invalid type for operator {e.tType}, {e.children[0].vType}.");
                exceptions.log(context.ToString());
            }
            if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
                exceptions.log(context.ToString());
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
                exceptions.log(context.ToString());
            }
            if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
                exceptions.log(context.ToString());
            }
        }
        else if (e.tType == TType.OP_AND || e.tType == TType.OP_OR) {
            if (e.children[0].vType != VType.BOOL) {
                exceptions.log($"Error (ln: {e.lineNum}): Invalid type for operator {e.tType}, {e.children[0].vType}.");
                exceptions.log(context.ToString());
            }
            if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
                exceptions.log(context.ToString());
            }
        }
        else if (e.tType == TType.OP_EQUALITY || e.tType == TType.OP_NOTEQUALS) {
            if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
                exceptions.log(context.ToString());
            }
        }
        else if (e.tType == TType.OP_ASSIGNMENT) {
            if (e.children[0].eType != EType.IDENTIFIER) {
                exceptions.log($"Error (ln: {e.lineNum}): Cannot assign a value to an expression of type {e.children[0].eType}");
                exceptions.log(context.ToString());
            }
            else if (e.children[0].vType != e.children[1].vType) {
                exceptions.log($"Error (ln: {e.lineNum}): Type mismatch for operator {e.tType}, {e.children[0].vType} and {e.children[1].vType}.");
                exceptions.log(context.ToString());
            }
        }
    }

    void traverseUnaryExpr(ExprNode e) {
        // traverseExpr(e.children[0]);
        if (e.tType == TType.OP_NEGATION) {
            if (e.children[0].vType != VType.BOOL) {
                Debug.Log($"Error (ln: {e.lineNum}): Invalid type for operator {e.tType}, {e.children[0].vType}.");
                Debug.Log(context.ToString());
            }
        }
        else if (e.tType == TType.OP_INVERSE) {
            if (e.children[0].vType != VType.INT || e.children[0].vType != VType.FLOAT) {
                Debug.Log($"Error (ln: {e.lineNum}): Invalid type for operator {e.tType}, {e.children[0].vType}.");
                Debug.Log(context.ToString());
            }
        }
    }

    void traverseFunctionCallExpr(ExprNode e) {
        //TODO
    }

    void traverseIndexingExpr(ExprNode e) {
        //TODO
    }

    // void translate(List<Statement> program) {

    // }

    // void transform() {

    // }
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

    // Create first 'global' level scope and populate it with references to the
    // SODB libraries.
    private void initGlobalScope() {
        pushScope();
        //ScopeVar[] libIdentifiers = SODB.inst.getContextualizedLibValues();
        //foreach (ScopeVar v in libIdentifiers) { addVar(v); }
        // TODO: add function identfires here from EventInterface?
        //      //Functions need to have the types of each parameter defined...
    }

    public string ToString() {
        string retval = "";
        foreach(Scope s in scopes) {
            retval += s.ToString();
        }
        retval += "\n* * * * * * * * * * * * * * * * * * * *";
        return retval;
    }
}

// Keeps track of what variables are defined in the current scope.
public class Scope {
    public List<ScopeVar> vars;

    public Scope() {
        vars = new List<ScopeVar>();
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

    public string ToString() {
        string retval = "";
        foreach(ScopeVar v in vars) {
            retval += v.ToString() + "\n";
        }
        return retval;
    }
}

// A variable or function that is visible in the current scope.
public class ScopeVar {
    public string name;
    public VType type;
    //TODO: add List<ScopeVar> parameters in case this particular ScopeVar is a function.
    // Do functions need to be separate?
    // type would be RETURN type of the function...

    public ScopeVar(string _name, VType _type) {
        name = _name;
        type = _type;
    }

    public string ToString() {
        return $"var: {name}, {type.ToString()}";
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
