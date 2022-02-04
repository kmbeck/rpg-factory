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

    // TODO: Define order of operations.

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

    // Convert input into a list of Tokens.
    public List<Token> tokenize(string input) {
        char[] inputChars = input.ToCharArray();    // Expand input string to char array.
        List<Token> tokens = new List<Token>();     // Return value.
        string buf = "";
        int idx = 0;
        while(idx < inputChars.Length) {
            buf += inputChars[idx];
            if (buf == L_BRACKET) {                                     // [
                tokens.Add(new Token(TType.L_BRACKET,buf));
                buf = "";
            }
            else if (buf == R_BRACKET) {                                // ]
                tokens.Add(new Token(TType.R_BRACKET,buf));
                buf = "";
            }
            else if (buf == L_PAREN) {                                  // (
                tokens.Add(new Token(TType.L_PAREN,buf));
                buf = "";
            }
            else if (buf == R_PAREN) {                                  // )
                tokens.Add(new Token(TType.R_PAREN,buf));
                buf = "";
            }
            else if (buf == OP_ASSIGNMENT) {                            // = & ==
                if (inputChars[idx + 1] == '=') {                       
                    idx++;
                    buf += inputChars[idx];
                    tokens.Add(new Token(TType.OP_EQUALITY,buf));
                    buf = "";
                }
                else {
                    tokens.Add(new Token(TType.OP_ASSIGNMENT,buf));
                    buf = "";
                }
            }
            else if (buf == OP_ADDITION) {                              // +
                tokens.Add(new Token(TType.OP_ADDITION,buf));
                buf = "";
            }
            else if (buf == OP_SUBRACTION) {                            // -
                tokens.Add(new Token(TType.OP_SUBTRACTION,buf));
                buf = "";
            }
            else if (buf == OP_MULTIPLICATION) {                        // *
                tokens.Add(new Token(TType.OP_MULTIPLICATION,buf));
                buf = "";
            }
            else if (buf == OP_DIVISION) {                              // /
                tokens.Add(new Token(TType.OP_DIVISION,buf));
                buf = "";
            }
            else if (buf == OP_NEGATION) {                              // ! && !=
                if (inputChars[idx + 1] == '=') {
                    idx++;
                    buf += inputChars[idx];
                    tokens.Add(new Token(TType.OP_NOTEQUALS,buf));
                    buf = "";
                }
                else {
                    tokens.Add(new Token(TType.OP_NEGATION,buf));
                    buf = "";
                }
            }
            else if (buf == OP_NOTEQUALS) {                             // !=
                tokens.Add(new Token(TType.OP_NOTEQUALS,buf));
                buf = "";
            }
            else if (buf == OP_GREATER) {                               // >
                tokens.Add(new Token(TType.OP_GREATER,buf));
                buf = "";
            }
            else if (buf == OP_GREATEROREQUAL) {                        // >=
                tokens.Add(new Token(TType.OP_GREATEROREQUAL,buf));
                buf = "";
            }
            else if (buf == OP_LESS) {                                  // <
                tokens.Add(new Token(TType.OP_LESS,buf));
                buf = "";
            }
            else if (buf == OP_LESSOREQUAL) {                           // <=
                tokens.Add(new Token(TType.OP_LESSOREQUAL,buf));
                buf = "";
            }
            else if (buf == OP_AND) {                                   // &&
                tokens.Add(new Token(TType.OP_AND,buf));
                buf = "";
            }
             else if (buf == OP_OR) {                                    // ||
                tokens.Add(new Token(TType.OP_OR,buf));
                buf = "";
            }
            else if (buf == OP_COMMA) {                                 // ,
                tokens.Add(new Token(TType.OP_COMMA,buf));
                buf = "";
            }
            else if (buf == OP_INVERSE) {                               // -
                tokens.Add(new Token(TType.OP_INVERSE,buf));
                buf = "";
            }
            else if (buf == WS_SPACE) {                                 // ' '
                // Check if it is a tab (4 consecutive spaces)
                int count = 1;
                while (idx + 1 < inputChars.Length &&
                        inputChars[idx + 1] == ' ') {
                    idx++;
                    count++;
                }
                for (int i = 0; i < count / 4; i++) {
                    tokens.Add(new Token(TType.WS_TAB, "\t"));
                }

                for (int i = 0; i < count % 4; i++) {
                    //tokens.Add(new Token("ws_space"," "));
                }
                buf = "";
            }
            else if (buf == WS_TAB) {                                   // \t
                tokens.Add(new Token(TType.WS_TAB,buf));
                buf = "";
            }
            else if (buf == WS_NEWLINE) {                               // \n
                tokens.Add(new Token(TType.WS_NEWLINE,buf));
                buf = "";
            }
            else if (rxBool.Match(buf).Success) {
                tokens.Add(new Token(TType.BOOL_LITERAL,buf));
                buf = "";
            }
            else if (rxIdentifier.Match(buf).Success) {                 // ALL IDENTIFIERS & KEYWORDS.
                // Read up until end of current Identifier.
                // TODO: There are better was to do it than looping (RE.INDEX/RE.LENGTH)...
                // Check to ensure we havent reached end of input.
                while (idx + 1 < inputChars.Length &&
                        rxIdentifier.Match(inputChars[idx + 1].ToString()).Success) {
                    idx++;
                    buf += inputChars[idx];
                }

                // Determine if we are looking at a keyword.
                if (buf == KEY_IF) {                                        // if
                    tokens.Add(new Token(TType.KEY_IF,buf));
                }
                else if (buf == KEY_WHILE) {                                // while
                    tokens.Add(new Token(TType.KEY_WHILE,buf));
                }
                else if (buf == KEY_WAIT) {                                 // wait
                    tokens.Add(new Token(TType.KEY_WAIT,buf));
                }
                else if (buf == KEY_BOOL) {                                 // bool
                    tokens.Add(new Token(TType.KEY_BOOL,buf));
                }
                else if (buf == KEY_FLOAT) {                                // float
                    tokens.Add(new Token(TType.KEY_FLOAT,buf));
                }
                else if (buf == KEY_INT) {                                  // int
                    tokens.Add(new Token(TType.KEY_INT,buf));
                }
                else if (buf == KEY_STR) {                                  // str
                    tokens.Add(new Token(TType.KEY_STR,buf));
                }
                else {                                                      // IDENTIFIER
                    tokens.Add(new Token(TType.IDENTIFIER,buf));
                }
                buf = "";
            }
            else if (rxNumeral.Match(buf).Success) {                    // INT & FLOAT LITERAL
                // Read up until end of current INT or FLOAT LITERAL.
                // TODO: There are better was to do it than looping (RE.INDEX/RE.LENGTH)...
                while (idx + 1 < inputChars.Length &&
                        rxNumeral.Match(inputChars[idx + 1].ToString()).Success) {
                    idx++;
                    buf += inputChars[idx];
                }

                if (buf.Contains(".")) {
                    tokens.Add(new Token(TType.FLOAT_LITERAL,buf));
                }
                else {
                    tokens.Add(new Token(TType.INT_LITERAL,buf));
                }
                buf = "";
            }
            else if (rxString.Match(buf).Success) {                     // STRING LITERAL
                // Read up until end of current STRING LITERAL.
                // TODO: There are better was to do it than looping (RE.INDEX/RE.LENGTH)...
                while (inputChars[idx + 1] != '\"') {   // Break out before closing "
                    idx++;
                    buf += inputChars[idx];
                    // Check to ensure we havent reached end of input.
                    // TODO: throw error if end of input encountered here or wait until later?
                    // if (idx + 1 >= inputChars.Length) {
                    //     break;
                    // }
                }
                // Push closing "
                // TODO: rule where end of string must be located on the same line?
                idx++;
                buf += inputChars[idx];
                tokens.Add(new Token(TType.STR_LITERAL,buf));
                buf = "";
            }
            idx++;
        }
        return tokens;
    }

    // Parse Token list returned from tokenize(). Returns our AST.
    public void parse(List<Token> tokens) {

    }

    void traverse() {

    }

    void transform() {

    }
}

// A token.
public struct Token {
    public TType type;
    public string value;

    public Token(TType _type, string _value) {
        type = _type;
        value = _value;
    }
}


/* * * * *
 * This object encapsulates functions that convert a Token[] to an Abstract
 * Syntax Tree (AST).
 * * * * */
public class ASTParser {
    
    private LexContext context;
    private int idx;        // Current index of the token list.
    private Token[] tokens;
    private Token cur;      // The token we are looking at.
    private Token next;     // The token after cur.
    private List<Statement> program;
    private ExprNode curExpr;   // The expression being built by parseExpression.

    public ASTParser() {
        
    }

    // Parse a list of tokens into an AST and return.
    public List<Statement> parse(Token[] _tokens) {
        tokens = _tokens;
        program = new List<Statement>();
        idx = 0;
        while (idx < tokens.Length) {
            program.Add(parseStatement());
        }
        return program;
    }

    // Parses and returns a statement.
    public Statement parseStatement() {
        // Eat first token of statment.
        eatTokens();
        if (cur.type == TType.KEY_IF) {
            IfStatement s = new IfStatement();
            // Parse the conditional expr for the if statement.
            // (this should start with the '(' token...)
            eatTokens();
            s.condition = parseExpression();

            // Get ready to read child statements of the current if statement.
            eatEOS();
            context.tdepth++;
            s.tdepth = context.tdepth;
            context.pushScope();

            // While we have not broken out of this if block, read in child statements.
            while(context.tdepth >= s.tdepth) {
                s.children.Add(parseStatement());
                eatEOS();
            }
            context.popScope();
            return s;
        }
        else if (cur.type == TType.KEY_WHILE) {
            WhileStatement s = new WhileStatement();
            // Parse the conditional expr for the while statement.
            // (this should start with the '(' token...)
            eatTokens();
            s.condition = parseExpression();

            // Get ready to read child statements of the current while statement.
            eatEOS();
            context.tdepth++;
            s.tdepth = context.tdepth;
            context.pushScope();

            // While we have not broken out of this while block, read in child statements.
            while(context.tdepth >= s.tdepth) {
                s.children.Add(parseStatement());
                eatEOS();
            }
            context.popScope();
            return s;
        }
        else if (cur.type == TType.KEY_WAIT) {
            WaitStatement s = new WaitStatement();
            eatTokens();
            s.time = parseExpression();
            eatEOS();
            return s;
        }
        else if (cur.type == TType.KEY_BOOL ||
                 cur.type == TType.KEY_INT ||
                 cur.type == TType.KEY_FLOAT ||
                 cur.type == TType.KEY_STR) {
            VarDefStatement s = new VarDefStatement();
            s.type = cur.type;
            s.expr = parseExpression();
            eatEOS();
            return s;
        }
        else {
            ExprStatement s = new ExprStatement();
            s.expr = parseExpression();
            eatEOS();
            return s;
        }
        return null;
    }

    // Parse and return an expression. Assumes idx is pointing to the first
    // token in the expression (excluding any '[]'s or '()'s.)
    public ExprNode parseExpression() {
        eatTokens();
        if (cur.type == TType.IDENTIFIER) {
            IdentifierExpr identExpr = new IdentifierExpr(cur.value);
            if (next.type == TType.L_PAREN) {
                FuncExpr funcExpr = new FuncExpr(identExpr);
                eatTokens();
                context.pdepth++;
                while(cur.type != TType.R_PAREN) {
                    funcExpr.children.Add(parseExpression());    // Read params for function call.
                }
                context.pdepth--;
                return funcExpr;
            }
            else if (next.type == TType.L_BRACKET) {
                IndexingExpr expr = new IndexingExpr(identExpr);
                eatTokens();
                context.bdepth++;
                expr.children.Add(parseExpression());   // Read index value.
                context.bdepth--;
                return expr;
            }
            // Return new IdentifierExpr by default.
            return identExpr;
        }
        // All literal expressions.
        else if (cur.type == TType.INT_LITERAL  ||
                 cur.type == TType.STR_LITERAL  ||
                 cur.type == TType.BOOL_LITERAL ||
                 cur.type == TType.FLOAT_LITERAL) { 
            return new LiteralExpr(cur.type, cur.value);
        }
        // All binary expressions.
        else if (cur.type == TType.OP_ASSIGNMENT     || 
                 cur.type == TType.OP_ADDITION       ||
                 cur.type == TType.OP_SUBTRACTION    ||
                 cur.type == TType.OP_MULTIPLICATION ||
                 cur.type == TType.OP_DIVISION       ||
                 cur.type == TType.OP_EQUALITY       ||
                 cur.type == TType.OP_NOTEQUALS      ||
                 cur.type == TType.OP_GREATER        ||
                 cur.type == TType.OP_GREATEROREQUAL ||
                 cur.type == TType.OP_LESS           ||
                 cur.type == TType.OP_LESSOREQUAL    ||
                 cur.type == TType.OP_AND            ||
                 cur.type == TType.OP_OR             || 
                 cur.type == TType.OP_COMMA) { 
            BinExpr expr = new BinExpr(cur.type);
            //TODO: add last pushed expression as left?
            expr.children.Add(parseExpression());
            return expr;
        }
        // All unary expressions. 
        else if (cur.type == TType.OP_NEGATION ||
                 cur.type == TType.OP_INVERSE) {
            UnaryExpr expr = new UnaryExpr();
            expr.type = cur.type;
            expr.children.Add(parseExpression());
            return expr;
        }
        // '(' and ')' encountered outside of a function call expression.
        else if (cur.type == TType.L_PAREN) {
            context.pdepth++;
            return parseExpression();
        }                         
        // TODO: Does this ever happen (should be accounted for by L_PAREN...)                          
        else if (cur.type == TType.R_PAREN) {
            context.pdepth--;
        }
        return null;
    }

    public void updateCurExpr(ExprNode expr) {
       // how to update curExpr correctly using new expr? 
    }
    
    // Iterate idx by num and update cur and next.
    public void eatTokens(int num=1) {
        idx += num;
        cur = tokens[idx];
        next = tokens[idx + 1];
    }

    // Eats tokens after a statement has ended up until the start of the next
    // statement. Updates context.tdepth value.
    public void eatEOS() {
        int tcount = 0;
        while(next.type == TType.WS_NEWLINE && next.type == TType.WS_TAB) {
            eatTokens();
            if (cur.type == TType.WS_TAB) {
                tcount++;
            }
            else if (cur.type ==  TType.WS_NEWLINE) {
                tcount = 0;
            }
        }
        if (tcount > context.tdepth) {
            // Error too many tabs
        }
        else {
            context.tdepth = tcount;
        }
    }
}


/* * * * *
 * This keeps track of the 'context' while we are parsing the script.
 *  - \t, (), and [] counters
 *  - Tracks which variables are visible within the current scope.
 * * * * */
public class LexContext {
    public int pdepth;
    public int bdepth;
    public int tdepth;
    private List<Scope> scopes;

    public LexContext() {
        scopes = new List<Scope>();
        pdepth = 0;
        bdepth = 0;
        tdepth = 0;
    }
    
    // Are we currently inside of any [] or ()?
    public bool enclosed() {
        if (bdepth > 0 || bdepth > 0) {
            return true;
        }
        return false;
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
    public void addVar(Token token) {
        scopes[scopes.Count-1].vars.Add(token); 
    }

    // Check if the var exists in the current context.
    public bool hasVar(Token token) {
        foreach(Scope s in scopes) {
            if (s.hasVar(token)) {
                return true;
            }
        }
        return false;
    }
}

// Keeps track of what variables are defined in the current scope.
public struct Scope {
    public List<Token> vars;
    public bool hasVar(Token token) { return vars.Contains(token); }
}


/* * * * *
 * These objects define the nodes that we will use to construct expressions in
 * out AST.
 * * * * */
// Parent for all Expression classes.
public class ExprNode {
    public List<ExprNode> children;
    public override string ToString() { return "Base ExprNode"; }
}

// Identifier expression.
public class IdentifierExpr : ExprNode {
    public string name;     // Name of the identifier.
    public IdentifierExpr(string n) { 
        name = n; 
        children = null;    // Identifiers have no children.
    }
    public override string ToString() { return name; }
}

// Literal expressions.
public class LiteralExpr : ExprNode {
    public TType type;      // Type of literal.
    public string value;    // Value of literal.
    public LiteralExpr(TType t, string v) { 
        type = t; 
        value = v;
        children = null;    // Literals have no children.
    }
    public override string ToString() { return value; }
}

// Indexing expression.
//  children[0] == identifier expression
//  children[1] == indexing expression
public class IndexingExpr : ExprNode {
    public IndexingExpr(IdentifierExpr _identifier) { 
        children = new List<ExprNode>();
        children.Add(_identifier);
    }
    public IndexingExpr(IdentifierExpr _identifier, ExprNode _index) {
        children = new List<ExprNode>();
        children.Add(_identifier);
        children.Add(_index);
    }
    public override string ToString() { 
        return $"{children[0].ToString()}[{children[1].ToString()}]";
    }
}

// Function call expression.
//  children[0] == identifier expression
//  children[1] == parameters
public class FuncExpr : ExprNode {
    public FuncExpr(IdentifierExpr _identifier) { 
        children = new List<ExprNode>();
        children.Add(_identifier);
    }
    public FuncExpr(IdentifierExpr _identifier, ExprNode _parameters) { //TODO: change param type to other thing.
        children = new List<ExprNode>();
        children.Add(_identifier);
        children.Add(_parameters);
    }
    public override string ToString() {
        return $"{children[0].ToString()}({children[1].ToString()})";
    }
}

// Binary expressions.
//  children[0] == left hand expression
//  children[1] == right hand expression
public class BinExpr : ExprNode {
    public TType type;                  // The type of op this is (+,-,*,etc...)
    
    public BinExpr(TType _type) {
        type = _type;
        children = new List<ExprNode>();
    }
    public override string ToString() {
        return $"({children[0].ToString()} {type.ToString()} {children[1].ToString()})";
    }
}

// Unary expressions.
//  children[1] == right hand expression
public class UnaryExpr: ExprNode {
    public TType type;                  // The type of op (-,!)
    public override string ToString() {
        return $"({type.ToString()} {children[0].ToString()})";
    }
}

/* * * * *
 * These objects define the types of statements that will comprise the program.
 * * * * */
// Parent for all Statement classes.
public class Statement {
    
}

// if
public class IfStatement : Statement {
    public ExprNode condition;          // Condition for execution.
    public List<Statement> children;    // Statements to execute.
    public int tdepth;                  // Desired tdepth for children of this if block.
}
// while
public class WhileStatement : Statement {
    public ExprNode condition;          // Condition for execution.
    public List<Statement> children;    // Statements to execute.
    public int tdepth;                  // Desired tdepth for children of this while block.
}
// wait
public class WaitStatement : Statement {
    public ExprNode time;               // Amount of time to wait.
}
// var defs.
public class VarDefStatement : Statement {
    public TType type;
    public ExprNode expr;
}
// expression
public class ExprStatement : Statement {
    public ExprNode expr;
}
// pure whitespace or comment.
public class NoOpStatement : Statement{

}
    
// Enum defines all types of tokens.
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
    STR_LITERAL
}
