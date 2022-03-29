using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/* * * * *
 * Contains functions to easily turn fields & methods of a class into ScopeVars
 * and ScopeFuncs that can then be loaded into the global scope for the
 * GScript engine.
 * * * * */

public class GScriptContextualizer
{
    public GScriptContextualizer() {

    }

    // Generate a ScopeVar for each field in specified type T.
    public ScopeVar[] getContextualizedScopeVars<T>(BindingFlags flags=BindingFlags.Public|BindingFlags.Static|BindingFlags.DeclaredOnly) {
        FieldInfo[] fields = typeof(T).GetFields(flags);
        List<ScopeVar> vars = new List<ScopeVar>();
        foreach (FieldInfo f in fields) {
            vars.Add(new ScopeVar(f.Name.ToUpper(), getTypeAsVType(typeof(T))));
        }
        return vars.ToArray();
    }

    // Generate a ScopeFunc for each method in specified type T.
    public ScopeFunc[] getContextualizedScopeFuncs<T>(BindingFlags flags=BindingFlags.Public|BindingFlags.Static) {
        MethodInfo[] methods = typeof(T).GetMethods(flags);
        List<ScopeFunc> funcs = new List<ScopeFunc>();
        foreach (MethodInfo m in methods) {
            List<ScopeParam> parameters = new List<ScopeParam>();
            foreach(ParameterInfo p in m.GetParameters()) {
                VType type = getTypeAsVType(p.ParameterType);
                parameters.Add(new ScopeParam(p.Name, type, !p.HasDefaultValue));
            }
            funcs.Add(new ScopeFunc(m.Name, getTypeAsVType(m.ReturnType), parameters));
        }
        return funcs.ToArray();
    }

    // Try to translate between a regualar c# type and a VType
    public VType getTypeAsVType(Type t) {
        if (t == typeof(bool)) {
            return VType.BOOL;
        }
        else if (t == typeof(int)) {
            return VType.INT;
        }
        else if (t == typeof(float)) {
            return VType.FLOAT;
        }
        else if (t == typeof(string)) {
            return VType.STRING;
        }
        // Error cannot translate type T to VType...
        return VType.NONE;
    }
}
