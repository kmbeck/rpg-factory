using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class GScriptTokenizer
{
    // Regex for finding all IDENTIFIERS & KEYWORDS.
    Regex rxIdentifierStart = new Regex("[a-zA-Z_]");       // Start with letter or '_'
    Regex rxIdentifierBody = new Regex("[a-zA-Z0-9_]");     // Body can contain numerals
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
    const string KEY_LIST = "list";
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
    const string OP_MODULUS = "%";
    const string OP_EQUALITY = "==";
    const string OP_NOTEQUALS = "!=";
    const string OP_GREATER = ">";
    const string OP_GREATEROREQUAL = ">=";
    const string OP_LESS = "<";
    const string OP_LESSOREQUAL = "<=";
    const string OP_AND = "&&";
    const string OP_OR = "||";
    const string OP_COMMA = ",";
    const string OP_NEGATION = "!";
    const string OP_INVERSE = "-";
    const string OP_ACCESSOR = ".";
    const string EOF = "EOF";

    public GScriptTokenizer() {
        
    }

    // Convert input into a list of Tokens.
    // NOTE: Every time idx is incremented, xLoc must also be incremented!!!
    //       Otherwise error messages will point to the wrong location in source code LOL
    public List<Token> tokenize(string input) {
        char[] inputChars = input.ToCharArray();    // Expand input string to char array.
        List<Token> tokens = new List<Token>();     // Return value.
        string buf = "";
        int idx = 0;
        int xLoc = 0;   // Column of token.
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
            else if (buf == OP_ACCESSOR) {                              // .
                tokens.Add(new Token(TType.OP_ACCESSOR,buf,new int[2] {xLoc, yLoc}));
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
            else if (buf == OP_MODULUS) {                               // %
                tokens.Add(new Token(TType.OP_MODULUS,buf,new int[2] {xLoc, yLoc}));
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
            else if (rxIdentifierStart.Match(buf).Success) {                 // ALL IDENTIFIERS & KEYWORDS.
                // Read up until end of current Identifier.
                // Check to ensure we havent reached end of input.
                while (idx + 1 < inputChars.Length &&
                        rxIdentifierBody.Match(inputChars[idx + 1].ToString()).Success) {
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
                else if (buf == KEY_LIST) {                                 // list
                    tokens.Add(new Token(TType.KEY_LIST,buf,new int[2] {xLoc, yLoc}));
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
    OP_ACCESSOR,
    OP_NEGATION,
    OP_INVERSE,
    OP_MULTIPLICATION,
    OP_DIVISION,
    OP_MODULUS,
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
    KEY_LIST,
    IDENTIFIER,
    BOOL_LITERAL,
    FLOAT_LITERAL,
    INT_LITERAL,
    STR_LITERAL,
    NONE
}
