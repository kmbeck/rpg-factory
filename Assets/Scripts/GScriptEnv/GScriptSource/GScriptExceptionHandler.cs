using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* * * * *
 * An interface for queueing Exceptions while traversing a GScript program.
 * Allows exceptions to be registered and printed in a simple & uniform way.
 * TODO: better way to print multiple similar error messages? Log an error enum
 *      type and each error type has it's own message?
 * * * * */

public class GScriptExceptionHandler
{
    public static List<GScriptException> exceptions;

    public GScriptExceptionHandler() {
        // Only initialize exceptions if it has not been initialized yet.
        if (exceptions == null) {
            exceptions = new List<GScriptException>();
        }
    }

    public void log(string message) {
        exceptions.Add(new GScriptException(message));
    }

    public bool isEmpty() {
        return exceptions.Count == 0 ? true : false;
    }

    public void flush() {
        exceptions.Clear();
    }

    // Print all exceptions to Debug.LogError(). Flushes exception list when done.
    public void printAllExceptions() {
        foreach (GScriptException e in exceptions) {
            Debug.LogError(e.message);
        }
        flush();
    }
}

public class GScriptException {
    public string message;

    public GScriptException(string _message) {
        message = _message;
    }
}