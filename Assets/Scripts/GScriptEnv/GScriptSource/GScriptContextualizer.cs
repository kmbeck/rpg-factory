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

public static class GScriptContextualizer
{
    public static List<ScopeVar> vars = new List<ScopeVar>();
    public static List<ScopeFunc> funcs = new List<ScopeFunc>();

    // Generate a ScopeVar for each field in specified type T.
    public static void registerContextualizedScopeVars<T>(BindingFlags flags=BindingFlags.Public|BindingFlags.Static|BindingFlags.DeclaredOnly) {
        FieldInfo[] fields = typeof(T).GetFields(flags);
        foreach (FieldInfo f in fields) {
            if (f.GetCustomAttribute(typeof(GScript)) != null)  {
                vars.Add(new ScopeVar(f.Name.ToUpper(), getTypeAsVType(typeof(T))));
            }
        }
    }

    // Generate a ScopeFunc for each method in specified type T.
    public static void registerContextualizedScopeFuncs<T>(BindingFlags flags=BindingFlags.Public|BindingFlags.Static) {
        MethodInfo[] methods = typeof(T).GetMethods(flags);
        foreach (MethodInfo m in methods) {
            if (m.GetCustomAttribute(typeof(GScript)) != null) {
                List<ScopeParam> parameters = new List<ScopeParam>();
                foreach(ParameterInfo p in m.GetParameters()) {
                    Debug.Log(p.Name);
                    VType type = getTypeAsVType(p.ParameterType);
                    parameters.Add(new ScopeParam(p.Name, type, !p.HasDefaultValue));
                }
                funcs.Add(new ScopeFunc(m.Name, getTypeAsVType(m.ReturnType), typeof(T), parameters));
            }
        }
    }

    public static void registerNativeScopeFuncs<T>(BindingFlags flags=BindingFlags.Public) {
        List<ScopeFunc> funcs = new List<ScopeFunc>();
        foreach (MethodInfo m in typeof(T).GetMethods(flags)) {
            List<ScopeParam> parameters = new List<ScopeParam>();
            foreach (ParameterInfo p in m.GetParameters()) {
                VType type = getTypeAsVType(p.ParameterType);
                parameters.Add(new ScopeParam(p.Name, type, !p.HasDefaultValue));
            }
            funcs.Add(new ScopeFunc(m.Name, getTypeAsVType(m.ReturnType), typeof(T), parameters));
        }
    }

    public static void registerContextualizedFlags() {
        List<ScopeVar> newVars = generateContextualizedFlags();
        foreach (ScopeVar v in newVars) { vars.Add(v); }
    }

    public static List<ScopeVar> generateContextualizedFlags() {
        List<ScopeVar> retval = new List<ScopeVar>();
        foreach (KeyValuePair<string, SOFlag> f in SODB.LIB_FLAG.lib) {
            switch (f.Value.dataType) {
                case FlagDataType.INT:
                    retval.Add(new ScopeVar(f.Key, VType.INT));
                    break;
                case FlagDataType.STRING:
                    retval.Add(new ScopeVar(f.Key, VType.STRING));
                    break;
                case FlagDataType.BOOL:
                    retval.Add(new ScopeVar(f.Key, VType.BOOL));
                    break;
                case FlagDataType.FLOAT:
                    retval.Add(new ScopeVar(f.Key, VType.FLOAT));
                    break;
            }
        }
        return retval;
    }

    public static ScopeVar[] getContextualizedVars() {
        return vars.ToArray();
    }

    public static ScopeVar getContextualizedVar(string name) {
        foreach (ScopeVar v in vars) {
            if (v.name == name) { return v; }
        }
        return null;
    }

    public static ScopeFunc[] getContextualizedFuncs() {
        return funcs.ToArray();
    }

    public static ScopeFunc getContextualizedFunc(string name) {
        foreach (ScopeFunc f in funcs) {
            if (f.name == name) { return f; }
        }
        return null;
    }

    // Reset vars and funcs
    public static void flush() {
        vars.Clear();
        funcs.Clear();
    }

    // Try to translate between a regualar c# type and a VType
    public static VType getTypeAsVType(Type t) {
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