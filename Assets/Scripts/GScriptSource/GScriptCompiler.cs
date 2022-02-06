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

    // Parse Token list returned from tokenize(). Returns list of statements
    // equivalent to the entire program.
    public List<Statement> parse(Token[] tokens) {
        return new ASTParser().parse(tokens);
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
