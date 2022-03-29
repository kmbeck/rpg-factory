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
    public List<GScriptException> exceptions;

    public GScriptExceptionHandler() {
        exceptions = new List<GScriptException>();
    }

    public void log(string message) {
        exceptions.Add(new GScriptException(message));
    }

    public bool empty() {
        return exceptions.Count == 0 ? true : false;
    }

    // Print all exceptions to Debug.LogError()
    public void printAllExceptions() {
        foreach (GScriptException e in exceptions) {
            Debug.LogError(e.message);
        }
    }
}

public class GScriptException {
    public string message;

    public GScriptException(string _message) {
        message = _message;
    }
}