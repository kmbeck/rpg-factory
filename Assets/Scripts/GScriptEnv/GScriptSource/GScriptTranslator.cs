using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * Functionality to translate legal GScript into C#
 * * * * */

public class GScriptTranslator 
{

    public GScriptTranslator() {

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
        if (s.varDefVType == VType.LIST) {
            retval += $"GScriptListObjWrapper<{translateVType(s.expr.children[0].elementType)}>";
        }
        else {
            retval += translateVType(s.varDefVType);
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
        string retval = "";
        switch (e.eType) {
            case EType.IDENTIFIER:
                retval += translateIdentifierExpr(e);
                break;
            case EType.LITERAL:
                retval += translateLiteralExpr(e);
                break;
            case EType.LIST_LITERAL:
                retval += translateListLiteralExpr(e);
                break;
            case EType.FUNCTION:
                retval += translateFunctionCallExpr(e);
                break;
            case EType.INDEXING:
                retval += translateIndexingExpr(e);
                break;
            case EType.BINARY:
                retval += translateBinaryExpr(e);
                break;
            case EType.UNARY:
                retval += translateUnaryExpr(e);
                break;
            case EType.TYPE_CAST:
                retval += translateTypeCastExpr(e);
                break;
        }
        return retval;
    }

    string translateIdentifierExpr(ExprNode e) {
        if (SODB.LIB_FLAG.lib.ContainsKey(e.value)) {
            return $"GScriptFlagLibrary.{e.value}";
        }
        return $"{e.value}";
    }

    string translateLiteralExpr(ExprNode e) {
        if (e.vType == VType.FLOAT) {
            return $"{e.value}f";
        }
        else {
            return $"{e.value}";
        }
    }

    string translateListLiteralExpr(ExprNode e) {
        string vals = "";
        vals += translateExpr(e.children[0]);
        return $"new GScriptListObjWrapper<{translateVType(e.children[0].vType)}>() {{{vals}}}";
    }

    string translateBinaryExpr(ExprNode e) {
        return $"{translateExpr(e.children[0])}{translateTType(e.tType)}{translateExpr(e.children[1])}";
    }

    string translateUnaryExpr(ExprNode e) {
        return $"{translateTType(e.tType)}{translateExpr(e.children[0])}";
    }

    string translateFunctionCallExpr(ExprNode e) {
        string paramStr = "";
        // Skip first child of expr which is the function identifier (i = 1).
        for (int i = 1; i < e.children.Count; i++) {
            if (e.children[i].vType == VType.NONE) { continue; }
            paramStr += translateExpr(e.children[i]) + ",";
        }
        if (paramStr.Length > 0) { 
            paramStr = paramStr.Remove(paramStr.Length - 1, 1); 
        }

        // Check to see if we are translating a native list function. Otherwise assume we are dealing with
        // an EventInterface function.
        string funcName = "";
        if (e.parent != null && e.parent.tType == TType.OP_ACCESSOR) {
            funcName = translateExpr(e.children[0]);
            funcName = funcName.Remove(0, 1);
        }
        else {
            funcName = translateExpr(e.children[0]);
            string typeStr = GScriptContextualizer.getContextualizedFunc(funcName).originType.ToString();
            funcName = $"{typeStr}.{funcName}";
        }
        return $"{funcName}({paramStr})";
    }

    string translateIndexingExpr(ExprNode e) {
        return $"{translateExpr(e.children[0])}[{translateExpr(e.children[1])}]";
    }

    string translateTypeCastExpr(ExprNode e) {
        if (e.vType == VType.STRING) {
            return $"({translateExpr(e.children[0])}).ToString()";
        }
        else {
            return $"({translateVType(e.vType)}){translateExpr(e.children[0])}";
        }
    }

    // Translate the input token into a string. NOT ALL TOKENS ARE IN HERE!!!
    string translateTType(TType tType) {
        switch (tType) {
            case TType.OP_NEGATION:
                return "!";
            case TType.OP_INVERSE:
                return "-";
            case TType.OP_MULTIPLICATION:
                return "*";
            case TType.OP_DIVISION:
                return "/";
            case TType.OP_MODULUS:
                return "%";
            case TType.OP_ADDITION:
                return "+";
            case TType.OP_SUBTRACTION:
                return "-";
            case TType.OP_LESS:
                return "<";
            case TType.OP_LESSOREQUAL:
                return "<=";
            case TType.OP_GREATER:
                return ">";
            case TType.OP_GREATEROREQUAL:
                return ">=";
            case TType.OP_EQUALITY:
                return "==";
            case TType.OP_NOTEQUALS:
                return "!=";
            case TType.OP_AND:
                return "&&";
            case TType.OP_OR:
                return "||";
            case TType.OP_ASSIGNMENT:
                return "=";
            case TType.OP_COMMA:
                return ",";
            case TType.OP_ACCESSOR:
                return ".";
        }
        //TODO: Error here?
        return "";
    }

    // Translate input vType into valid C# equivalent. 
    // VType.LIST is returned as just "List".
    string translateVType(VType vType) {
        switch(vType) {
            case VType.INT:
                return "int";
            case VType.BOOL:
                return "bool";
            case VType.FLOAT:
                return "float";
            case VType.STRING:
                return "string";
            case VType.LIST:
                return "GScriptListObjWrapper";
            case VType.NONE:
                return "NONE???";
        }
        //TODO: Error here?
        return "";
    }
}
