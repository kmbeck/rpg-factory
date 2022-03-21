using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * This object encapsulates functions that convert a Token[] to an Abstract
 * Syntax Tree (AST).
 * * * * */
public class ASTParser {
    
    private LexContext context;
    private int idx;        // Current index of the token list.
    private Token[] tokens;
    private Token start;    // The token that the pack starts with 
    private Token cur;      // The token we are looking at.
    private Token next;     // The token after cur.
    private List<Statement> program;
    private ExprNode curExpr;   // The last expression returned by parseExpr().

    public ASTParser() {

    }

    // Parse a list of tokens into an AST and return.
    public List<Statement> parse(Token[] _tokens) {
        context = new LexContext();
        idx = -1;
        tokens = _tokens;
        program = new List<Statement>();
        //eatTokens();    // Set up token pointers for first pass.
        next = tokens[0];
        while (!reachedEOF()) {
            Statement s = parseStatement();
            program.Add(s);
        }
        return program;
    }

    // Parses and returns a statement.
    public Statement parseStatement() {
        // curExpr never carries over between statements?
        curExpr = new ExprNode(EType.NONE);

        // Eat first token of statment.
        if (next.type == TType.KEY_IF) {
            Statement s = new Statement(SType.IF);
            // Parse the conditional expr for the if statement.
            // (this should start with the '(' token...)
            eatTokens();
            s.expr = parseAndGetExpression();

            // Get ready to read child statements of the current if statement.
            eatEOS();
            context.tdepth++;
            s.tdepth = context.tdepth;

            // While we have not broken out of this if block, read in child statements.
            while(context.tdepth >= s.tdepth) {
                s.children.Add(parseStatement());
                eatEOS();
            }
            return s;
        }
        else if (next.type == TType.KEY_WHILE) {
            Statement s = new Statement(SType.WHILE);
            // Parse the conditional expr for the while statement.
            // (this should start with the '(' token...)
            eatTokens();
            s.expr = parseAndGetExpression();

            // Get ready to read child statements of the current while statement.
            eatEOS();
            context.tdepth++;
            s.tdepth = context.tdepth;

            // While we have not broken out of this while block, read in child statements.
            while(context.tdepth >= s.tdepth) {
                s.children.Add(parseStatement());
                eatEOS();
            }
            return s;
        }
        else if (next.type == TType.KEY_WAIT) {
            Statement s = new Statement(SType.WAIT);
            eatTokens();
            s.expr = parseAndGetExpression();
            eatEOS();
            return s;
        }
        else if (next.type == TType.KEY_BOOL ||
                 next.type == TType.KEY_INT ||
                 next.type == TType.KEY_FLOAT ||
                 next.type == TType.KEY_STR) {
            Statement s = new Statement(SType.VAR_DEF);
            switch (next.type) {
                case TType.KEY_BOOL:
                    s.varDefVType = VType.BOOL;
                    break;
                case TType.KEY_INT:
                    s.varDefVType = VType.INT;
                    break;
                case TType.KEY_FLOAT:
                    s.varDefVType = VType.FLOAT;
                    break;
                case TType.KEY_STR:
                    s.varDefVType = VType.STRING;
                    break;
                default:
                    s.varDefVType = VType.NONE;
                    // TODO: ERROR: Invalid keyword in VAR_DEF statement.
                    // Should we wait to throw this error until the
                    // traverse?
                    break;
            }
            eatTokens();
            s.expr = parseAndGetExpression();
            eatEOS();
            return s;
        }
        else {
            Statement s = new Statement(SType.EXPR);
            s.expr = parseAndGetExpression();
            eatEOS();
            return s;
        }
        return null;
    }

    // Wrapper for parseExpression that also returns the value of curExpr.
    private ExprNode parseAndGetExpression() {
        // Parse expression until unenclosed newline or eof is reached.
        while((!context.enclosed() && next.type != TType.WS_NEWLINE) && !reachedEOF()) {
            parseExpression();
        }
        curExpr.lineNum = cur.location[1];
        ExprNode rootExpr = curExpr.getRoot();
        // Debug print all expressions in their own msg.
        Debug.Log(rootExpr.ToString());
        return rootExpr;
    }

    // Parses an expression and updates the value of curExpression. 
    // Assumes idx is pointing to the first token in the expression 
    // (excluding any '[]'s or '()'s.)
    private void parseExpression() {
        // Dont eat token on first statement...
        eatTokens();

        // Return if we are at the end of the token stream.
        if (cur.type == TType.IDENTIFIER) {
            ExprNode identExpr = new ExprNode(EType.IDENTIFIER);
            identExpr.value = cur.value;
            curExpr = identExpr;
            if (next.type == TType.L_PAREN) {
                ExprNode expr = new ExprNode(EType.FUNCTION);
                expr.addChild(identExpr);
                eatTokens();    // Eat up to opening '('
                context.pdepth++;
                while(next.type != TType.R_PAREN) {
                    parseExpression();
                }
                expr.addChild(curExpr);
                eatTokens();
                context.pdepth--;
                curExpr = expr;
            }
            else if (next.type == TType.L_BRACKET) {
                ExprNode expr = new ExprNode(EType.INDEXING);
                expr.addChild(identExpr);
                eatTokens();
                context.bdepth++;
                parseExpression();
                expr.addChild(curExpr);   // Read index value.
                context.bdepth--;
                curExpr = expr;
            }
            else if (next.type == TType.OP_ASSIGNMENT) {
                ExprNode expr = new ExprNode(EType.BINARY);
                expr.tType = TType.OP_ASSIGNMENT;
                expr.addChild(curExpr);
                eatTokens();    // Eat '=' token.
                parseExpression();
                expr.addChild(curExpr);
                curExpr = expr;
            }
        }
        // All literal expressions.
        else if (cur.type == TType.INT_LITERAL  ||
                 cur.type == TType.STR_LITERAL  ||
                 cur.type == TType.BOOL_LITERAL ||
                 cur.type == TType.FLOAT_LITERAL) {
            ExprNode expr = new ExprNode(EType.LITERAL);
            expr.tType = cur.type;
            expr.value = cur.value;
            switch (cur.type) {
                case TType.INT_LITERAL:
                    expr.vType = VType.INT;
                    break;
                case TType.STR_LITERAL:
                    expr.vType = VType.STRING;
                    break;
                case TType.BOOL_LITERAL:
                    expr.vType = VType.BOOL;
                    break;
                case TType.FLOAT_LITERAL:
                    expr.vType = VType.FLOAT;
                    break;
                //TODO: fall-through case if value is JACKED??
            }
            curExpr = expr;
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
            ExprNode expr = new ExprNode(EType.BINARY);
            expr.tType = cur.type;
            if (curExpr != null) {
                // If curExpr is binary expr, add it based on operator precedence.
                if (curExpr.eType == EType.BINARY) {
                    if ((int)curExpr.tType <= (int)expr.tType) {
                        expr.addChild(curExpr);
                        parseExpression();
                        expr.addChild(curExpr);
                    }
                    else {
                        ExprNode temp = curExpr.children[1];
                        curExpr.children.RemoveAt(1);
                        curExpr.addChild(expr);
                        expr.addChild(temp);
                        parseExpression();
                        expr.addChild(curExpr);
                    }
                }
                // Otherwise add curExpr as left-hand value.
                else {
                    expr.addChild(curExpr);
                    parseExpression();
                    expr.addChild(curExpr);
                }
            }
            curExpr = expr;
        }
        // All unary expressions. 
        else if (cur.type == TType.OP_NEGATION ||
                 cur.type == TType.OP_INVERSE) {
            ExprNode expr = new ExprNode(EType.UNARY);
            expr.tType = cur.type;
            expr.addChild(parseAndGetExpression());
            curExpr = expr;
        }
        // '(' and ')' encountered (other than a function call expression).
        else if (cur.type == TType.L_PAREN) {
            //ExprNode expr = new ExprNode(EType.EXPRESION);
            context.pdepth++;
            while (cur.type != TType.R_PAREN) {
                parseExpression();
            }
            context.pdepth--;
            return;
        }
        else if (cur.type == TType.R_PAREN) {
            context.pdepth--;
            return;
        }

        // Check if we have reached end of a statement...
        if ((next.type == TType.WS_NEWLINE && !context.enclosed()) || reachedEOF()) {
            return;
        }
    }
    
    // Iterate idx by num and update cur and next.
    private void eatTokens(int num=1) {
        // if we are at end of token stream, point cur and next to last token & return.
        if (idx + num >= tokens.Length) {
            idx = tokens.Length - 1;
            cur = tokens[idx];
            next = tokens[idx];
            //Debug.Log($"[[{idx.ToString()}/{tokens.Length-1}]] Type: {cur.type.ToString()}, Val: {cur.value}  ->  Type: {next.type.ToString()}, Val: {next.value}");
            return;
        }

        idx += num;
        cur = tokens[idx];

        // Return if next os OOB.
        if (idx + 1 < tokens.Length) { 
            next = tokens[idx + 1];
        }
        else {
            next = tokens[tokens.Length - 1];
        }
        //Debug.Log($"[[{idx.ToString()}/{tokens.Length-1}]] Type: {cur.type.ToString()}, Val: {cur.value}  ->  Type: {next.type.ToString()}, Val: {next.value}");
    }

    // Eats tokens after a statement has ended up until the start of the next
    // statement. Updates context.tdepth value.
    private void eatEOS() {
        int tcount = 0;
        while((next.type == TType.WS_NEWLINE || next.type == TType.WS_TAB) && !reachedEOF()) {
            eatTokens();
            if (cur.type == TType.WS_TAB) {
                tcount++;
            }
            else if (cur.type ==  TType.WS_NEWLINE) {
                tcount = 0;
            }
        }
        if (tcount > context.tdepth) {
            // TODO: Throw error: too many tabs!
        }
        else {
            context.tdepth = tcount;
        }
    }

    // Returns next token in token stream without incrementing.
    private Token peek() {
        return tokens[idx + 1];
    }

    // Have we reached the end of the file?
    private bool reachedEOF() {
        return idx >= tokens.Length - 1;
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

    public LexContext() {
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
}

// One statement in the program (line or block)
public class Statement {
    public SType type;
    public ExprNode expr;
    public List<Statement> children;
    public int tdepth;

    // Only used in VAR_DEF statements. Tells us what type the new var should be.
    public VType varDefVType;

    public Statement(SType _type) {
        type = _type;
        expr = null;
        children = new List<Statement>();
        tdepth = 0;
        varDefVType = VType.NONE;
    }

    public override string ToString()
    {
        string retval = "";
        if (expr != null) {
            retval += $"({expr.eType.ToString()})condition: {expr.ToString()}\n";
        }
        foreach (Statement s in children) {
            retval += s.ToString();
        }
        return retval;
    }
}

// One Node in an Expression Tree.
// TODO: add pdepth/bdepth values for expressions?
public class ExprNode {
    public string value;            // Used by Identifiers to denote name and Literals to denote value.
    public EType eType;             // The type of expression this is.
    public TType tType;             // The type of token associated with this expr.
    public List<ExprNode> children; // Child expressions of this expression.
    public ExprNode parent;         // Node of this node.
    public VType vType;             // The type that this expression should ultimately evaluate to.
    public int lineNum;          // The line number in the original source code this exprssion is found on.

    public ExprNode(EType exprType=EType.NONE) {
        value = "";
        eType = exprType;
        tType = TType.NONE;
        children = new List<ExprNode>();
        parent = null;
    }

    public void addChild(ExprNode child) {
        children.Add(child);
        child.parent = this;
    }

    // Iterate through parents to find root node of this Expression.
    public ExprNode getRoot() {
        ExprNode cur = this;
        while (cur.parent != null) {
            cur = parent;
        }
        return cur;
    }

    // Gives us a string representation of the expression depending on it's EType.
    public override string ToString() { 
        string retval = "";
        switch (eType) {
            case EType.IDENTIFIER:
                retval = $"{value}";
                break;
            case EType.LITERAL:
                retval = $"{value}";
                break;
            case EType.INDEXING:
                retval = $"{children[0].ToString()}[{children[1].ToString()}]";
                break;
            case EType.FUNCTION:
                retval = $"{children[0].ToString()}({children[1].ToString()})";
                break;
            case EType.BINARY:
                retval = $"({children[0].ToString()} {tType.ToString()} {children[1].ToString()})";
                break;
            case EType.UNARY:
                retval = $"({tType.ToString()}{children[0].ToString()})";
                break;
            case EType.NONE:
                retval = "NONE";
                break;
        }
        return retval;
    }
}

// Enum defines all types of Statements.
public enum SType {
    IF,
    WHILE,
    WAIT,
    VAR_DEF,
    EXPR,
    NO_OP,
    NONE
}

// Enum defines all types of Exprssions.
public enum EType {
    EXPRESION,
    IDENTIFIER,
    LITERAL,
    FUNCTION,
    INDEXING,
    BINARY,
    UNARY,
    NONE
}

// Enum defines data types of VALUES of Expressions.
public enum VType {
    BOOL,
    INT,
    FLOAT,
    STRING,
    NONE
}