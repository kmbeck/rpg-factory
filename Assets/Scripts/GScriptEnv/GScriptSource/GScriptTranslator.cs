using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Functionality to translate legal GScript into C#
 * * * * */

public class GScriptTranslator 
{
    TranslatorContext context;

    public GScriptTranslator() {
        context = new TranslatorContext();
    }

    // Translate a GScript program into a string of C# code.
    public string translate(List<Statement> program) {
        string retval = "";
        foreach (Statement s in program) {
            retval += translateStatement(s);
        }
        return retval;
    }

    string translateStatement(Statement s) {
        string retval = "";

        // Pad the start of the line with necissary tabs.
        for(int i = 0; i < s.tdepth; i++) {
            retval += '\t';
        }
        switch (s.type) {
            case SType.EXPR:
                retval += translateExprStatement(s);
                break;
            case SType.IF:
                retval += translateIfStatement(s);
                break;
            case SType.ELIF:
                retval += translateElifStatement(s);
                break;
            case SType.ELSE:
                retval += translateElseStatement(s);
                break;
            case SType.VAR_DEF:
                retval += translateVarDefStatement(s);
                break;
            case SType.WAIT:
                retval += translateWaitStatement(s);
                break;
            case SType.WHILE:
                retval += translateWhileStatement(s);
                break;
            case SType.NONE:
                //Error: cannot declare variable of NONE type.
                break;
        }
        return retval;
    }

    string translateExprStatement(Statement s) {
        return translateExpr(s.expr) + ";\n";
    }

    string translateIfStatement(Statement s) {
        string retval = $"if ({translateExpr(s.expr)}) {{\n";
        foreach (Statement c in s.children) {
            retval += translateStatement(c);
        }
        retval += "}\n";
        return retval;
    }

    string translateElifStatement(Statement s) {
        string retval = $"else if ({translateExpr(s.expr)}) {{\n";
        foreach (Statement c in s.children) {
            retval += translateStatement(c);
        }
        retval += "}\n";
        return retval;
    }

    string translateElseStatement(Statement s) {
        string retval = $"else {{\n";
        foreach (Statement c in s.children) {
            retval += translateStatement(c);
        }
        retval += "}\n";
        return retval;
    }

    string translateVarDefStatement(Statement s) {
        string retval = "";
        switch(s.varDefVType) {
            case VType.BOOL:
                retval += "bool";
                break;
            case VType.INT:
                retval += "int";
                break;
            case VType.FLOAT:
                retval += "float";
                break;
            case VType.STRING:
                retval += "string";
                break;
        }
        retval += " ";
        retval += translateExpr(s.expr) + ";\n";
        return retval;
    }

    string translateWaitStatement(Statement s) {
        //TODO: IMPLEMENT WAIT STATEMENTS & PARALLEL SCRIPT EXECUTION?!?!?
        return "";
    }

    string translateWhileStatement(Statement s) {
        string retval = $"while ({translateExpr(s.expr)}) {{\n";
        foreach (Statement c in s.children) {
            retval += translateStatement(c);
        }
        retval += "}\n";
        return retval;
    }

    string translateExpr(ExprNode e) {
        return e.ToString();   
    }
}

public class TranslatorContext {
    int tdepth;

    public TranslatorContext() {
        tdepth = 0;
    }
}